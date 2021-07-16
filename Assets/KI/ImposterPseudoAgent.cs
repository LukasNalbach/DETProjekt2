using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImposterPseudoAgent : PseudoAgent
{
    // Start is called before the first frame update
    public Imposter imposterScript;
    public float kill;
    public float vent;
    public float changeVent;
    void Start()
    {
        imposterScript=GetComponent<Imposter>();
    }

    // Update is called once per frame
    void Update()
    {
        doingTask=0;
        if(imposterScript.activePlayer())
        {
            movement=new Vector2(0,0);
            movement[0] = Input.GetAxis("Horizontal");
            movement[1] = Input.GetAxis("Vertical");
            report = Input.GetKeyDown(KeyCode.Space)?1f:0;
            kill = Input.GetKeyDown(KeyCode.Return)?1f:0;
            vent = Input.GetKeyDown(KeyCode.V)?1f:0;
            changeVent=Input.GetKeyDown(KeyCode.Tab)?1f:0;
        }
        else
        {
            movement=calculateMovement();
            report=0;
            kill=calculateKill();
            vent=calculateVent();
            changeVent=calculateChangeVent();
        }
    }
    void FixedUpdate()
    {
        
    }
    Vector2 calculateMovement()
    {
        return new Vector2(0,0);
    }
    float calculateKill()
    {
        return 0f;
    }
    float calculateVent()
    {
        return 0f;
    }
    float calculateChangeVent()
    {
        return 0f;
    }
}
