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

    public int  taskDone;

    public int totalTasks;

    public LinkedList<Player>allPlayers;
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
        setRooms();

        SceneManager.LoadScene("IngameGUI", LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("World"));
    }

    private void setRooms() {
        int i = 1;
        while (GameObject.Find("Room" + i + "_1")) {
            Debug.Log(i);
            Room room = gameObject.AddComponent<Room>();
            Task task1 = gameObject.AddComponent<Task>();
            Task task2 = gameObject.AddComponent<Task>();
            task1.CreateTask(1);
            task2.CreateTask(2);
            room.CreateRoom(i, task1, task2);
            task1.room = room;
            task2.room = room;
            i++;
        }
    }

    private void FixedUpdate() {
        if(killCooldown>0)
        {
            killCooldown-=Time.deltaTime;
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
