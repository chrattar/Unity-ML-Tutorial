using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators; //Classes related to agent actions
using Unity.MLAgents.Sensors;
using UnityEngine.InputSystem; // Collect info form environment


//NOTE: WE ARE USING LOCAL BECAUSE THE GAME LIVES INSIDE THE GAME OBJECT OF THE ENVIRONMENT
//WHEN WE COPY GAME OBJECT TO RUN DIFFERENT INSTANCE OF THE TRAINING, WE DON'T WANT IT TO BE GLOBAL AS IT WILL FAIL
public class TurtleAgent : Agent
{
    [SerializeField] private Transform _goal;
    [SerializeField] private float _moveSpeed = 1.5f;
    [SerializeField] private float _rotationSpeed =180f;

    private Renderer _renderer; 

    [HideInInspector] public int CurrentEpisode = 0;
    [HideInInspector] public float CumulativeReward = 0f;
    public override void Initialize()
    {
        Debug.Log("Initialize()");
        _renderer = GetComponent<Renderer>();  // Add this line
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("OnEpisodeBegin()");
        CurrentEpisode++;
        CumulativeReward = 0f;
        _renderer.material.color = Color.blue;

        SpawnObjects();
    }

    private void SpawnObjects()
    {
        transform.localRotation= Quaternion.identity;
        transform.localPosition = new Vector3(0f, 0.15f, 0f);

        //Randomize Direction on the yaxis (angle in deg)
        float randomAngle = Random.Range(0f, 360f);
        Vector3 randomDirection = Quaternion.Euler(0f, randomAngle, 0f) * Vector3.forward;

        //Randomize the distance with the range [1,2.5]
        float randomDistance = Random.Range(1f, 2.5f);

        //Calcualte the goals position
        Vector3 goalPosition = transform.localPosition + randomDirection * randomDistance;

        //Apply the calculated position to the goal
        _goal.localPosition = new Vector3(goalPosition.x, 0.3f, goalPosition.z);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //Goal's Position
        float goalPosX_normalized = _goal.localPosition.x / 5f;
        float goalPosZ_normalized = _goal.localPosition.z / 5f;

        //Turtle Pos
        float turtlePosX_normalized = transform.localPosition.x / 5f;
        float turtlePosZ_normalized = transform.localPosition.z /5f;

        //Turtle Direction on Y Axis
        float turtleRotation_normalized = (transform.localRotation.eulerAngles.y / 360f) * 2f - 1.5f;
        sensor.AddObservation(goalPosX_normalized);
        sensor.AddObservation(goalPosZ_normalized);
        sensor.AddObservation(turtlePosX_normalized);
        sensor.AddObservation(turtlePosZ_normalized);
        sensor.AddObservation(turtleRotation_normalized);

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] =  0; //DONT MOVE - DO NOTHING
        if (Input.GetKey(KeyCode.UpArrow))
        {
            discreteActionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            discreteActionsOut[0] = 2;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            discreteActionsOut[0] =3;
        }
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        //PMOVE AGENT USING ACTION
        MoveAgent(actions.DiscreteActions);

        //PENALTY GIVEN EACH STEP. ENCOURAGE QUICK FINISH
        AddReward(-2f / MaxStep);

        //UPDATE CUMULATIVE REWARD AFTER INCLUD STEP PENALTY
        CumulativeReward = GetCumulativeReward();
    }

    public void MoveAgent(ActionSegment<int> act)
    {
        var action = act[0];

        switch (action)
        {
            case 1: //FORWARD
                transform.position += transform.forward * _moveSpeed * Time.deltaTime;
                break;
            case 2: //ROTATE LEFT
                transform.Rotate(0f, -_rotationSpeed * Time.deltaTime, 0f);
                break;
            case 3: //ROTATE RIGHT
                transform.Rotate(0f, _rotationSpeed * Time.deltaTime, 0f);
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Goal"))
        {
            GoalReached();
        }
    }

    private void GoalReached()
    {
        AddReward(1.0f); // LARGE REWARD FOR GOAL
       CumulativeReward = GetCumulativeReward();

        EndEpisode();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            //Applly small negative reward when collision begins
            AddReward(-0.05f);
            if (_renderer != null)
            {
                _renderer.material.color = Color.red;
            }
        } 
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        { 
            // Continually Penalize agent while in contact with wall
            AddReward(-0.01f * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            //Reset wall color when colligion ends
            if(_renderer != null)
            { 
            // Continually Penalize agent while in contact with wall
            _renderer.material.color = Color.blue;
            }
        }
    }
}


