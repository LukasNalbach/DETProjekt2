using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
    public string name;
    protected bool imposter;

    public bool alive = true;

    public Color color;

    
    public UpdateRoom updateRoom;

    public void create(String name, Color color)
    {
        this.name=name;
        this.color=color;
    }
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    public void Update()
    {

        if (Input.GetKey(KeyCode.Space)&&activePlayer())
        {
            if(nearDeadBody())
            {
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
                if(Vector3.Distance(gameObject.transform.position, player.transform.position)<=Game.Instance.Settings.viewDistance)
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

    public bool nearDeadBody()
    {
        foreach (var player in Game.Instance.allPlayers)
        {
            if(!player.isAlive())
            {
                if(Vector3.Distance(gameObject.transform.position, player.transform.position)<=Game.Instance.Settings.viewDistance)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool activePlayer()
    {
        return gameObject==Game.Instance.swapPlayer().currentPlayer;
    }

    public abstract bool immobile();
}
