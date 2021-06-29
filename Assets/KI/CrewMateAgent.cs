using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class CrewMateAgent : Agent
{
     Rigidbody2D rBody;
     public System.Random random = new System.Random();
     List<Transform>targets=new List<Transform>();
     public Transform[] checkPoints=new Transform[0];
     public Vector2 movement=new Vector2(0,0);
     public float doingTask=0;
     public float report=0;
     public CrewMate crewMateScript;
    void Start () {
    }
    public Transform Target=null;
    /*public override void Initialize() { 
         Debug.Log("Initialize begins");
    }*/
    public override void OnEpisodeBegin()
    {
       // If the Agent fell, zero its momentum
        /*foreach(Task task in crewMateScript.taskToDo)
        {
            targets.Add(task.transform);
        }
        Transform lastTarget=Target;
        // Move the target to a new spot
        Game.Shuffle<Transform>(targets,random);
        Target=targets[0];
        if(Target==lastTarget)
        {
            Target=targets[1];
        }*/
        rBody = GetComponent<Rigidbody2D>();
        crewMateScript=GetComponent<CrewMate>();
        Target=this.transform;
        lastCheckpoint=this.transform;
        lastDistance=Vector3.Distance(this.transform.localPosition, Target.localPosition);
        //Debug.Log("Next Target at "+Target.position);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        // Agent velocity
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.y);

        //checkPoints
        sensor.AddObservation(new Vector3(-999999,-999999,-999999));
        sensor.AddObservation(new Vector3(-999999,-999999,-999999));
        sensor.AddObservation(new Vector3(-999999,-999999,-999999));
        sensor.AddObservation(new Vector3(-999999,-999999,-999999));
        /*sensor.AddObservation(checkPoints[0].localPosition);
        sensor.AddObservation(checkPoints[1].localPosition);
        sensor.AddObservation(checkPoints[2].localPosition);
        sensor.AddObservation(checkPoints[3].localPosition);*/
    }
    public float forceMultiplier = 10;
    public int timeOffside=0;
    public float lastDistance;
    public Transform lastCheckpoint;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 2
        movement.x = actionBuffers.ContinuousActions[0];
        movement.y = actionBuffers.ContinuousActions[1];
        doingTask=actionBuffers.ContinuousActions[2];
        report=actionBuffers.ContinuousActions[3];
        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);
       // Reached target
        if (distanceToTarget < 1.5f)
        {
            if(doingTask<crewMateScript.activation)
            {
                 SetReward(-0.1f);
            }
        }
        SetReward(lastDistance-distanceToTarget);
        if(rBody.velocity.x==0&&rBody.velocity.y==0)
        {
            SetReward(-0.1f); 
        }
        /*foreach(Transform checkpoint in checkPoints)
        {
            if(Vector3.Distance(this.transform.localPosition, checkpoint.localPosition)<0.5f)
            {
                if(lastCheckpoint!=checkpoint)
                {
                    if(Vector3.Distance(Target.localPosition,lastCheckpoint.localPosition)<Vector3.Distance(Target.localPosition,checkpoint.localPosition))
                    {
                        SetReward(-2f);
                    }
                    else
                    {
                        SetReward(2f);
                    }
                    Debug.Log("Next Checkpoint at "+checkpoint.localPosition);
                    lastCheckpoint=checkpoint;
                }
                
            }
        }*/
        lastDistance=distanceToTarget;
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
        continuousActionsOut[2] = Input.GetKey(KeyCode.Return)?1f:0;
        continuousActionsOut[3] = Input.GetKey(KeyCode.Space)?1f:0;
    }
    public void rewardFinishSabortageTask()
    {
        SetReward(200.0f);
    }
    public void rewardFinishTask()
    {
        SetReward(100.0f);
    }
    public void rewardStartTask()
    {
        SetReward(10.0f);
    }
    
    public void rewardWinGame()
    {
        SetReward(1000.0f);
    }
    public void rewardLooseGame()
    {
        SetReward(-1000.0f);
    }
}
