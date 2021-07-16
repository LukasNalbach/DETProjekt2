using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImposterPseudoAgent : PseudoAgent
{
    // Start is called before the first frame update
    public Imposter imposterScript;
    void Start()
    {
        imposterScript=GetComponent<Imposter>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void FixedUpdate()
    {
        if(imposterScript.activePlayer())
        {
            movement=new Vector2(0,0);
            movement[0] = Input.GetAxis("Horizontal");
            movement[1] = Input.GetAxis("Vertical");
            doingTask = Input.GetKey(KeyCode.Return)?1f:0;
            report = Input.GetKey(KeyCode.Space)?1f:0;
        }
    }
}
