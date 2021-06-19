using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrewMate : Player
{
    public new string name="Player";
    public bool ki=false;

    public LinkedList<Task>taskToDo=new LinkedList<Task>();

    public int taskDone;

    //Task the crew mate is currently solving(or null).When a crewMate is doing task, he cannot move
    private Task activeTask;

    private IEnumerator taskCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        updateRoom=GetComponent<UpdateRoom>();
        imposter=false;
        taskDone=0;
        activeTask=null;
    }

    // Update is called once per frame
    public new void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)&&activePlayer())
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
        taskToDo.AddLast(task);
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
                ((SabortageTask)task).finishSolving();
            }
            else
            {
                task.endSolving();
                taskDone++;
                Game.Instance.increaseTaskProgress();
                taskToDo.Remove(task);
            }
        }
        activeTask=null;
        
    }
    public void stopAllTasks()
    {
        if(doingTask())
        {
            activeTask.endSolving();
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
}
