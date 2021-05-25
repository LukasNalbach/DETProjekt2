using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task : MonoBehaviour
{
    //Time to solve one Task(in Seconds)
    public float timeToSolve=1f;

    public bool solvingVisible=true;

    private bool visibleAktivated;
    private int taskNum {get; set;}
    public Room room {get; set;}

    public void CreateTask(int taskNum) {
        this.taskNum = taskNum;
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
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
        }
    }

    public void endSolving()
    {
        if(solvingVisible)
        {
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
        }
    }
    public void setActivated()
    {
        if(!visibleAktivated)
        {
            visibleAktivated=true;
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        }
        
    }
    public void setDeactivated()
    {
        if(visibleAktivated)
        {
            visibleAktivated=false;
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
        }
       
    }

}
