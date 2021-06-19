using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public GameObject buttonPrefab;
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

    //so the imposter cannot spam sabortage
    private float sabortageStartCooldown{get;set;}

    private float maxSabortageStartCooldown=15f;

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
        sabortageStartCooldown=0;
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
        WorldGenerator wGen = gameObject.AddComponent<WorldGenerator>();
        
        createCrew();
        setRooms();
        setCrewMadesTask();
        createVentConnections();
        createSabortageOptions();
        AsyncOperation op = SceneManager.LoadSceneAsync("IngameGUI", LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("World"));
    }

    private void createCrew()
    {
        if(Settings.numberPlayers==0)
        {
            return;
        }
        GameObject playerPrefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Player.prefab", typeof(GameObject)) as GameObject;
        List<Vector2> positions = new List<Vector2>();
        RealGenRoom meetingraum;
        GetComponent<WorldGenerator>().rooms.TryGetValue(RoomType.Meetingraum, out meetingraum);
        List<Rectangle> placedObjectsRects = meetingraum.placedObjects.ConvertAll((GameObject obj) => WorldGenerator.GetRectangleFromTransform(obj.transform));
        Rectangle spawnRect = new Rectangle((int) ((Meetingraum) meetingraum).emergencyButton.transform.position.x - 2, (int) ((Meetingraum) meetingraum).emergencyButton.transform.position.y - 2, 5, 5);

        for (int i = 0; i < Settings.numberPlayers; i++) {
            while (positions.Count == i) {
                Vector2 pos = new Vector2(spawnRect.X + random.Next(spawnRect.Width), spawnRect.Y + random.Next(spawnRect.Height));
                if (
                    !VirtualGenRoom.IsCloserToThan(pos, positions, "XY", 0) &&
                    !VirtualGenRoom.IsCloserToThan(pos, placedObjectsRects, "XY", 0)
                ) {
                    positions.Add(pos);
                }
            }
            GameObject newPlayer = Instantiate(playerPrefab, positions[positions.Count - 1], new Quaternion());
            if (i >= Settings.numberImposters) {
                newPlayer.AddComponent<CrewMate>();
            } else {
                newPlayer.AddComponent<Imposter>();
            }
            newPlayer.GetComponent<Player>().startPos = positions[positions.Count - 1];
            UnityEngine.Color nextColor=Settings.getPossibleColors()[(Settings.getPlayerColorPointer()+i)%Settings.getPossibleColors().Length];
            newPlayer.GetComponent<Player>().create(nextColor);
            allPlayers.Add(newPlayer.GetComponent<Player>());
        }

        int currentPlayerIndex = random.Next(Settings.numberPlayers);
        GameObject currentPlayer = allPlayers[currentPlayerIndex].gameObject;
        
        currentPlayer.GetComponent<Cainos.PixelArtTopDown_Basic.TopDownCharacterController>().active = true;
        GameObject.Find("Main Camera").GetComponent<Cainos.PixelArtTopDown_Basic.CameraFollow>().target = currentPlayer.transform;
        
        GetComponent<swapPlayer>().currentPlayer = currentPlayer;
        GetComponent<swapPlayer>().currentPlayerIndex = currentPlayerIndex;

        Cainos.PixelArtTopDown_Basic.CameraFollow cameraFollow = GameObject.Find("Main Camera").GetComponent<Cainos.PixelArtTopDown_Basic.CameraFollow>();
        cameraFollow.target = currentPlayer.transform;
        cameraFollow.transform.position = currentPlayer.transform.position + cameraFollow.offset;
    }

    private void setRooms() {
        int i = 0;
        while (i < 10) {
            Room room = gameObject.AddComponent<Room>();
            Task task = GameObject.Find("Room" + i + "Task").AddComponent<Task>();
            task.CreateTask(1,1f,true);
            room.CreateRoom(i, task);
            task.room = room;
            addRoom(room);
            addTask(task);
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
        Sabortage sabortage;
        SabortageTask sTask1, sTask2;
        GameObject stopTask1, stopTask2;

        sabortage=gameObject.AddComponent<Sabortage>();
        sabortage.create(1,45f);
        stopTask1=GameObject.Find("Stopsabortage11");
        sTask1=stopTask1.AddComponent<SabortageTask>();
        sTask1.createSabortageTask(1, 1f,sabortage);
        stopTask2=GameObject.Find("Stopsabortage12");
        sTask2=stopTask2.AddComponent<SabortageTask>();
        sTask2.createSabortageTask(2, 1f,sabortage);
        sabortage.addTask(sTask1);
        sabortage.addTask(sTask2);

        allSabortages.Add(sabortage);

        sabortage=gameObject.AddComponent<Sabortage>();
        sabortage.create(1,45f);
        stopTask1=GameObject.Find("Stopsabortage21");
        sTask1=stopTask1.AddComponent<SabortageTask>();
        sTask1.createSabortageTask(1, 1f,sabortage);
        stopTask2=GameObject.Find("Stopsabortage22");
        sTask2=stopTask2.AddComponent<SabortageTask>();
        sTask2.createSabortageTask(2, 1f,sabortage);
        sabortage.addTask(sTask1);
        sabortage.addTask(sTask2);

        allSabortages.Add(sabortage);
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

    private void Update() {
        Game.Instance.GUI.setSabotageGui(activeSabortage != null);
    }
    private void FixedUpdate() {
        if(!meetingNow)
        {
             if(killCooldown>0)
            {
                killCooldown-=Time.deltaTime;
                GUI.updateKillCooldown(1 - killCooldown / Settings.cooldownTime);
            }
            if(sabortageStartCooldown>0)
            {
                sabortageStartCooldown-=Time.deltaTime;
            }
            if(activeSabortage!=null)
            {
                activeSabortage.currentTimeToSolve-=Time.deltaTime;
                if(activeSabortage.currentTimeToSolve<=0f)
                {
                    impostersWin(true);
                }
                else
                {
                    GUI.updateSabortageCountdown(activeSabortage.currentTimeToSolve / activeSabortage.timeToSolve);
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
        GUI.updateKillCooldown(1);
    }
    public float getTaskProgress()
    {
        float progress=1.0f*taskDone/totalTasks;
        float visibleProgress=0f;
        if(progress>=0.25f)
        {
            visibleProgress=0.25f;
        }
        if(progress>=0.5f)
        {
            visibleProgress=0.5f;
        }
        if(progress>=0.75f)
        {
            visibleProgress=0.75f;
        }
        if(taskDone==totalTasks)
        {
            visibleProgress=1f;
            crewMatesWin(true);
        }
        return visibleProgress;
    }
    public void increaseTaskProgress()
    {
        taskDone++;
        GUI.updateTaskProgress(getTaskProgress());
    }
    public void removeCrewMateFromTaskProgress(CrewMate lostCrewMate)
    {
        totalTasks-=Game.Instance.Settings.tasks;
        taskDone-=lostCrewMate.taskDone;
        GUI.updateTaskProgress(getTaskProgress());
    }
    public static void startSabortageBournTrees()
    {
        Game.Instance.startSabortage(Game.Instance.allSabortages[0]);
    }
    public void startSabortage(Sabortage sabortage)
    {
        if(sabortagePossible())
        {
            activeSabortage=sabortage;
            sabortage.activate();
        }
    }
    public bool sabortagePossible()
    {
        return activeSabortage==null&&sabortageStartCooldown<=0;
    }
    public void stopSabortage()
    {
        activeSabortage=null;
        sabortageStartCooldown=maxSabortageStartCooldown;
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
        checkWinningOverPlayers();
    }
    public void endMeeting()
    {
        meetingNow=false;
    }
    //return the only human player in the end
    public Player activePlayer()
    {
        return swapPlayer().currentPlayer.GetComponent<Player>();
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
    /*
    checks wheter imposter wins because the number of imposters is equal to the number of crewMates
    checks wheter crew Mates wins because the number of imposters=0
    */
    public void checkWinningOverPlayers()
    {
        int imposters=0;
        int crewMates=0;
        foreach(Player player in allPlayers)
        {
            if(player.isAlive())
            {
                if(player.isImposter())
                {
                    imposters++;
                }
                else
                {
                    crewMates++;
                }
            }
        }
        if(imposters>=crewMates)
        {
            impostersWin(false);
        }
        else if(imposters==0)
        {
            crewMatesWin(false);
        }
    }
    public bool impostersWin()
    {
        int imposters=0;
        int crewMates=0;
        foreach(Player player in allPlayers)
        {
            if(player.isAlive())
            {
                if(player.isImposter())
                {
                    imposters++;
                }
                else
                {
                    crewMates++;
                }
            }
        }
        if(imposters>=crewMates)
        {
            return true;
        }
        return false;
    }
    public void impostersWin(bool withSabortage)
    {
        string line="Imposters wins";
        if(withSabortage)
        {
            line+=" with Sabortage";
        }
        Debug.Log(line);
    }
    public void crewMatesWin(bool withTasks)
    {
        if(impostersWin())
        {
            impostersWin(false);
        }
        else
        {
            string line="Crew wins";
            if(withTasks)
            {
                line+=" with Tasks";
            }
            Debug.Log(line);
        }
        
    }
}
