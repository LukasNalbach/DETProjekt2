using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrewMate : Player
{
    public bool ki=false;

    public List<Task>taskToDo=new List<Task>();

    public int taskDone;
    private AccursationCrew accursation; 

    private Observation observation;
    //Task the crew mate is currently solving(or null).When a crewMate is doing task, he cannot move
    private Task activeTask;

    private IEnumerator taskCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        agent=this.gameObject.AddComponent<CrewMatePseudoAgent>();
        updateRoom=GetComponent<UpdateRoom>();
        imposter=false;
        taskDone=0;
        activeTask=null;
        accursation=new AccursationCrew(this);
        observation=new Observation(this);
    }

    // Update is called once per frame
    public new void Update()
    {
        if (agent.doingTask>=activation)
        {
            if(canDoTask())
            {
                Task taskToDoNow=null;
                float nearesTaskDistance=Mathf.Infinity;
                float distance;
                foreach (var task in updateRoom.getCurrentRoom().getTasks())
                {
                    if(taskToDo.Contains(task))
                    {
                        distance=Vector3.Distance(gameObject.transform.position, task.transform.position);
                        if(distance<=nearesTaskDistance)
                        {
                            taskToDoNow=task;
                            nearesTaskDistance=distance;
                        }
                    }
                }
                foreach (var task in Game.Instance.allActiveSabortageTasks())
                {
                    
                        distance=Vector3.Distance(gameObject.transform.position, task.transform.position);
                        if(distance<=nearesTaskDistance)
                        {
                            taskToDoNow=task;
                            nearesTaskDistance=distance;
                        }
                    
                }
                doTask(taskToDoNow);
            }
        }
        base.Update();
    }
    public new void FixedUpdate()
    {
        
        agent.time-=Time.deltaTime;
        if(activePlayer())
        {
            float distance;
            foreach (var task in taskToDo)
            {
                distance=Vector3.Distance(gameObject.transform.position, task.transform.position);
                if(distance<=Game.Instance.Settings.viewDistance)
                {
                    task.setActivated();
                }
                else
                {
                    task.setDeactivated();
                }
            }
        }
        base.FixedUpdate();
    }
    public void addTask(Task task)
    {
        taskToDo.Add(task);
    }
    public bool doingTask()
    {
        return activeTask!=null;
    }
    public bool nearPossibleTask()
    {
        foreach (var task in updateRoom.getCurrentRoom().getTasks())
        {
            if(taskToDo.Contains(task)&&Vector3.Distance(gameObject.transform.position, task.transform.position)<=2f)
            {
                return true;
            }
        }
        foreach (var sabTask in Game.Instance.allActiveSabortageTasks())
        {
            if(Vector3.Distance(gameObject.transform.position, sabTask.transform.position)<=2f)
            {
                return true;
            }
        }
        return false;
    }
    public bool canDoTask()
    {
        return !doingTask()&&nearPossibleTask();
    }
    void doTask(Task task)
    {
        agent.rewardStartTask();
        activeTask=task;
        taskCoroutine=coTask(activeTask);
        StartCoroutine(taskCoroutine);
    }

    IEnumerator coTask(Task task)
    {
        task.startSolving(this);
        yield return new WaitForSeconds(task.timeToSolve);
        if(isAlive())
        {
            if(task.sabortageTask)
            {
                ((SabortageTask)task).finishSolving(this);
                agent.rewardFinishSabortageTask();
            }
            else
            {
                task.endSolving(this);
                taskDone++;
                Game.Instance.increaseTaskProgress();
                taskToDo.Remove(task);
                agent.rewardFinishTask();
            }
        }
        activeTask=null;
        
    }
    public void stopAllTasks()
    {
        if(doingTask())
        {
            activeTask.endSolving(this);
            activeTask=null;
            StopCoroutine(taskCoroutine);
        }
    }
    public void getKilledByImposter()
    {
        alive=false;
        stopAllTasks();
        addDeadBody();
        Game.Instance.removeCrewMateFromTaskProgress(this);
        /*
        if(!ki)
        {
            becomeGhost();
        }
        else
        {
            Game.Instance.removeCrewMateFromTaskProgress(this);
        }
        */
    }

    void addDeadBody()
    {
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
    }
    public override void goToMeeting()
    {
        goToMeetingStandard();
        stopAllTasks(); 
    }
    public float processByTask()
    {
        return (float)(taskDone)/Game.Instance.Settings.tasks;
    }

    public override bool immobile()
    {
        return doingTask()||!isAlive();
    }
    public override bool visible()
    {
        return !deadAndInvisible;
    }
    public override void accusePublic()
    {
        if(!activePlayer()&&isAlive())
        {
            accursation.accusePublic();
        }
    }
    public override void accuse()
    {
        if(!activePlayer()&&isAlive())
        {
            accursation.accuse();
        }
    }
    public override void noticePublicAccuse(int p1,int p2)
    {
        if(!activePlayer()&&isAlive())
        {
            accursation.noticePublicAccuse(p1,p2);
        }
    }
    public override void noticePublicDefend(int p1,int p2)
    {
        if(!activePlayer()&&isAlive())
        {
            accursation.noticePublicDefend(p1,p2);
        }
    }
    public override float verdacht(int playerNumber)
    {
        if(activePlayer())
        {
            return AccursationCrew.standardVerdacht;
        }
        return accursation.verdacht[playerNumber];
    }
}
