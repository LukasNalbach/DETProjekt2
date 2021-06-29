using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
    public Vector2 startPos;
    public new string name;
    protected bool imposter;

    public bool alive = true;

    public bool deadAndInvisible=false;

    public Color color;

    public Room lastRoomBeforeMeeting;
    public UpdateRoom updateRoom;
    public CrewMateAgent agent;
    public float activation=0.8f;//when an imput form the agent is greater than activation, it is activated

    public void create(Color color)
    {
        this.color=color;
        gameObject.GetComponent<Renderer>().material.SetColor("_Color",color);
    }
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    public void Update()
    {

        if (agent.report>=activation)
        {
            Player deadBody=nearDeadBody();
            if(deadBody!=null)
            {
                deadBody.DestroyDeadBody();
                Game.Instance.startEmergencyMeeting(this);
            }
        }
    }


    public void FixedUpdate()
    {
        if(activePlayer())
        {
            foreach (var player in Game.Instance.allPlayers)
            {
                if(player.visible() && (Game.Instance.meetingNow || Vector3.Distance(gameObject.transform.position, player.transform.position)<=Game.Instance.Settings.viewDistance))
                {
                    player.gameObject.GetComponent<Renderer>().enabled=true;
                }
                else
                {
                    player.gameObject.GetComponent<Renderer>().enabled=false;
                }
            }
            foreach (var task in Game.Instance.allTasks)
            {
                if(Vector3.Distance(gameObject.transform.position, task.transform.position)<=Game.Instance.Settings.viewDistance)
                {
                    task.setVisible();
                }
                else
                {
                    task.setInvisble();
                }
            }
           float distance;
            foreach (var sabTask in Game.Instance.allActiveSabortageTasks())
            {
                distance=Vector3.Distance(gameObject.transform.position, sabTask.transform.position);
                if(distance<=Game.Instance.Settings.viewDistance)
                {
                    sabTask.setActivated();
                    sabTask.setVisible();
                }
                else
                {
                    sabTask.setInvisble();
                }
            }
        
        }
        
    }

    public bool isImposter()
    {
        return imposter;
    }
    public bool isAlive()
    {
        return alive;
    }
    protected void becomeGhost()
    {
        
    }
    /*
    returns a dead-Body, that is inside the players vision, or null, if there is no deadBody inside Player vision 
    */
    public Player nearDeadBody()
    {
        foreach (var player in Game.Instance.allPlayers)
        {
            if(!player.isAlive()&&player.visible())
            {
                if(Vector3.Distance(gameObject.transform.position, player.transform.position)<=Game.Instance.Settings.viewDistance)
                {
                    return player;
                }
            }
        }
        return null;
    }

    public bool activePlayer()
    {
        return gameObject==Game.Instance.swapPlayer().currentPlayer;
    }
    
    //just neaded for CrewMates, cause Imposter never have dead Bodies
    public void DestroyDeadBody()
    {
        deadAndInvisible=true;
    }

    public abstract void goToMeeting();
    protected void goToMeetingStandard()
    {
        if(isAlive())
        {
            transform.position=startPos;
            lastRoomBeforeMeeting=updateRoom.getCurrentRoom();
            updateRoom.setCurrentRoom(2);
        }

    }
    public void killAfterMeeting()
    {
        alive=false;
        deadAndInvisible=true;
        if(!imposter)
        {
            Game.Instance.removeCrewMateFromTaskProgress((CrewMate)this);
        }
    }
    public abstract bool immobile();

    public abstract bool visible();
}
