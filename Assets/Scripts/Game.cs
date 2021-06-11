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

    public Vector3 startPoint=new Vector3(0,0,0);

    public int  taskDone;

    public int totalTasks;

    public List<Room>allRooms;

    public List<Task>allTasks;

    public List<Player>allPlayers;

    public List<Vent>allVents;

    public List<Sabortage>allSabortages;

    //never more than one
    public Sabortage activeSabortage;

    //the prefabs of all weapons

    public System.Random random = new System.Random();

    private float killCooldown{get; set;}

    private bool meetingNow=false;
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
        allVents=new List<Vent>();
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
        createVentConnections();
        createSabortageOptions();
        SceneManager.LoadScene("IngameGUI", LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("World"));
        GUI.updateTaskProgress(0);
        GUI.stopSabortageCountdown();

        gameObject.AddComponent<WorldGenerator>();
    }

    private void createCrew()
    {
        GameObject playerPrefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Player.prefab", typeof(GameObject)) as GameObject;

        for (int i = 0; i < Settings.numberPlayers; i++) {
            Vector3 pos = startPoint;
            GameObject newPlayer = Instantiate(playerPrefab, pos, new Quaternion());
            if (i >= Settings.numberImposters) {
                newPlayer.AddComponent<CrewMate>();
            } else {
                newPlayer.AddComponent<Imposter>();
            }
            Color nextColor=Settings.getPossibleColors()[(Settings.getPlayerColorPointer()+i)%Settings.getPossibleColors().Length];
            newPlayer.GetComponent<Player>().create(nextColor);
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
            task1.CreateTask(1,1f,true);
            task2.CreateTask(2,1f,true);
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
    private void createVentConnections()
    {
        int i=1;
        while (true) {
            GameObject vent=GameObject.Find("Vent" + i );
            if(vent==null)
            {
                break;
            }
            Vent ventScript=vent.AddComponent<Vent>();
            allVents.Add(ventScript);
            i++;
        }
        allVents[0].matchedVent=allVents[3];
        allVents[1].matchedVent=allVents[2];
        allVents[2].matchedVent=allVents[1];
        allVents[3].matchedVent=allVents[0];

    }
    private void createSabortageOptions()
    {
        Sabortage sabortage1=gameObject.AddComponent<Sabortage>();
        sabortage1.create(1,45f);
        GameObject stopTask1=GameObject.Find("StopSabortage11");
        SabortageTask sTask1=stopTask1.AddComponent<SabortageTask>();
        sTask1.createSabortageTask(1, 1f,sabortage1);
        GameObject stopTask2=GameObject.Find("StopSabortage12");
        SabortageTask sTask2=stopTask2.AddComponent<SabortageTask>();
        sTask2.createSabortageTask(2, 1f,sabortage1);
        sabortage1.addTask(sTask1);
        sabortage1.addTask(sTask2);
        allSabortages.Add(sabortage1);
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
                totalTasks++;
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
        if(!meetingNow)
        {
             if(killCooldown>0)
            {
                killCooldown-=Time.deltaTime;
                GUI.updateKillCooldown((int)killCooldown);
            }
            if(activeSabortage!=null)
            {
                activeSabortage.currentTimeToSolve-=Time.deltaTime;
                if(activeSabortage.currentTimeToSolve<=0f)
                {
                    Debug.Log("Imposer wins with Sabortage");
                }
                else
                {
                    GUI.updateSabortageCountdown((int)activeSabortage.currentTimeToSolve);
                }
            }
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
        GUI.updateTaskProgress((int)(getTaskProgress()*100));
    }
    public void removeCrewMateFromTaskProgress(CrewMate lostCrewMate)
    {
        totalTasks-=Game.Instance.Settings.tasks;
        taskDone-=lostCrewMate.taskDone;
        GUI.updateTaskProgress((int)(getTaskProgress()*100));
    }
    public void startSabortage(Sabortage sabortage)
    {
        if(activeSabortage==null)
        {
            activeSabortage=sabortage;
            sabortage.activate();
        }
        
    }
    public void stopSabortage()
    {
        activeSabortage=null;
        GUI.stopSabortageCountdown();
    }
    public List<SabortageTask> allActiveSabortageTasks()
    {
        List<SabortageTask>result=new List<SabortageTask>();
        if(activeSabortage!=null)
        {
            foreach(SabortageTask sTask in activeSabortage.tasksToStop)
            {
                if(!sTask.solved)
                {
                    result.Add(sTask);
                }
            }
        }
        return result;
    }
    public void startEmergencyMeeting(Player initiator)
    {
        meetingNow=true;
        if(activeSabortage)
        {
            activeSabortage.currentTimeToSolve=activeSabortage.timeToSolve;
        }
        foreach(var player in allPlayers)
        {
            if(player.isAlive())
            {
                player.goToMeeting();
            }
        }
        gameObject.AddComponent<Voting>();
    }
    public void meetingResult(int playerToKill)
    {
        allPlayers[playerToKill].killAfterMeeting();
    }
    public void endMeeting()
    {
        meetingNow=false;
    }
    public swapPlayer swapPlayer()
    {
        return gameObject.GetComponent<swapPlayer>();
    }
     public static void accuse(int p2) {
        Game.Instance.gameObject.GetComponent<Voting>().accuse(-1, p2);
    }

    public static void accusePublic(int p2) {
        Game.Instance.gameObject.GetComponent<Voting>().accusePublic(-1, p2);
    }

    public static void defendPublic(int p2) {
        Game.Instance.gameObject.GetComponent<Voting>().defendPublic(-1, p2);
    }

    public static void skip() {
        Game.Instance.gameObject.GetComponent<Voting>().skip(-1);
    }
}
