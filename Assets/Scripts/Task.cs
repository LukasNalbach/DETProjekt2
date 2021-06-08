using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task : MonoBehaviour
{
    //Time to solve one Task(in Seconds)
    public float timeToSolve=1f;

    public bool solvingVisible=true;
    public bool sabortageTask=false;
    protected bool visibleAktivated;
    protected int taskNum {get; set;}
    public Room room {get; set;}
    public Color currentColor=Color.white;

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

    public void startSolving()
    {
        if(solvingVisible)
        {
            currentColor= Color.blue;
        }
    }

    public void endSolving()
    {
        if(solvingVisible)
        {
            currentColor=  Color.white;
        }
    }
    public void setActivated()
    {
        if(!visibleAktivated)
        {
            visibleAktivated=true;
            currentColor= Color.red;
        }
        
    }
    public void setDeactivated()
    {
        if(visibleAktivated)
        {
            visibleAktivated=false;
            currentColor= Color.white;
        }
       
    }
    public void setVisible()
    {
        gameObject.GetComponent<Renderer>().material.SetColor("_Color",currentColor);
    }
    public void setInvisble()
    {
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
    }

}
