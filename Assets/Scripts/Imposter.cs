using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Imposter : Player
{
    // Start is called before the first frame update
    void Start()
    {
        updateRoom=GetComponent<UpdateRoom>();
    }

    void Awake() {
        imposter=true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool nearOtherPlayer()
    {
        foreach (var player in Game.Instance.allPlayers)
        {
            if(!player.isImposter()&&player.isAlive())
            {
                return Vector3.Distance(gameObject.transform.position, player.transform.position)<=Game.Instance.Settings.killDistance;
            }
        }
        return false;
    }
    public bool canKill()
    {
        return Game.Instance.getKillCooldown()==0 &&nearOtherPlayer();
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
