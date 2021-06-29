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
     int maxTasks=6;
    int maxCheckpoints=22;
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
        lastCheckpoint=this.transform;
        lastDistance=distanceToNextTask();
        //Debug.Log("Next Target at "+Target.position);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Agent positions(2)
        sensor.AddObservation(this.transform.localPosition[0],this.transform.localPosition[1]);

        // Agent velocity(2)
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.y);

        //Tasks(6*2)
        for(int i=0;i<maxTasks;i++)
        {
            if(i<crewMateScript.taskToDo.Count)
            {
                sensor.AddObservation(crewMateScript.taskToDo[i].transform.localPosition[0],
                crewMateScript.taskToDo[i].transform.localPosition[1]);
            }
            else
            {
                sensor.AddObservation(new Vector2(-999999,-999999));
            }
        }
        //checkPoints(22*2)
        for(int i=0;i<maxCheckpoints;i++)
        {
            if(i<Game.Instance.allCheckpoints().Count)
            {
                sensor.AddObservation(Game.Instance.allCheckpoints[i]);
            }
            else
            {
                sensor.AddObservation(new Vector2(-999999,-999999));
            }
        }
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
        SetReward(-0.1f);//nichts tun wird bestraft
       distanceToNextTask=distanceToNextTask();
       // Reached target
        if (distanceToNextTask < 1.5f)
        {
            if(doingTask<crewMateScript.activation)
            {
                 SetReward(-0.1f);
            }
        }
        SetReward(lastDistance-distanceToNextTask);
        lastDistance=distanceToNextTask;
        Vector2 ownPosition=new Vector2(this.transform.position[0],this.transform.position[1]);
        foreach(Vector2 checkpoint in Game.Instance.allCheckpoints())
        {
            if(Vector2.Distance(ownPosition, checkpoint)<0.5f)
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
        }
    }
    public float distanceToNextTask()
    {
         float distanceToBestTarget = Mathf.Infinity;
        foreach(Task task in crewMateScript.taskToDo)
        {
            Vector3 newDistance=Vector3.Distance(this.transform.localPosition, task.transform.localPosition);
            distanceToBestTarget=Mathf.Min(distanceToBestTarget,newDistance);
        }
        return distanceToBestTarget;
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
        //OnEpisodeBegin();
    }
    public void rewardLooseGame()
    {
        SetReward(-1000.0f);
         //OnEpisodeBegin();
    }
}
