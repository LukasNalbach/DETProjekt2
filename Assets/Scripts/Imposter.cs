using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Imposter : Player
{
    // Start is called before the first frame update
    void Start()
    {
        updateRoom=GetComponent<UpdateRoom>();
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
    }

    void Awake() {
        imposter=true;
    }

    // Update is called once per frame
    public void Update()
    {
        if (activePlayer()&&Input.GetKey(KeyCode.Return))
        {
            Debug.Log("Imposter wonts to Kill");
            if(canKill())
            {
                Debug.Log("Killing possible");
                Player playerToKill=null;
                float nearesPlayerDistance=Mathf.Infinity;
                float distance;
                foreach (var player in Game.Instance.allPlayers)
                {
                    if(!player.isImposter()&&player.isAlive())
                    {
                        distance=Vector3.Distance(gameObject.transform.position, player.transform.position);
                        if(distance<=nearesPlayerDistance)
                        {
                            playerToKill=player;
                            nearesPlayerDistance=distance;
                        }
                    }
                }
                kill((CrewMate)playerToKill);
            }
        }
        base.Update();
    }
    public void FixedUpdate()
    {
        base.FixedUpdate();
    }
    public bool nearOtherPlayer()
    {
        foreach (var player in Game.Instance.allPlayers)
        {
            if(!player.isImposter()&&player.isAlive())
            {
                if(Vector3.Distance(gameObject.transform.position, player.transform.position)<=Game.Instance.Settings.killDistance)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public bool canKill()
    {
        bool result=Game.Instance.getKillCooldown()<=0;
        if(!result)
        {
            Debug.Log("Kill Cooldown active");
            return result;
        }
        result=nearOtherPlayer();
        if(!result)
        {
            Debug.Log("No Player to kill near imposter");
        }
        return result;
    }
    void kill(CrewMate crewMate)
    {
        Debug.Log("Imposter kills CrewMate");
        crewMate.getKilledByImposter();
        Game.Instance.resetKillCooldown();
    }

    public override bool immobile()
    {
        return false;
    }
}
