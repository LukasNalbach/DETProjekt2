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

    [Tooltip("number of imposters")]
    public int numberImposters;
    
    [Tooltip("player speed")]
    public float playerSpeed;

    [Tooltip("how far can a player see")]
    public int viewDistance;

    [Tooltip("what is the minimal time between to imposter kills in secounds")]
    public int cooldownTime;

    [Tooltip("what is the minimal time between to imposter kills in secounds")]
    public int killDistance;

    [Tooltip("Amount of task that one crew member must solve")]
    public int tasks;
    [Tooltip("time of the voting phase in seconds")]
    public int votingTime;
}
