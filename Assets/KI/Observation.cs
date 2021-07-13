using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observation 
{
    private CrewMate crewMateScript;
    private int nr;
    public Observation(CrewMate crewMateScript)
    {
        this.crewMateScript=crewMateScript;
        nr=crewMateScript.number;
    }
    //called 2 times für every secound
    public void observeVisibleTasks()
    {
        /*
        foreach(Task task in Game.Instance.allTasks())
        {
            if(task.solvingVisible&&Vector3.Distance(task.transform.position, crewMateScript.transform.position)<=Game.Instance.Settings.viewDistance)
            {

            }
        }*/
    }
}
