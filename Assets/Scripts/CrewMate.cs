using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrewMate : Player
{
    public bool ki=false;

    public List<Task>taskToDo=new List<Task>();

    public int taskDone;
    public AccursationCrew accursation; 

    public Observation observation;
    //Task the crew mate is currently solving(or null).When a crewMate is doing task, he cannot move
    private Task activeTask;

    private IEnumerator taskCoroutine;

    private bool immobileCauseVotingEtc=false;

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
        if(Input.GetKeyDown(KeyCode.P))
        {
            Grid<bool> worldGrid=Game.Instance.GetComponent<WorldGenerator>().mapGrid;
            int[] gridPosition=worldGrid.getXY(transform.position);
            Debug.Log("Position: "+transform.position+" , Grid Position "+worldGrid.getWorldPosition(gridPosition[0], gridPosition[1]));
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
        observation.observeVisibleTasks();
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
            if(taskToDo.Contains(task)&&Vector3.Distance(gameObject.transform.position, task.transform.position)<=maxDistanceToSolveTask)
            {
                return true;
            }
        }
        foreach (var sabTask in Game.Instance.allActiveSabortageTasks())
        {
            if(Vector3.Distance(gameObject.transform.position, sabTask.transform.position)<=maxDistanceToSolveTask)
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
    public IEnumerator coWaiting(float time)
    {
        immobileCauseVotingEtc=true;
        yield return new WaitForSeconds(time);
        immobileCauseVotingEtc=false;
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
        if(activePlayer()&&!Game.Instance.finished)
        {
           StartCoroutine(changePlayerAfterDeath(5));
        }
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
    public IEnumerator changePlayerAfterDeath(int t) {
        yield return new WaitForSeconds(t);
        Game.Instance.activePlayerRespawnsInSameTeam();
        yield return null;
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
    public void seeVenting(Player venter)
    {
        observation.seeVenting(venter);
        ((CrewMatePseudoAgent)agent).reactionAfterVent();
    }

    public override bool immobile()
    {
        return doingTask()||!isAlive()||immobileCauseVotingEtc||Game.Instance.meetingNow||Game.Instance.escMenuOpenend;
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
        agent.wantsMeetingNow=false;
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
    public override void feedbackMeeting(int playerToKill)
    {
        accursation.feedbackMeeting(playerToKill);
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
