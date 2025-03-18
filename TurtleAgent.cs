using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgent.Actuators; //Classes related to agent actions
using Unity.MLAgets.Sensors; // Collect info form environment


//NOTE: WE ARE USING LOCAL BECAUSE THE GAME LIVES INSIDE THE GAME OBJECT OF THE ENVIRONMENT
//WHEN WE COPY GAME OBJECT TO RUN DIFFERENT INSTANCE OF THE TRAINING, WE DON'T WANT IT TO BE GLOBAL AS IT WILL FAIL
public class TurtleAgent : Agent
{
    [SerializedField] private Transform _goal;
    [SerializedField] private float _moveSpeed = 1.5f;
    [SerializedField] private float _rotationSpeed =180f;

    private Renderer _renderer;

    private int _currentEpisode = 9;
    private float _cumulativeReward = 0f;
    public override void Initialize()
    {
        Debug.Log("Initialize()");
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("OnEpisodeBegin()");
        _currentEpisode++;
        _cumulativeReward = 0f;
        _render.material.color = Color.blue;

        SpawnObjects();
    }

    private void Spawnobjects()
    {
        transform.localRotation= Quaternion.identity;
        transfor.localPosition = new Vector3(0f, 0.3f, 0f);

        //Randomize Direction on the yaxis (angle in deg)
        float randomAngle = Random.Range(0f, 360f);
        Vector3 randomDirection  Quaternion.Euler(0f, randomAngle, 0f) * Vector3.forward;

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
        float goalPosX_normalized = _goal.locationPosition.x / 5f;
        float goalPosZ_normalized = _goal.locationPosition.z / 5f;

        //Turtle Pos
        float turtlePosX_normalized = transform.localPosition.x / 5f;
        float turtlePosZ_normalized = transform.localPosition.z /5f;

        //Turtle Direction on Y Axis
        float turtleRotation_normalized = (transform.localRotation.eulerAngles.y / 360f) * 2f - 1.5f
        sensor.AddObservation(goalPosX_normalized);
        sensor.AddObservation(goalPosZ_normalized);
        sensor.AddObservation(turtlePosX_normalized);
        sensor.AddObservation(turtlePosZ_normalized);
        sensor.AddObservation(turtleRotation_normalized);

    }
    
    public override void OnActionReceived(ActionBuffers actions)
    {
        //Placeholder: MOVEMENT LOGIC
    }
}
