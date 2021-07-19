using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class TrainigGame : Game
{
    private void Awake()
    {
        if (Instance)
        {
            //Debug.LogError("only one Level instance allowed");
            Destroy(gameObject); // exercise: what would be different if we used Destroy(this) instead?
            return;
        }
        else
        {
            Instance = this;
            //Debug.Log("registered Level instance", Instance);
        }
        
        // load settings
        Settings = GameSettings.Load();
        GUI=gameObject.AddComponent<GUI>();
        GUI.scaleFactor = Screen.width / 1920f;
        allPlayers=new List<Player>();
        allRooms=new List<Room>();
        allVents=new List<Vent>();
        killCooldown=0;
        sabortageStartCooldown=0;
        training=true;
    }
    void Start()
    {
        createTrainingCrew();
        firstCreateTrainingRooms();
        setCrewMadesTask();
        createVentConnections();
    }
    void createTrainingCrew()
    {
        GameObject playerPrefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Player.prefab", typeof(GameObject)) as GameObject;
        GameObject playerPrefabKI=AssetDatabase.LoadAssetAtPath("Assets/KI/PlayerKI.prefab", typeof(GameObject)) as GameObject;
        UnityEngine.Color playerColor = Settings.getPlayerColor();
        List<UnityEngine.Color> remainingColors = new List<UnityEngine.Color>();
        remainingColors.AddRange(Settings.getPossibleColors());
         for (int i = 0; i < Settings.numberPlayers; i++) {
            GameObject newPlayer=null; 
            if (i >= Settings.numberImposters) 
            {
                newPlayer= Instantiate(playerPrefabKI, startPoint, new Quaternion());
                 newPlayer.AddComponent<CrewMate>();
            } 
            else 
            {
                newPlayer= Instantiate(playerPrefab, startPoint, new Quaternion());
                newPlayer.AddComponent<Imposter>();
            }
            newPlayer.GetComponent<Player>().startPos = startPoint;
            
            int colorIndex = random.Next(remainingColors.Count);
            UnityEngine.Color nextColor = remainingColors[colorIndex];
            remainingColors.RemoveAt(colorIndex);
            newPlayer.GetComponent<Player>().create(nextColor);
            newPlayer.GetComponent<Player>().giveNumber(i);

            allPlayers.Add(newPlayer.GetComponent<Player>());
         }
        GameObject currentPlayer = allPlayers[0].gameObject;
         currentPlayer.GetComponent<Cainos.PixelArtTopDown_Basic.TopDownCharacterController>().active = true;
        GetComponent<swapPlayer>().currentPlayer = currentPlayer;
        GetComponent<swapPlayer>().currentPlayerIndex = 0;
    }
    private void firstCreateTrainingRooms() {
        int i = 0;

        while (i <= 5) {
            Room room = gameObject.AddComponent<Room>();
            
            if(i==2)
            {
                Task t21=GameObject.Find("Room" + i + "Task1").AddComponent<Task>();
                room.CreateRoom(i,t21);
                Task t22=GameObject.Find("Room" + i + "Task2").AddComponent<Task>();
                Task t23=GameObject.Find("Room" + i + "Task3").AddComponent<Task>();
                room.addTask(t22);
                room.addTask(t23);
                addTask(t21);
                addTask(t22);
                addTask(t23);
            }
            else
            {
                room.CreateRoom(i,null);
            }
            addRoom(room);
            i++;
        }
    }
     private void createTrainingRooms() {
        int i = 0;
        while (i < 5) {
            Room room = gameObject.AddComponent<Room>();
            Task task = GameObject.Find("Room" + i + "Task1").AddComponent<Task>();
            task.CreateTask(1,1f,true);
            room.CreateRoom(i, task);
            task.room = room;
            addRoom(room);
            addTask(task);
            for(int j=2;j<99;j++)
            {
                 Task nextTask = GameObject.Find("Room" + i + "Task"+j).AddComponent<Task>();
                 if(nextTask==null)
                 {
                     break;
                 }
                nextTask.CreateTask(1,1f,true);
                room.addTask(nextTask);
                task.room = room;
                addTask(task);
            }
            i++;
        }
    }
    
}
