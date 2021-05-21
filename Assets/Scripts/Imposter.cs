using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Imposter : Player
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void kill(CrewMate crewMate)
    {
        crewMate.getKilledByImposter();
        Game.Instance.resetKillCooldown();
    }

    public override bool immobile()
    {
        return false;
    }
}
