using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task : MonoBehaviour
{
    //Time to solve one Task(in Seconds)
    public float timeToSolve=1f;

    public bool solvingVisible=true;
    public bool sabortageTask=false;
    protected bool activePlayerNear=false;

    public bool activePlayerHasToDoThisTaskAndNear=false;

    public List<Player> playerDoingThisTask=new List<Player>();
    protected int taskNum {get; set;}
    public Room room {get; set;}

    public void CreateTask(int taskNum, float timeToSolve, bool solvingVisible) {
        this.taskNum = taskNum;
        this.timeToSolve=timeToSolve;
        this.solvingVisible=solvingVisible;
    }

    public int getTaskNum() {
        return taskNum;
    }
    public Room getRoom() {
        return room;
    }
    // Start is called before the first frame update

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startSolving(Player player)
    {
        playerDoingThisTask.Add(player);
        setColor();
    }

    public void endSolving(Player player)
    {
        playerDoingThisTask.Remove(player);
        setDeactivated();
    }
    public void setActivated()
    {
        if(!activePlayerHasToDoThisTaskAndNear)
        {
            activePlayerHasToDoThisTaskAndNear=true;
            setColor();
        }
        
    }
    public void setDeactivated()
    {
        if(activePlayerHasToDoThisTaskAndNear)
        {
            activePlayerHasToDoThisTaskAndNear=false;
            setColor();
        }
       
    }
    public void setVisible()
    {
        activePlayerNear=true;
        setColor();
    }
    public void setInvisble()
    {
        activePlayerNear=false;
        setColor();
    }
    protected void setColor()
    {
        gameObject.GetComponent<Renderer>().material.SetColor("_Color",currentNeededColor());
    }
    private Color currentNeededColor()
    {
        if(!activePlayerNear)
        {
            return Color.white;
        }
        if(playerDoingThisTask.Count==0)
        {
            if(activePlayerHasToDoThisTaskAndNear)
            {
                return Color.red;
            }
            else
            {
                return Color.white;
            }
        }
        if(playerDoingThisTask.Contains(Game.Instance.activePlayer()))
        {
            return Color.blue;
        }
        if(solvingVisible)//other player does this task
        {
            if(activePlayerHasToDoThisTaskAndNear)
            {
                return (Color.red + Color.blue) / 2;
            }
            else
            {
                return Color.blue;
            }
        }
        else
        {
           if(activePlayerHasToDoThisTaskAndNear)
            {
                return Color.red ;
            }
            else
            {
                return Color.white;
            } 
        }
    }
}
