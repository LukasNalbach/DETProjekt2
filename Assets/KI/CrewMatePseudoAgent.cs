using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrewMatePseudoAgent : MonoBehaviour
{
    public Vector2 movement=new Vector2(0,0);
     public float doingTask=0;
     public float report=0;
     public CrewMate crewMateScript;
    public float time;
     void Start()
     {
         crewMateScript=GetComponent<CrewMate>();
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
            movement=new Vector2(0,0);
            doingTask=1;
            report=1;
        }
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
