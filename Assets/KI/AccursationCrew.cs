using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccursationCrew 
{
     public float[] verdacht;//fuer jeden Spieler der aktuelle Verdacht, imposer zu sein(0-1)
     public float[] temporerVerdachtDurchPublicAccursation;//wird geloescht, falls beschuldiger selbst imposter ist(und das in diesem Meeting herausgefunden wird)
     public float minSchwellePublicAccursation=0.5f;
     public float minSchwelleAccurcation;
    public Player playerScript;
    public static float standardVerdacht=0f;//set in initialise
    public int nr;//=crewMate number
    
    public AccursationCrew(Player playerScript)
    {
        this.playerScript=playerScript;
        nr=playerScript.number;
        initialisiereVerdacht();
    }
    private void initialisiereVerdacht()
    {
        verdacht=new float[Game.Instance.allPlayers.Count];
        float standardStartVerdacht=(float)Game.Instance.Settings.numberImposters;
        float quotient=(float)Game.Instance.Settings.numberPlayers-1;
        if(quotient==0f)
        {
            quotient=1f;
        }
        standardStartVerdacht/=quotient;
        for(int i=0;i<verdacht.Length;i++)
        {
            if(i==nr)
            {
                verdacht[i]=0f;
            }
            else
            {
                verdacht[i]=standardStartVerdacht;
            }
        }
        standardVerdacht=standardStartVerdacht;
        minSchwelleAccurcation=standardStartVerdacht*1.2f;
        temporerVerdachtDurchPublicAccursation=new float[Game.Instance.allPlayers.Count];
    }
    public virtual void accusePublic()
    {
        for(int i=0;i<verdacht.Length;i++)
        {
            if(Game.Instance.allPlayers[i].isAlive())
            {
                if(verdacht[i]>= minSchwellePublicAccursation)
                {
                    accusePublic(nr,i);
                }
                if(verdacht[i]==0f&&i!=nr)
                {
                    defendPublic(nr,i);
                }
            }
        }
    }
    public virtual void accuse()
    {
        float greatestVerdacht=0f;
        int numberAcursedPlayer=-1;
        int amountPlayersWithSameVerdacht=1;
        for(int i=0;i<verdacht.Length;i++)
        {
            if(i!=nr&&Game.Instance.allPlayers[i].isAlive())
            {
                float tempVerdacht=verdacht[i]+temporerVerdachtDurchPublicAccursation[i];
                //Debug.Log("Verdacht CM "+nr+" fuer "+i+": "+tempVerdacht);
                if(Mathf.Abs(tempVerdacht-greatestVerdacht)<=0.05f)
                {
                    amountPlayersWithSameVerdacht++;
                }
                else if(tempVerdacht>greatestVerdacht)
                {
                    greatestVerdacht=tempVerdacht;
                    numberAcursedPlayer=i;
                    amountPlayersWithSameVerdacht=1;
                }
            }
        }
        if(amountPlayersWithSameVerdacht>1)
        {
            List<int>numbersSameVerdacht=new List<int>();
            for(int i=0;i<verdacht.Length;i++)
            {
                float tempVerdacht=verdacht[i]+temporerVerdachtDurchPublicAccursation[i];
                if(Mathf.Abs(tempVerdacht-greatestVerdacht)<=0.05f)
                {
                    numbersSameVerdacht.Add(i);
                }
            }
            int random=Game.Instance.random.Next(numbersSameVerdacht.Count);
            //Debug.Log(random+" ,"+ numbersSameVerdacht.Count);
            numberAcursedPlayer=numbersSameVerdacht[random];
        }
        //Debug.Log("numberAcursedPlayer:"+numberAcursedPlayer+", greatestVerdacht: "+greatestVerdacht+", minSchwelleAccurcation"+minSchwelleAccurcation);
        if(numberAcursedPlayer==-1)
        {
            Game.skip(nr);
        }
        else if(greatestVerdacht>=minSchwelleAccurcation)
        {
            accuse(nr,numberAcursedPlayer);
        }
        else if(2*Game.Instance.livingCrewMates()-Game.Instance.livingImposter()<=1)
        {
            accuse(nr,numberAcursedPlayer);
        }
        else
        {
            Game.skip(nr);
        }
    }
    public virtual void noticePublicAccuse(int initiator,int reciver)
    {
        if(reciver==nr)
        {
            accusePublic(nr,initiator);
        }
        else if(verdacht[initiator]<=minSchwelleAccurcation)
        {
             temporerVerdachtDurchPublicAccursation[reciver]+=0.5f;
        }
    }
    public virtual void noticePublicDefend(int initiator,int reciver)
    {
        if(verdacht[initiator]<=minSchwelleAccurcation*1.5&&verdacht[reciver]<=minSchwelleAccurcation)
        {
            verdacht[reciver]/=2;
        }
    }
    public void accuse(int initiator, int reciver)
    {
        if(playerScript.activePlayer()||!playerScript.isAlive())
        {
            return;
        }
        Game.accuse(initiator,reciver);
    }
    public void accusePublic(int initiator, int reciver)
    {
        if(playerScript.activePlayer()||!playerScript.isAlive())
        {
            return;
        }
        if(!Game.Instance.GetComponent<Voting>().hasAccusatedPublic(initiator, reciver))
        {
            Game.accusePublic(initiator, reciver);
        }
    }
    public void defendPublic(int initiator, int reciver)
    {
        if(playerScript.activePlayer()||!playerScript.isAlive())
        {
            return;
        }
        if(!Game.Instance.GetComponent<Voting>().hasDefendsPublic(initiator, reciver))
        {
            Game.defendPublic(initiator, reciver);
        }
    }
}
