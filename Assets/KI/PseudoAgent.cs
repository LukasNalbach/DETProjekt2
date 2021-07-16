using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PseudoAgent : MonoBehaviour
{
     public Vector2 movement=new Vector2(0,0);
     public float doingTask=0;
     public float report=0;
    public float time=0f;
    public virtual void startSabortage()
    {
        
    }
    public virtual void stopSabortage()
    {
        
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
