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

    private float[] playerSpeedOptions={1.0f,1.5f,2.0f,2.5f,3.0f};

    [Tooltip("how far can a player see")]
    public int viewDistance;

    private int[] viewDistanceOptions={2, 4, 5, 6, 8};

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
   
}
