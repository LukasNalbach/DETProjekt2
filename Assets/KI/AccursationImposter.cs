using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccursationImposter:AccursationCrew
{
    public int playerToAccuse;
    public int wrongAccusionStrength;//0: no accuasition this round, 1: wrong accusation from crewMate,
                                    //2: wrong accusation from other imposter, 3: wrong accusation from this player
    
    public AccursationImposter(Player playerScript):base(playerScript)
    {
        playerToAccuse=-1;
        wrongAccusionStrength=0;
    }
    public override void accusePublic()
    {
        
    }
    public override void accuse()
    {
        if(playerToAccuse!=-1)
        {
            accuse(nr,playerToAccuse);
        }
        else
        {
            Game.skip(nr);
        }
        wrongAccusionStrength=0;
    }
    public override void noticePublicAccuse(int initiator,int reciver)
    {
        if(reciver==nr)
        {
            accusePublic(nr,initiator);
            playerToAccuse=initiator;
            wrongAccusionStrength=3;
        }
        else if(!Game.Instance.allPlayers[reciver].isImposter())
        {
            int strengthThisAccusation=1;
            if(Game.Instance.allPlayers[initiator].isImposter())
            {
                strengthThisAccusation=2;
            }
            if(strengthThisAccusation>wrongAccusionStrength)
            {
                playerToAccuse=reciver;
                wrongAccusionStrength=strengthThisAccusation;
            }
        }
    }
    public override void noticePublicDefend(int initiator,int reciver)
    {
        
    }
    

}
