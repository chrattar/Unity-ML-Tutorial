using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgent.Actuators; //Classes related to agent actions
using Unity.MLAgets.Sensors; // Collect info form environment

public class TurtleAgent : Agent
{
    [SerializedField] private Transform _goal;
    [SerializedField] private float _moveSpeed = 1.5f;
    [SerializedField] private float _rotationSpeed =180f;
    public override void Initialize()
    {
        Debug.Log("Initialize()");
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("OnEpisodeBegin()");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //Placeholder: OBSERVATION DATA
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //Placeholder: MOVEMENT LOGIC
    }
}
