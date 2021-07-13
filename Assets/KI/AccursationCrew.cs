using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccursationCrew 
{
     public float[] verdacht;//fuer jeden Spieler der aktuelle Verdacht, imposer zu sein(0-1)
     public float[] temporerVerdachtDurchPublicAccursation;//wird geloescht, falls beschuldiger selbst imposter ist(und das in diesem Meeting herausgefunden wird)
     public float minSchwellePublicAccursation=0.5f;
     public float minSchwelleAccurcation;
    public CrewMate crewMateScript;
    public static float standardVerdacht=0f;//set in initialise
    public int nr;//=crewMate number
    
    public AccursationCrew(CrewMate crewMateScript)
    {
        this.crewMateScript=crewMateScript;
        nr=crewMateScript.number;
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
    public void accusePublic()
    {
        for(int i=0;i<verdacht.Length;i++)
        {
            if(Game.Instance.allPlayers[i].isAlive())
            {
                if(verdacht[i]>= minSchwellePublicAccursation)
                {
                    Game.accusePublic(nr,i);
                }
                if(verdacht[i]==0f&&i!=nr)
                {
                    Game.defendPublic(nr,i);
                }
            }
            

        }
    }
    public void accuse()
    {
        float greatestVerdacht=0f;
        int numberAcursedPlayer=-1;
        for(int i=0;i<verdacht.Length;i++)
        {
            if(i!=nr&&Game.Instance.allPlayers[i].isAlive())
            {
                float tempVerdacht=verdacht[i]+temporerVerdachtDurchPublicAccursation[i];
                Debug.Log("Verdacht CM "+nr+" fuer "+i+": "+tempVerdacht);
                if(tempVerdacht>=greatestVerdacht)
                {
                    greatestVerdacht=tempVerdacht;
                    numberAcursedPlayer=i;
                }
            }
        }
        Debug.Log("numberAcursedPlayer:"+numberAcursedPlayer+", greatestVerdacht: "+greatestVerdacht+", minSchwelleAccurcation"+minSchwelleAccurcation);
        if(numberAcursedPlayer==-1)
        {
            Game.skip(nr);
        }
        else if(greatestVerdacht>=minSchwelleAccurcation)
        {
            Debug.Log("Accuse"+nr+","+numberAcursedPlayer);
            Game.accuse(nr,numberAcursedPlayer);
        }
        else if(2*Game.Instance.livingCrewMates()-Game.Instance.livingImposter()<=1)
        {
            Game.accuse(nr,numberAcursedPlayer);
        }
        else
        {
            Game.skip(nr);
        }
    }
    public void noticePublicAccuse(int initiator,int reciver)
    {
        if(verdacht[initiator]<=minSchwelleAccurcation)
        {
             temporerVerdachtDurchPublicAccursation[reciver]+=0.5f;
        }
    }
    public void noticePublicDefend(int initiator,int reciver)
    {
        if(verdacht[initiator]<=minSchwelleAccurcation&&verdacht[reciver]<=minSchwelleAccurcation)
        {
            verdacht[reciver]/=2;
        }
    }
}
