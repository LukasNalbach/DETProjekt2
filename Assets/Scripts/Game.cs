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
    public static Game Instance { get; protected set; }
    /// <summary>
    /// The game settings.
    /// </summary>
    public GameSettings Settings { get; protected set; }

    public bool training=false;
    public bool gridVisible=false;
    public bool finished=false;
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
    public List<GameObject> sabotageFires = new List<GameObject>();

    //the prefabs of all weapons

    public System.Random random = new System.Random();

    public float killCooldown{get; set;}

    //so the imposter cannot spam sabortage
    protected float sabortageStartCooldown{get;set;}

    protected float maxSabortageStartCooldown=15f;

    public bool meetingNow=false;
    public bool escMenuOpenend, mapOpened = false;
    private void Awake()
    {
        // there can be only one...
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
        allTasks=new List<Task>();
        allRooms=new List<Room>();
        allVents=new List<Vent>();
        killCooldown=0;
        sabortageStartCooldown=0;
        WorldGenerator wGen = gameObject.AddComponent<WorldGenerator>();

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
        if(gridVisible)
        {
            visualisiereGrid();
        }
    }
    private void visualisiereGrid()
    {
        WorldGenerator wGen = gameObject.GetComponent<WorldGenerator>();
        GameObject redSquarePrefab=AssetDatabase.LoadAssetAtPath("Assets/Prefabs/RedSquare.prefab", typeof(GameObject)) as GameObject;
        GameObject greenSquarePrefab=AssetDatabase.LoadAssetAtPath("Assets/Prefabs/GreenSquare.prefab", typeof(GameObject)) as GameObject;
        for(int x=0; x<wGen.mapGrid.widht;x++)
        {
            for(int y=0;y<wGen.mapGrid.height;y++)
            {
                Vector3 positionWorld=wGen.mapGrid.getWorldPosition(x, y);
                positionWorld.z=1;
                if(!wGen.mapGrid.getValue(x,y))
                {
                    Instantiate(redSquarePrefab, positionWorld , new Quaternion());
                }
                else
                {
                    Instantiate(greenSquarePrefab, positionWorld, new Quaternion());
                }
            }
        }
    }

    private void createCrew()
    {
        if(Settings.numberPlayers==0)
        {
            return;
        }
        GameObject playerPrefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Player.prefab", typeof(GameObject)) as GameObject;
        GameObject playerPrefabKI=AssetDatabase.LoadAssetAtPath("Assets/KI/PlayerKI.prefab", typeof(GameObject)) as GameObject;
        List<Vector2> positions = new List<Vector2>();
        RealGenRoom meetingraum;
        GetComponent<WorldGenerator>().rooms.TryGetValue(RoomType.Meetingraum, out meetingraum);
        List<Rectangle> placedObjectsRects = meetingraum.placedObjects.ConvertAll((GameObject obj) => WorldGenerator.GetRectangleFromTransform(obj.transform));
        Rectangle spawnRect = new Rectangle((int) ((Meetingraum) meetingraum).emergencyButton.transform.position.x - 2, (int) ((Meetingraum) meetingraum).emergencyButton.transform.position.y - 2, 5, 5);

        UnityEngine.Color playerColor = Settings.getPlayerColor();

        List<UnityEngine.Color> remainingColors = new List<UnityEngine.Color>();
        remainingColors.AddRange(Settings.getPossibleColors());
        int activePlayerImposer=0;//1 wenn achtive Player Imposter
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
            startPoint=positions[0];
            GameObject newPlayer=null;
            if(i==0)
            {
                string role=Settings.getPlayerRole();
                activePlayerImposer=role.Equals("IMP")?1:0;
                if(role.Equals("?"))
                {
                    double randomDouble=random.NextDouble();
                    //Debug.Log(randomDouble+" <="+1f*Settings.numberImposters/Settings.numberPlayers+"?");
                    if(randomDouble<=(1f*Settings.numberImposters/Settings.numberPlayers))
                    {
                        activePlayerImposer=1;
                    }
                }
                if(activePlayerImposer==1)
                {
                    newPlayer= Instantiate(playerPrefab, positions[positions.Count - 1], new Quaternion());
                    newPlayer.AddComponent<Imposter>();
                }
                else
                {
                    newPlayer= Instantiate(playerPrefab, positions[positions.Count - 1], new Quaternion());
                    newPlayer.AddComponent<CrewMate>();
                }
            } 
            else if (i >= Settings.numberImposters+1-activePlayerImposer) {
                newPlayer= Instantiate(playerPrefab, positions[positions.Count - 1], new Quaternion());
                 newPlayer.AddComponent<CrewMate>();
            }
            else {
                newPlayer= Instantiate(playerPrefab, positions[positions.Count - 1], new Quaternion());
                newPlayer.AddComponent<Imposter>();
            }
            newPlayer.GetComponent<Player>().startPos = positions[positions.Count - 1];
            int colorIndex;
            if(i==0)
            {
                colorIndex=Settings.getPlayerColorPointer();
            }
            else
            {
                colorIndex = random.Next(remainingColors.Count);
            }
            UnityEngine.Color nextColor = remainingColors[colorIndex];
            remainingColors.RemoveAt(colorIndex);
            newPlayer.GetComponent<Player>().create(nextColor);
            allPlayers.Add(newPlayer.GetComponent<Player>());
        }
        GameObject currentPlayer = allPlayers[0].gameObject;
        Shuffle<Player>(allPlayers,random);
        for(int i=0;i<allPlayers.Count;i++)
        {
            allPlayers[i].giveNumber(i);
        }
        int currentPlayerIndex = currentPlayer.GetComponent<Player>().number;

        /*
        int currentPlayerIndex = -1;
        for (int i = 0; i < allPlayers.Count; i++) {
            if (allPlayers[i].gameObject.GetComponent<Renderer>().material.color.Equals(playerColor)) {
                currentPlayerIndex = i;
                break;
            }
        }
        if (currentPlayerIndex == -1) {
            currentPlayerIndex = random.Next(allPlayers.Count);
            allPlayers[currentPlayerIndex].gameObject.GetComponent<Renderer>().material.SetColor("_Color", playerColor);
        }
        GameObject currentPlayer = allPlayers[currentPlayerIndex].gameObject;
        */
        
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

    protected void addRoom(Room room)
    {
        allRooms.Add(room);
    }

    protected void addTask(Task task)
    {
        allTasks.Add(task);
    }
    protected void setCrewMadesTask()
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
    protected void createVentConnections()
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
        Shuffle<Task>(allTasks,random);
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
    public static void Shuffle<T>(List<T> list, System.Random random)  
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
    public List<Vector2>allCheckpoints()
    {
         WorldGenerator wGen = gameObject.GetComponent<WorldGenerator>();
         if(wGen==null)
         {
             return new List<Vector2>();
         }
         return wGen.checkpoints;
    }
    private void Update() {
        Game.Instance.GUI.setSabotageGui(activeSabortage != null);
        if (Input.GetKeyDown(KeyCode.Escape) && !meetingNow && !mapOpened) {
            if (escMenuOpenend) {
                CloseEscMenu();
            } else {
                OpenEscMenu();
            }
        }
        if (Input.GetKeyDown(KeyCode.M) && !meetingNow && !escMenuOpenend) {
            if (mapOpened) {
                GetComponent<WorldGenerator>().CloseMap();
            } else {
                GetComponent<WorldGenerator>().OpenMap();
            }
            mapOpened = !mapOpened;
        }
    }
    private void FixedUpdate() {
        if(!meetingNow)
        {
             if(killCooldown>0)
            {
                killCooldown -= Time.deltaTime / Settings.cooldownTime;
                GUI.updateKillCooldown(1 - killCooldown);
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
            //Debug.Log("unregistered Level instance", Instance);
        }
    }
    public float getKillCooldown()
    {
        return killCooldown;
    }
    public void resetKillCooldown()
    {
        killCooldown = 1;
        GUI.updateKillCooldown(1 - killCooldown);
    }
    public virtual float getTaskProgress()
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

    public void startSabortageChests()
    {
        if (allSabortages.Count == 0) {
            Game.Instance.startSabortageChests();
            return;
        }
        if (!mapOpened || !sabortagePossible()) {
            return;
        }
        Game.Instance.startSabortage(allSabortages[1]);
    }
    public void startSabortageBournTrees()
    {
        if (allSabortages.Count == 0) {
            Game.Instance.startSabortageBournTrees();
            return;
        }
        if (!mapOpened || !sabortagePossible()) {
            return;
        }
        startSabortage(allSabortages[0]);
        RealGenRoom room;
        GetComponent<WorldGenerator>().rooms.TryGetValue(RoomType.Wald, out room);
        Wald roomForest = (Wald) room;

        Rectangle rectForest = roomForest.innerRect;

        GameObject prefabFire = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/FireBig.prefab", typeof(GameObject)) as GameObject;

        List<Rectangle> fireRects = new List<Rectangle>();

        int n = rectForest.Width * rectForest.Height / 10;
        for (int i = 0; i < n; i++) {
            int j = 0;
            while (j < n) {
                Vector2 pos = new Vector2(rectForest.X + 1 + random.Next(rectForest.Width - 2), rectForest.Y + 1 + random.Next(rectForest.Height - 2));
                if (!VirtualGenRoom.IsCloserToThan(pos, fireRects, "XY", 1)) {

                    GameObject fire = Instantiate(prefabFire, pos, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
                    sabotageFires.Add(fire);
                    float size = 0.6f +  2.4f * (float) random.NextDouble();
                    fire.GetComponent<ParticleSystem>().startSize = size;
                    fireRects.Add(new Rectangle((int) pos.x, (int) pos.y, (int) size, (int) size));

                    break;
                }
                j++;
            }
        }
    }
    public void startSabortage(Sabortage sabortage)
    {
        if(sabortagePossible())
        {
            activeSabortage=sabortage;
            sabortage.activate();
            foreach(Player player in allPlayers)
            {
                if(player.isAlive()&&!player.isImposter())
                {
                    ((CrewMate)player).agent.startSabortage();
                }
            }
        }
        GUI.showMessage("The Sabotage was started", 3);
    }
    public bool sabortagePossible()
    {
        return activeSabortage==null&&sabortageStartCooldown<=0;
    }
    public void stopSabortage()
    {
        if (sabotageFires.Count != 0) {
            foreach (GameObject fire in sabotageFires) {
                Destroy(fire);
            }
        }
        sabotageFires = new List<GameObject>();
        activeSabortage=null;
        sabortageStartCooldown=maxSabortageStartCooldown;
        foreach(Player player in allPlayers)
            {
                if(player.isAlive()&&!player.isImposter())
                {
                    ((CrewMate)player).agent.stopSabortage();
                }
            }
        GUI.showMessage("The Sabotage was stopped", 3);
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
        
        if(meetingNow||finished)
        {
            return;
        }
        if (mapOpened) {
                GetComponent<WorldGenerator>().CloseMap();
                mapOpened = !mapOpened;
        } 
        Debug.Log(initiator.number+" starts meeting");
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

    public void OpenEscMenu() {
        GetComponent<swapPlayer>().currentPlayer.GetComponent<Cainos.PixelArtTopDown_Basic.TopDownCharacterController>().active = false;
        escMenuOpenend = true;
        AsyncOperation op = SceneManager.LoadSceneAsync("EscGUI", LoadSceneMode.Additive);
        StartCoroutine(setEscMenuActive(op));
    }

    private IEnumerator setEscMenuActive(AsyncOperation op) {
        while (!op.isDone) {
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
    public void CloseEscMenu() {
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("EscGUI"));
        Game.Instance.GetComponent<swapPlayer>().currentPlayer.GetComponent<Cainos.PixelArtTopDown_Basic.TopDownCharacterController>().active = true;
        Game.Instance.escMenuOpenend = false;
    }
    public void OpenMainMenu() {
        SceneManager.LoadSceneAsync("StartGui", LoadSceneMode.Single);
    }
    public void QuitGame() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    public IEnumerator meetingResult(int playerToKill)
    {
        GameObject currentPlayer = GetComponent<swapPlayer>().currentPlayer;
        GameObject playerToKillObject;
        Cainos.PixelArtTopDown_Basic.CameraFollow cameraFollow = GameObject.Find("Main Camera").GetComponent<Cainos.PixelArtTopDown_Basic.CameraFollow>();

        if (playerToKill != -1) {
            playerToKillObject = allPlayers[playerToKill].gameObject;
            Game.Instance.GUI.showMessage((allPlayers[playerToKill].isImposter() ? "Imposter " : "Crewmate ") + "Player " + playerToKill + " kicked out", 3);

            RealGenRoom room;
            GetComponent<WorldGenerator>().rooms.TryGetValue(RoomType.Lavagrube, out room);
            Rectangle lavaRect = ((Lavagrube) room).lavaRect;

            playerToKillObject.transform.position = new Vector2(lavaRect.X + lavaRect.Width / 2, lavaRect.Y + lavaRect.Height / 2);
            cameraFollow.target = playerToKillObject.transform;

            yield return new WaitForSeconds(3);

            allPlayers[playerToKill].killAfterMeeting();

            yield return new WaitForSeconds(1);

            if (checkWinningOverPlayers()) {
                yield break;
            } else if (playerToKillObject.Equals(currentPlayer)) {
                activePlayerRespawnsInSameTeam();
        } else {
                cameraFollow.target = currentPlayer.transform;
                cameraFollow.transform.position = currentPlayer.transform.position + cameraFollow.offset;
        }}
        else {
            Game.Instance.GUI.showMessage("Noone was kicked out", 4);
        }
        currentPlayer.GetComponent<Cainos.PixelArtTopDown_Basic.TopDownCharacterController>().active = true;
        GUI.setStandardGui(true);
        meetingNow=false;
    }
    public void activePlayerRespawnsInSameTeam()
    {
        GameObject playerToKillObject=activePlayer().gameObject;
        if (playerToKillObject.GetComponent<Player>() is Imposter) {
                    do {
                        GetComponent<swapPlayer>().next();
                    } while(GetComponent<swapPlayer>().currentPlayer.GetComponent<Player>() is CrewMate);
                    GUI.showMessage("You can now play on as the Imposter Player " + GetComponent<swapPlayer>().currentPlayer.GetComponent<Player>().number, 3);
                } 
        else {
                    do {
                        GetComponent<swapPlayer>().next();
                    } while(GetComponent<swapPlayer>().currentPlayer.GetComponent<Player>() is Imposter);
                    GUI.showMessage("You can now play on as the CrewMate Player " + GetComponent<swapPlayer>().currentPlayer.GetComponent<Player>().number, 3);
                }
             
        
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
    //just use from gui
     public static void accuse(int p2) {
        accuse(Game.Instance.numberActivePlayer(), p2);
    }
     public static void accuse(int p1,int p2) {
         //Debug.Log(p1+" accuses "+p2);
        Game.Instance.gameObject.GetComponent<Voting>().accuse(p1, p2);
    }

    public static void accusePublic(int p2) {
       accusePublic(Game.Instance.numberActivePlayer(), p2);
    }

    public static void accusePublic(int p1,int p2) {
         //Debug.Log(p1+" accuses "+p2+" public");
        Game.Instance.gameObject.GetComponent<Voting>().accusePublic(p1, p2);
        foreach(Player player in Instance.allLivingPlayers())
        {
            player.noticePublicAccuse(p1,p2);
        }
    }
    public static void defendPublic(int p2) {
        defendPublic(Game.Instance.numberActivePlayer(), p2);
    }

    public static void defendPublic(int p1,int p2) {
         //Debug.Log(p1+" defends "+p2+" public");
        Game.Instance.gameObject.GetComponent<Voting>().defendPublic(p1, p2);
        foreach(Player player in Instance.allLivingPlayers())
        {
            player.noticePublicDefend(p1,p2);
        }
    }
    public static void skip() {
        skip(Game.Instance.numberActivePlayer());
    }
    public static void skip(int p1) {
        Game.Instance.gameObject.GetComponent<Voting>().skip(p1);
    }
    public int numberActivePlayer()
    {
        foreach(Player player in allPlayers)
        {
            if(player.activePlayer())
            {
                return player.number;
            }
        }
        return -1;
    }
    public List<Player> allLivingPlayers()
    {
        List<Player>result=new List<Player>();
        foreach(Player player in allPlayers)
        {
            if(player.isAlive())
            {
                result.Add(player);
            }
        }
        return result;
    }
    public int livingCrewMates()
    {
        int crewMates=0;
        foreach(Player player in allPlayers)
        {
            if(player.isAlive())
            {
                if(!player.isImposter())
                {
                    crewMates++;
                }
            }
        }
        return crewMates;
    }
    public int livingImposter()
    {
        int imposters=0;
        foreach(Player player in allPlayers)
        {
            if(player.isAlive())
            {
                if(player.isImposter())
                {
                    imposters++;
                }
            }
        }
        return imposters;
    }
    /*
    checks wheter imposter wins because the number of imposters is equal to the number of crewMates
    checks wheter crew Mates wins because the number of imposters=0
    */
    public bool checkWinningOverPlayers()
    {
        int imposters=livingImposter();
        int crewMates=livingCrewMates();
        if(imposters>=crewMates)
        {
            impostersWin(false);
            return true;
        }
        else if(imposters==0)
        {
            crewMatesWin(false);
            return true;
        }
        return false;
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
        string line="Imposters win";
        if(withSabortage)
        {
            line+=" with Sabotage";
        }
        Game.Instance.GUI.showMessage(line, 3);
        foreach(Player player in allPlayers)
            {
                if(player.isImposter())
                {
                    player.agent.rewardWinGame();
                }
                else
                {
                    player.agent.rewardLooseGame();
                }
            }
             if(training)
            {
                restartAfterTraining();
            }
            else
            {
                StartCoroutine(EndGameIn(4));
            }
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
            Game.Instance.GUI.showMessage(line, 3);
            foreach(Player player in allPlayers)
            {
                if(player.isImposter())
                {
                    player.agent.rewardLooseGame();
                }
                else
                {
                    player.agent.rewardWinGame();
                }
            }
            if(training)
            {
                restartAfterTraining();
            }
            else
            {
                StartCoroutine(EndGameIn(3));
            }      
        }
    }
    public bool fixMap()
    {
        return GetComponent<WorldGenerator>()==null;
    }
    public void restartAfterTraining()
    {
        Debug.Log(fixMap());
    }
    public IEnumerator EndGameIn(int t) {
        finished=true;
        yield return new WaitForSeconds(t);
        OpenMainMenu();
        yield return null;
    }
}
