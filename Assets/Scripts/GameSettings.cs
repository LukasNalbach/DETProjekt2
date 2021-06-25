using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The game's settings.
/// </summary>
[CreateAssetMenu(fileName = "Settings", menuName = "Game Settings")]
public class GameSettings : ScriptableObject
{
    /// <summary>
    /// Loads the game settings from the Resources directory.
    /// </summary>
    /// <returns></returns>
    public static GameSettings Load() => Resources.Load<GameSettings>("Settings");

    [Header("Map")]
    [Tooltip("The map's side length")]
    public int size;
    
    [Header("Gameplay")]
    [Tooltip("number of persons")]
    public int numberPlayers;

    private int minPlayers=4;

    private int maxPlayers=10;

    [Tooltip("number of imposters")]
    public int numberImposters;
    
    [Tooltip("player speed")]
    public float playerSpeed;

    private float[] playerSpeedOptions={2.0f,3.0f,4.0f,5.0f};

    [Tooltip("how far can a player see")]
    public int viewDistance;

    private int[] viewDistanceOptions={3, 4, 5, 6, 8};

    [Tooltip("what is the minimal time between to imposter kills in secounds")]
    public int killDistance;
    
    private int[] killDistanceOptions={1, 2 , 3, 4};

    [Tooltip("what is the minimal time between to imposter kills in secounds")]
    public int cooldownTime;
    
    private int[] cooldownTimeOptions={15, 30 , 45, 60};

    

    [Tooltip("Amount of task that one crew member must solve")]
    public int tasks;
    
    [Tooltip("time of the voting phase in seconds")]
    public int votingTime;
    
    private int[] taskOptions={ 2 , 3, 4, 5, 6};

    private int playerColorPointer;

    private Color[] possiblePlayerColors={Color.yellow, (Color.yellow+Color.red)/2, Color.red, (Color.red+Color.blue)/2, 
    Color.blue, Color.green, (Color.white+Color.black)/2, /*dark gray*/new Color32(40,79,79,255) ,/*brown*/new Color32(160,82,45,255), /*pink*/new Color32(255,105,180,255)};
    
    private int playerRolePointer;

    private string[] possibleRoles={"?", "IMP", "CM"};

    public int getMinPlayers()
    {
        return minPlayers;
    }
    public int getMaxPlayers()
    {
        return maxPlayers;
    }
    public int getMaxImposters(int playerAmount)
    {
        if(playerAmount<6)
        {
            return 1;
        }
        if(playerAmount<8)
        {
            return 2;
        }
        return 3;
    }
    public float[] getPlayerSpeedOptions()
    {
        return playerSpeedOptions;
    }
    public int[] getViewDistanceOptions()
    {
        return viewDistanceOptions;
    }
    public int[] getKillDistanceOptions()
    {
        return killDistanceOptions;
    }
    public int[] getCooldownTimeOptions()
    {
        return cooldownTimeOptions;
    }
    public int[] getTaskOptions()
    {
        return taskOptions;
    }
    public Color[] getPossibleColors()
    {
        return possiblePlayerColors;
    }
    public int getPlayerColorPointer()
    {
        return playerColorPointer;
    }
    public void setColorPointer(int newNr)
    {
        playerColorPointer=newNr;
    }
    public void increaseColorPointer()
    {
        playerColorPointer=(playerColorPointer+1)%possiblePlayerColors.Length;
    }
    public void decreaseColorPointer()
    {
        playerColorPointer=(playerColorPointer-1);
        if(playerColorPointer<0)
        {
            playerColorPointer+=possiblePlayerColors.Length;
        }
    }
    public Color getPlayerColor()
    {
        return possiblePlayerColors[playerColorPointer];
    }
    public Color getPreviousColor()
    {
        int color=(playerColorPointer-1);
        if(color<0)
        {
            color+=possiblePlayerColors.Length;
        }
        return possiblePlayerColors[color];
    }
    public Color getNextColor()
    {
        int color=(playerColorPointer+1)%possiblePlayerColors.Length;
        return possiblePlayerColors[color];
    }
    public void setRolePointer(int newNr)
    {
        playerRolePointer=newNr;
    }
    public void increaseRolePointer()
    {
        playerRolePointer=(playerRolePointer+1)%possibleRoles.Length;
    }
    public void decreaseRolePointer()
    {
        playerRolePointer=(playerRolePointer-1);
        if(playerRolePointer<0)
        {
            playerRolePointer+=possibleRoles.Length;
        }
    }
    public string getPlayerRole()
    {
        return possibleRoles[playerRolePointer];
    }
   
}
