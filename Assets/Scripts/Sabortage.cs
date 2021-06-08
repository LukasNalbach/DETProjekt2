using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sabortage:MonoBehaviour
{
    public int number;
    public float timeToSolve;
    public float currentTimeToSolve;
    public bool active;
    public List<SabortageTask> tasksToStop;
    void Start()
    {

    }
    public void create(int number,float timeToSolve)
    {
        this.number=number;
        this.timeToSolve=timeToSolve;
        active=false;
        tasksToStop=new List<SabortageTask>();
    }
    
    public void addTask(SabortageTask taskToStop)
    {
        tasksToStop.Add(taskToStop);
    }

    public void activate()
    {
        currentTimeToSolve=timeToSolve;
        active=true;
        foreach(SabortageTask sTask in tasksToStop)
        {
           sTask.solved=false; 
        }

    }
    public void maybeDeactivated()
    {
        foreach(SabortageTask sTask in tasksToStop)
        {
            if(!sTask.solved)
            {
                return;
            }
        }
        deactivate();
    }
    public void deactivate()
    {
        active=false;
        Game.Instance.stopSabortage();
    }
}
