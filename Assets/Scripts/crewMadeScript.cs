using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class crewMadeScript : MonoBehaviour
{
    public bool alive;

    private List<Task>taskToDo;


    //When a crewMate is doing task, he cannot move
    private bool doingTask;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void doTask(Task task)
    {
        StartCoroutine(coTask(task));
    }

    IEnumerator coTask(Task task)
    {
        task.startSolving();
        doingTask=true;
        yield return new WaitForSeconds(task.timeToSolve);
        doingTask=false;
        task.endSolving();
    }

    void getKilledByImposter()
    {
        alive=false;
        addDeadBody();
    }

    void addDeadBody()
    {
        
    }

    void stopSabotage()
    {

    }

    public float processByTask()
    {
        return (Game.Instance.Settings.tasks-taskToDo.size())/Game.Instance.Settings.tasks;
    }
}
