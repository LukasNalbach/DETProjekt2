using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class CrewMateAgent : Agent
{
     Rigidbody2D rBody;
    void Start () {
        rBody = GetComponent<Rigidbody2D>();
        Game.Instance.setRooms();
    }
    public Transform Target;
    /*public override void Initialize() { 
         Debug.Log("Initialize begins");
    }*/
    public override void OnEpisodeBegin()
    {
        Debug.Log("Episode begins");
       // If the Agent fell, zero its momentum
        this.rBody.velocity = Vector2.zero;
        this.transform.localPosition = Game.Instance.startPoint;

        // Move the target to a new spot
        Game.Instance.Shuffle<Task>(Game.Instance.allTasks);
        Target=Game.Instance.allTasks[0].transform;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Debug.Log("CollectObservations begins");
        // Target and Agent positions
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        // Agent velocity
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.y);
    }
    public float forceMultiplier = 10;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        Debug.Log("OnActionReceived begins");
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.y = actionBuffers.ContinuousActions[1];
        rBody.AddForce(controlSignal * forceMultiplier);

        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // Reached target
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            EndEpisode();
        }
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        Debug.Log("Heuristics begins");
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}
