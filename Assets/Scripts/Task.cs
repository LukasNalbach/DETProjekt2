using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task : MonoBehaviour
{
    //Time to solve one Task(in Seconds)
    public float timeToSolve;

    public bool solvingVisible;

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

    }

    public void endSolving()
    {

    }
}
