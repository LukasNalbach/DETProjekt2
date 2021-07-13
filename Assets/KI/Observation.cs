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
        foreach(Task task in Game.Instance.allTasks)
        {
            if(task.isVisibleSolved()&&!task.playerDoingThisTask.Contains(crewMateScript)
            &&Vector3.Distance(task.transform.position, crewMateScript.transform.position)<=Game.Instance.Settings.viewDistance)
            {
                List<Player>possibleTaskSolver=new List<Player>();
                foreach(Player player in Game.Instance.allPlayers)
                {
                    if(player.number!=nr&&player.isAlive()
                    &&Vector3.Distance(task.transform.position,player.transform.position)<=CrewMate.maxDistanceToSolveTask)
                    {
                        possibleTaskSolver.Add(player);
                    }
                }
                if(possibleTaskSolver.Count>0)
                {
                    if(possibleTaskSolver.Count==1)
                    {
                        Debug.Log("CM "+nr+" see Player "+possibleTaskSolver[0].number+" do visible Task alone");
                        crewMateScript.accursation.verdacht[possibleTaskSolver[0].number]=0f;
                    }
                    else
                    {
                        foreach(Player taskSolver in possibleTaskSolver)
                        {
                            float alterVerdacht= crewMateScript.accursation.verdacht[taskSolver.number];
                            crewMateScript.accursation.verdacht[taskSolver.number]-=0.4f/possibleTaskSolver.Count;
                            if(crewMateScript.accursation.verdacht[taskSolver.number]<0)
                            {
                                crewMateScript.accursation.verdacht[taskSolver.number]=0;
                            }
                            Debug.Log("CM "+nr+" see Players "+possibleTaskSolver[0].number+" maybe doing visible Task" );
                            Debug.Log("alter Verdacht: "+alterVerdacht+" neuerVerdacht: "+crewMateScript.accursation.verdacht[taskSolver.number]);
                        }
                    }
                }
            }
        }

    }
}
