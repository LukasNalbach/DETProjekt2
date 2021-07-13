using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class CrewMateAgent  :Agent
{
     Rigidbody2D rBody;
     public System.Random random = new System.Random();
     int maxTasks=6;
    int maxCheckpoints=22;
     public Vector2 movement=new Vector2(0,0);
     public float doingTask=0;
     public float report=0;
     public CrewMate crewMateScript;
     public float floatPositivInfinity=99999999;
     public float floatNegativeInfinity=-99999999;
    void Start () {
    }
    public override void Initialize() { 
        
    }
    public override void OnEpisodeBegin()
    {
        Debug.Log("On Episode Begin");
        crewMateScript=GetComponent<CrewMate>();
        rBody = GetComponent<Rigidbody2D>();
        time=0;

        gameObject.transform.position=Game.Instance.startPoint;
        lastCheckpoint=new Vector2(0,0);
        lastDistance=distanceToNextTask();
        Game.Shuffle<Task>(Game.Instance.allTasks,Game.Instance.random);
        if(crewMateScript.taskToDo.Count==0)
        {
            for(int i=0;i<Game.Instance.Settings.tasks;i++)
            {
                crewMateScript.addTask(Game.Instance.allTasks[i]);
            }
        }
        
        //Debug.Log("Next Target at "+Target.position);
    }
    public float time;
    public override void CollectObservations(VectorSensor sensor)
    {
        // Agent positions(2)
        sensor.AddObservation(this.transform.localPosition[0]);
        sensor.AddObservation(this.transform.localPosition[1]);
        // Agent velocity(2)
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.y);
        //Tasks(6*2)
        for(int i=0;i<maxTasks;i++)
        {
            if(i<crewMateScript.taskToDo.Count)
            {
                sensor.AddObservation(crewMateScript.taskToDo[i].transform.localPosition[0]);
                sensor.AddObservation(crewMateScript.taskToDo[i].transform.localPosition[1]);
            }
            else
            {
                sensor.AddObservation(floatNegativeInfinity);
                sensor.AddObservation(floatNegativeInfinity);
            }
        }
        //sabortageTasks(2*2) total:20
        List<SabortageTask>allActiveSabortageTasks=Game.Instance.allActiveSabortageTasks();
        for(int i=0;i<2;i++)
        {
            if(i<allActiveSabortageTasks.Count)
            {
                sensor.AddObservation(allActiveSabortageTasks[i].transform.localPosition[0]);
                sensor.AddObservation(allActiveSabortageTasks[i].transform.localPosition[1]);
            }
            else
            {
                sensor.AddObservation(floatNegativeInfinity);
                sensor.AddObservation(floatNegativeInfinity);
            }
        }
        //checkPoints(22*2)total:64
        for(int i=0;i<maxCheckpoints;i++)
        {
            if(i<Game.Instance.allCheckpoints().Count)
            {
                Vector2 checkpoint=Game.Instance.allCheckpoints()[i];
                sensor.AddObservation(checkpoint[0]);
                sensor.AddObservation(checkpoint[1]);
            }
            else
            {
                sensor.AddObservation(floatNegativeInfinity);
                sensor.AddObservation(floatNegativeInfinity);
            }
        }
        //Vent positions(4*2)total:72
        for(int i=0;i<4;i++)
        {
            sensor.AddObservation(Game.Instance.allVents[i].transform.localPosition[0]);
            sensor.AddObservation(Game.Instance.allVents[i].transform.localPosition[1]);
        }
        //Other Players: position(when know), allive(when know), bisheriger verdacht(9*4)total:108
        for(int i=0;i<10;i++)
        {
            if(i!=crewMateScript.number)//die eigene Position ist ja schon wo anders
            {
                if(i>=Game.Instance.allPlayers.Count)
                {
                    sensor.AddObservation(floatNegativeInfinity);
                    sensor.AddObservation(floatNegativeInfinity);
                    sensor.AddObservation(-1f);
                    sensor.AddObservation(-1f);
                }
                else
                {
                    Player player=Game.Instance.allPlayers[i];
                    if(crewMateScript.playerInViewDistance.Contains(player))
                    {
                        sensor.AddObservation(player.transform.localPosition[0]);
                        sensor.AddObservation(player.transform.localPosition[1]);
                        sensor.AddObservation(player.isAlive()?1f:0f);
                        sensor.AddObservation(player.verdacht(i));
                    }
                    else
                    {
                        sensor.AddObservation(floatNegativeInfinity);
                        sensor.AddObservation(floatNegativeInfinity);
                        sensor.AddObservation(player.deadAndInvisible?0f:1f);
                        sensor.AddObservation(player.verdacht(i));
                    }
                }
            }
        }
    }
    public float forceMultiplier = 10;
    public int timeOffside=0;
    public float lastDistance;
    public Vector2 lastCheckpoint;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 4
        movement.x = actionBuffers.ContinuousActions[0];
        movement.y = actionBuffers.ContinuousActions[1];
        doingTask=actionBuffers.ContinuousActions[2];
        report=actionBuffers.ContinuousActions[3];
        if(doingTask<=0.5f)
        {
            SetReward(-1);
        }
        if(movement.x==0||movement.y==0)
        {
            SetReward(1);
        }
        //Debug.Log(movement.x+","+movement.y+","+doingTask+", "+report);
        // Rewards
        //SetReward(-0.1f);//nichts tun wird bestraft
       float distanceNextTask=distanceToNextTask();
       // Reached target
        if (distanceNextTask < 1.5f)
        {
            if(doingTask<crewMateScript.activation)
            {
                 SetReward(-0.1f);
            }
        }
        if(distanceNextTask<Mathf.Infinity&&lastDistance<Mathf.Infinity)
        {
            Debug.Log("Output:"+movement.x+","+movement.y+","+doingTask+","+report);
            //SetReward(lastDistance-distanceNextTask);
        }
        
        //Debug.Log("Last Distance: "+lastDistance+", Current Distance: "+distanceNextTask);
        if(! crewMateScript.immobile()&&Mathf.Abs(lastDistance-distanceNextTask)<=0.001f)
        {
             //SetReward(-1.0f);
             time-=100f;
        }
        else
        {
            //SetReward(0.1f);
        }
        lastDistance=distanceNextTask;
        Vector2 ownPosition=new Vector2(this.transform.position[0],this.transform.position[1]);
        foreach(Vector2 checkpoint in Game.Instance.allCheckpoints())
        {
            if(Vector2.Distance(ownPosition, checkpoint)<0.5f)
            {
                if(lastCheckpoint!=checkpoint)
                {
                    //if(Vector3.Distance(Target.localPosition,lastCheckpoint.localPosition)<Vector3.Distance(Target.localPosition,checkpoint.localPosition))
                    Debug.Log("Reach Checkpoint "+checkpoint);
                    SetReward(0.5f);
                    lastCheckpoint=checkpoint;
                }
            }
        }
        if(time<=-1200f)
        {
            //SetReward(-1f);
            //EndEpisode();
        }
    }
    public float distanceToNextTask()
    {
         float distanceToBestTarget = Mathf.Infinity;
          float newDistance=0f;
        foreach(Task task in crewMateScript.taskToDo)
        {
           newDistance=Vector3.Distance(this.transform.localPosition, task.transform.localPosition);
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
        SetReward(1.0f);
    }
    public void rewardFinishTask()
    {
        Debug.Log("Crew Mate Finish Task");
        SetReward(1.0f);
            EndEpisode();
    }
    public void rewardStartTask()
    {
        Debug.Log("Crew Mate Starts Task");
        SetReward(1.0f);
    }
    
    public void rewardWinGame()
    {
        SetReward(1.0f);
            EndEpisode();
        //OnEpisodeBegin();
    }
    public void rewardLooseGame()
    {
        SetReward(-1.0f);
            EndEpisode();
         //OnEpisodeBegin();
    }
    
    
}   
