using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrewMatePseudoAgent : MonoBehaviour
{
    public Vector2 movement=new Vector2(0,0);
     public float doingTask=0;
     public float report=0;
     public CrewMate crewMateScript;
     private Pathfinding pathfinding;
    public float time;
    void Awake()
    {
        crewMateScript=GetComponent<CrewMate>();
         pathfinding=new Pathfinding(crewMateScript);
    }
    void FixedUpdate()
    {
        if(crewMateScript.activePlayer())
        {
            movement=new Vector2(0,0);
            movement[0] = Input.GetAxis("Horizontal");
            movement[1] = Input.GetAxis("Vertical");
            doingTask = Input.GetKey(KeyCode.Return)?1f:0;
            report = Input.GetKey(KeyCode.Space)?1f:0;
        }
        else{
            movement=calculateMovement();
            doingTask=1;
            report=1;
        }
    }
    public Vector3 calculateMovement()
    {
        Vector3 nextStep=new Vector3(0,0,0);
        if(pathfinding.hasNextPosition())
        {
            nextStep= pathfinding.getNextPosition();
        }
        else
        {
            pathfinding.calculateNextTaskGoal();
            if(pathfinding.hasNextPosition())
            {
                nextStep= pathfinding.getNextPosition();
            }
            else
            {
                return new Vector3(0,0,0);
            }
        }
        if(Vector3.Distance(crewMateScript.transform.position,nextStep)<=0.5f)
        {
            pathfinding.reachNextPosition();
            if(pathfinding.hasNextPosition())
            {
                nextStep= pathfinding.getNextPosition();
            }
            else
            {
                pathfinding.calculateNextTaskGoal();
                if(pathfinding.hasNextPosition())
                {
                    nextStep= pathfinding.getNextPosition();
                }
                else
                {
                    return new Vector3(0,0,0);
                }
            }
        }
        Vector3 movement=nextStep-crewMateScript.transform.position;
        Debug.Log(movement);
        return movement;
    }
    public void rewardFinishSabortageTask()
    {
        
    }
    public void rewardFinishTask()
    {
        
    }
    public void rewardStartTask()
    {
        
    }
    
    public void rewardWinGame()
    {
        
    }
    public void rewardLooseGame()
    {
        
    }
}
