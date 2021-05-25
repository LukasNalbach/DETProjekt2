using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
    protected bool imposter;

    public bool alive = true;

    public Color color;
    
    public UpdateRoom updateRoom;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public abstract bool immobile();
}
