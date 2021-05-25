using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrewMate : Player
{
    public string name="Player";
    public bool ki=false;

    private LinkedList<Task>taskToDo=new LinkedList<Task>();

    public int taskDone;

    //When a crewMate is doing task, he cannot move
    private bool doingTask;
    // Start is called before the first frame update
    void Start()
    {
        updateRoom=GetComponent<UpdateRoom>();
        imposter=false;
        taskDone=0;
        doingTask=false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Return))
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
                doTask(taskToDoNow);
            }
        }
    }
    public void addTask(Task task)
    {
        Debug.Log("Player "+name+" gets Task "+task.getTaskNum() +" from Room "+task.getRoom().getRoomNum());
        taskToDo.AddLast(task);
    }
    public bool nearOwnTask()
    {
        foreach (var task in updateRoom.getCurrentRoom().getTasks())
        {
            if(taskToDo.Contains(task))
            {
                return Vector3.Distance(gameObject.transform.position, task.transform.position)<=2f;
            }
        }
        return false;
    }
    public bool canDoTask()
    {
        return !doingTask&&nearOwnTask();
    }
    void doTask(Task task)
    {
        StartCoroutine(coTask(task));
    }

    IEnumerator coTask(Task task)
    {
        task.startSolving();
        doingTask=true;
        Debug.Log("I do a task");
        yield return new WaitForSeconds(task.timeToSolve);
        doingTask=false;
        task.endSolving();
        taskDone++;
        Game.Instance.increaseTaskProgress();
        taskToDo.Remove(task);
    }

    public void getKilledByImposter()
    {
        alive=false;
        addDeadBody();
        if(!ki)
        {
            getGhost();
        }
        else
        {
            Game.Instance.removeCrewMateFromTaskProgress(this);
        }
    }

    void addDeadBody()
    {

    }

    void stopSabotage()
    {

    }

    public float processByTask()
    {
        return (float)(taskDone)/Game.Instance.Settings.tasks;
    }

    public override bool immobile()
    {
        return doingTask;
    }
}
