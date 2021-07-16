using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observation 
{
    private CrewMate crewMateScript;
    private int nr;
    private List<Task> taskToIgnore=new List<Task>();//task, die fuer paar Sekunden ignoriert werden, da ihre Bearbeitung schon regrestriert wurde
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
            if(task.isVisibleSolved()&&!task.playerDoingThisTask.Contains(crewMateScript)&&!taskToIgnore.Contains(task)
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
                            Debug.Log("CM "+nr+" see Players "+taskSolver.number+" maybe doing visible Task" );
                            Debug.Log("alter Verdacht: "+alterVerdacht+" neuerVerdacht: "+crewMateScript.accursation.verdacht[taskSolver.number]);
                        }
                    }
                    crewMateScript.StartCoroutine(coIgnoreTask(task, taskToIgnore));
                }
            }
        }

    }
    IEnumerator coIgnoreTask(Task task, List<Task> taskToIgnore)
    {
        taskToIgnore.Add(task);
        yield return new WaitForSeconds(1f);
        taskToIgnore.Remove(task);
    }
    public void seeVenting(Player venter)
    {
        Debug.Log("CM "+nr+" see Player "+venter.number+" vent");
        crewMateScript.accursation.verdacht[venter.number]=1f;
    }
    public void maybeSeeKill(Player killed)
    {
        float distanceKill=Vector3.Distance(killed.transform.position, crewMateScript.transform.position);
         if(distanceKill<=Game.Instance.Settings.viewDistance)
         {
            bool seeAllInKillDistance=distanceKill+Game.Instance.Settings.killDistance<=Game.Instance.Settings.viewDistance;
            List<Player> possibleMurderer=new List<Player>();
            foreach(Player player in crewMateScript.playerInViewDistance)
            {
                if(player.isAlive()&&Vector3.Distance(killed.transform.position, player.transform.position)<=Game.Instance.Settings.killDistance
                &&crewMateScript.accursation.verdacht[player.number]>0)
                {
                    possibleMurderer.Add(player);
                }
            }
            if(possibleMurderer.Count==0)
            {
                Debug.Log("CM "+crewMateScript.number+" does not see murderer");
            }
            if(possibleMurderer.Count==1)
            {
                if(seeAllInKillDistance)
                {
                    Debug.Log("CM "+crewMateScript.number+" see "+possibleMurderer[0].number+" murdere");
                    crewMateScript.accursation.verdacht[possibleMurderer[0].number]=1f;
                }
                else
                {
                    Debug.Log("CM "+crewMateScript.number+" see "+possibleMurderer[0].number+" murdere, but does not see all");
                    crewMateScript.accursation.verdacht[possibleMurderer[0].number]+=0.5f;
                }
            }
            else
            {
                foreach(Player player in possibleMurderer)
                {
                    Debug.Log("CM "+crewMateScript.number+" see "+player.number+" maybe murdere, increase Verdacht by "+1f/possibleMurderer.Count);
                    crewMateScript.accursation.verdacht[player.number]+=1f/possibleMurderer.Count;
                }
            }
         }  
    }
}
