using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    /// <summary>
    /// The singleton instance.
    /// </summary>
    public static Game Instance { get; private set; }
    /// <summary>
    /// The game settings.
    /// </summary>
    public GameSettings Settings { get; private set; }

    public GUI GUI;

    public int  taskDone;

    public int totalTasks;

    public List<Room>allRooms;

    public List<Task>allTasks;

    public List<Player>allPlayers;
    //the prefabs of all weapons

    public System.Random random = new System.Random();

    private float killCooldown{get; set;}
    private void Awake()
    {
        // there can be only one...
        if (Instance)
        {
            Debug.LogError("only one Level instance allowed");
            Destroy(gameObject); // exercise: what would be different if we used Destroy(this) instead?
            return;
        }
        else
        {
            Instance = this;
            Debug.Log("registered Level instance", Instance);
        }
        
        // load settings
        Settings = GameSettings.Load();
        GUI=new GUI();
        allPlayers=new List<Player>();
        allTasks=new List<Task>();
        allRooms=new List<Room>();
        killCooldown=0;
    }
    public void SetTexture(GameObject obj, string name, float scale) {
        Texture2D tex = new Texture2D(500, 500);
        byte[] imageData = System.IO.File.ReadAllBytes("Assets/images/" + name + ".png");
        tex.LoadImage(imageData);
        Sprite spr = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width * scale, tex.height * scale), new Vector2(0.5f, 0.5f), 250.0f);
        obj.GetComponent<SpriteRenderer>().sprite = spr;
    }


    private void Start()
    {
        createCrew();
        setRooms();
        setCrewMadesTask();
        SceneManager.LoadScene("IngameGUI", LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("World"));
    }

    private void createCrew()
    {
        GameObject playerPrefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Player.prefab", typeof(GameObject)) as GameObject;

        for (int i = 0; i < Settings.numberPlayers; i++) {
            Vector3 pos = new Vector3(0, 0, 0);
            GameObject newPlayer = Instantiate(playerPrefab, pos, new Quaternion());
            if (i >= Settings.numberImposters) {
                newPlayer.AddComponent<CrewMate>();
            } else {
                newPlayer.AddComponent<Imposter>();
            }
            allPlayers.Add(newPlayer.GetComponent<Player>());
        }

        GameObject currentPlayer = allPlayers[random.Next(Settings.numberPlayers)].gameObject;
        
        currentPlayer.GetComponent<Cainos.PixelArtTopDown_Basic.TopDownCharacterController>().active = true;
        GameObject.Find("Main Camera").GetComponent<Cainos.PixelArtTopDown_Basic.CameraFollow>().target = currentPlayer.transform;
        
        GetComponent<swapPlayer>().currentPlayer = currentPlayer;
    }

    private void setRooms() {
        int i = 1;
        while (GameObject.Find("Room" + i + "_1")) {
            Room room = gameObject.AddComponent<Room>();
            Task task1 = GameObject.Find("Room" + i + "Task1").AddComponent<Task>();
            Task task2 = GameObject.Find("Room" + i + "Task2").AddComponent<Task>();
            task1.CreateTask(1);
            task2.CreateTask(2);
            room.CreateRoom(i, task1, task2);
            task1.room = room;
            task2.room = room;
            addRoom(room);
            addTask(task1);
            addTask(task2);
            i++;
        }
    }

    private void addRoom(Room room)
    {
        allRooms.Add(room);
    }

    private void addTask(Task task)
    {
        allTasks.Add(task);
    }
    private void setCrewMadesTask()
    {
        foreach(var player in allPlayers)
        {
            if(!player.isImposter())
            {
                CrewMate crew=player.gameObject.GetComponent<CrewMate>();
                addCrewMateTasks(crew);
            }
        }
    }
    private void addCrewMateTasks(CrewMate crewMate)
    {
        Shuffle<Task>(allTasks);
        for(int i=0;i<Settings.tasks;i++)
        {
            if(i>=allTasks.Count)
            {
                Debug.LogError("To many Tasks for crew Mates");
            }
            else
            {
                crewMate.addTask(allTasks[i]);
            }
        }
    }
    public void Shuffle<T>(List<T> list)  
    {  
        int n = list.Count;  
        while (n > 1) {  
            n--;  
            int k = random.Next(n + 1);  
            T value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }  
    }
    private void FixedUpdate() {
        if(killCooldown>0)
        {
            killCooldown-=Time.deltaTime;
            GUI.updateKillCooldown((int)killCooldown);
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            Debug.Log("unregistered Level instance", Instance);
        }
    }
    public float getKillCooldown()
    {
        return killCooldown;
    }
    public void resetKillCooldown()
    {
        killCooldown=Settings.cooldownTime;
        GUI.updateKillCooldown((int)killCooldown);
    }
    public float getTaskProgress()
    {
        return 1.0f*taskDone/totalTasks;
    }
    public void increaseTaskProgress()
    {
        taskDone++;
    }
    public void removeCrewMateFromTaskProgress(CrewMate lostCrewMate)
    {
        totalTasks-=Game.Instance.Settings.tasks;
        taskDone-=lostCrewMate.taskDone;
    }
}
