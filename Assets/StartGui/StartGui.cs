using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using TMPro;
public class StartGui : UIBehaviour, ICancelHandler
{
    [Tooltip("The name of the scene to load Game started.")]
    public string gameSceneName="World";
    public GameSettings Settings;

    private int pointerPlayerSpeedOptions;

    private int pointerViewDistanceOptions;

    private int pointerKillDistanceOptions;

    private int pointerCooldownTimeOptions;

    private int pointerTaskOptions;


    void Awake()
    {
        Settings = GameSettings.Load();
        DontDestroyOnLoad(Settings);
    }
    void Start()
    {

        Settings.numberPlayers=5;
        Settings.numberImposters=1;

        pointerPlayerSpeedOptions=(Settings.getPlayerSpeedOptions().Length-1)/2;
        Settings.playerSpeed=Settings.getPlayerSpeedOptions()[pointerPlayerSpeedOptions];

        pointerViewDistanceOptions=(Settings.getViewDistanceOptions().Length-1)/2;
        Settings.viewDistance=Settings.getViewDistanceOptions()[pointerViewDistanceOptions];

        pointerKillDistanceOptions=(Settings.getKillDistanceOptions().Length-1)/2;
        Settings.killDistance=Settings.getKillDistanceOptions()[pointerKillDistanceOptions];

        pointerCooldownTimeOptions=(Settings.getCooldownTimeOptions().Length-1)/2;
        Settings.cooldownTime=Settings.getCooldownTimeOptions()[pointerCooldownTimeOptions];

        pointerTaskOptions=(Settings.getTaskOptions().Length-1)/2;
        Settings.tasks=Settings.getTaskOptions()[pointerTaskOptions];

        Settings.setColorPointer(2);

        Settings.setRolePointer(0);

        GameObject.Find("Canvas/Spieleranzahl/Spieleranzahl").GetComponent<TextMeshProUGUI>().SetText("Number of Players: " + Settings.numberPlayers);
        GameObject.Find("Canvas/Imposter/Imposter").GetComponent<TextMeshProUGUI>().SetText("Number of Imposters: " + Settings.numberImposters);
        GameObject.Find("Canvas/Speed/Speed").GetComponent<TextMeshProUGUI>().SetText("Player Speed: x" + Settings.playerSpeed);
        GameObject.Find("Canvas/View/View").GetComponent<TextMeshProUGUI>().SetText("View Distance: " + Settings.viewDistance);
        GameObject.Find("Canvas/KillDistance/KillDistance").GetComponent<TextMeshProUGUI>().SetText("Kill Distance: " + Settings.killDistance);
        GameObject.Find("Canvas/CooldownTime/CooldownTime").GetComponent<TextMeshProUGUI>().SetText("Cooldown Time: " + Settings.cooldownTime);
        GameObject.Find("Canvas/Tasks/Tasks").GetComponent<TextMeshProUGUI>().SetText("Task per Player: " + Settings.tasks);
        setColorMenu();
        GameObject.Find("Canvas/Role/Role").GetComponent<TextMeshProUGUI>().SetText("Role: " + Settings.getPlayerRole());
        
        //GameObject.Find("Canvas/Tasks/Senken").GetComponent<TextMeshProUGUI>().color = Color.red;
    }
    void Update()
    {

    }
     public void OnQuitClicked()
    {
    #if UNITY_EDITOR
        Debug.Log("Quit");
    #endif
    Application.Quit(); // note: this does nothing in the Editor
    }
    public void OnCancel(BaseEventData eventData)
    {
        OnQuitClicked();
    }
    private void setButtonGray(GameObject button)
    {
        //button.clickable.active=false;
    }
    private void resetButton(GameObject button)
    {
        //button.clickable.active=true;
    }
    public void decreasePlayerAmount()
    {
        if(Settings.numberPlayers<=Settings.getMinPlayers())
        {
            return;
        }
        Settings.numberPlayers--;
        GameObject.Find("Canvas/Spieleranzahl/Spieleranzahl").GetComponent<TextMeshProUGUI>().SetText("Number of Players: " + Settings.numberPlayers);
        if(Settings.getMaxImposters(Settings.numberPlayers)<Settings.numberImposters)
        {
            decreaseImposterAmount();
        }
    }
    public void increasePlayerAmount()
    {
        if(Settings.numberPlayers>=Settings.getMaxPlayers())
        {
            return;
        }
        Settings.numberPlayers++;
        GameObject.Find("Canvas/Spieleranzahl/Spieleranzahl").GetComponent<TextMeshProUGUI>().SetText("Number of Players: " + Settings.numberPlayers);
    }
    public void decreaseImposterAmount()
    {
        if(Settings.numberImposters<=1)
        {
            return;
        }
        Settings.numberImposters--;
        GameObject.Find("Canvas/Imposter/Imposter").GetComponent<TextMeshProUGUI>().SetText("Number of Imposters: " + Settings.numberImposters);
    }
    public void increaseImposterAmount()
    {
        if(Settings.numberImposters>=Settings.getMaxImposters(Settings.numberPlayers))
        {
            return;
        }
        Settings.numberImposters++;
        GameObject.Find("Canvas/Imposter/Imposter").GetComponent<TextMeshProUGUI>().SetText("Number of Imposters: " + Settings.numberImposters);
    }
    public void decreasePlayerSpeed()
    {
        if(pointerPlayerSpeedOptions==0)
        {
            return;
        }
        pointerPlayerSpeedOptions--;
        Settings.playerSpeed=Settings.getPlayerSpeedOptions()[pointerPlayerSpeedOptions];
        GameObject.Find("Canvas/Speed/Speed").GetComponent<TextMeshProUGUI>().SetText("Player Speed: x" + Settings.playerSpeed);
    }
    public void increasePlayerSpeed()
    {
        if(pointerPlayerSpeedOptions>=Settings.getPlayerSpeedOptions().Length-1)
        {
            return;
        }
        pointerPlayerSpeedOptions++;
        Settings.playerSpeed=Settings.getPlayerSpeedOptions()[pointerPlayerSpeedOptions];
        GameObject.Find("Canvas/Speed/Speed").GetComponent<TextMeshProUGUI>().SetText("Player Speed: x" + Settings.playerSpeed);
    }
    public void decreaseViewDistance()
    {
        if(pointerViewDistanceOptions==0)
        {
            return;
        }
        pointerViewDistanceOptions--;
        Settings.viewDistance=Settings.getViewDistanceOptions()[pointerViewDistanceOptions];
        GameObject.Find("Canvas/View/View").GetComponent<TextMeshProUGUI>().SetText("View Distance: " + Settings.viewDistance);
    }
    public void increaseViewDistance()
    {
        if(pointerViewDistanceOptions>=Settings.getViewDistanceOptions().Length-1)
        {
            return;
        }
        pointerViewDistanceOptions++;
        Settings.viewDistance=Settings.getViewDistanceOptions()[pointerViewDistanceOptions];
        GameObject.Find("Canvas/View/View").GetComponent<TextMeshProUGUI>().SetText("View Distance: " + Settings.viewDistance);
    }
    public void decreaseKillDistance()
    {
        if(pointerKillDistanceOptions==0)
        {
            return;
        }
        pointerKillDistanceOptions--;
        Settings.killDistance=Settings.getKillDistanceOptions()[pointerKillDistanceOptions];
        GameObject.Find("Canvas/KillDistance/KillDistance").GetComponent<TextMeshProUGUI>().SetText("Kill Distance: " + Settings.killDistance);
    }
    public void increaseKillDistance()
    {
        if(pointerKillDistanceOptions>=Settings.getKillDistanceOptions().Length-1)
        {
            return;
        }
        pointerKillDistanceOptions++;
        Settings.killDistance=Settings.getKillDistanceOptions()[pointerKillDistanceOptions];
        GameObject.Find("Canvas/KillDistance/KillDistance").GetComponent<TextMeshProUGUI>().SetText("Kill Distance: " + Settings.killDistance);
    }
    public void decreaseCooldownTime()
    {
        if(pointerCooldownTimeOptions==0)
        {
            return;
        }
        pointerCooldownTimeOptions--;
        Settings.cooldownTime=Settings.getCooldownTimeOptions()[pointerCooldownTimeOptions];
        GameObject.Find("Canvas/CooldownTime/CooldownTime").GetComponent<TextMeshProUGUI>().SetText("Cooldown Time: " + Settings.cooldownTime);
    }
    public void increaseCooldownTime()
    {
        if(pointerCooldownTimeOptions>=Settings.getCooldownTimeOptions().Length-1)
        {
            return;
        }
        pointerCooldownTimeOptions++;
        Settings.cooldownTime=Settings.getCooldownTimeOptions()[pointerCooldownTimeOptions];
        GameObject.Find("Canvas/CooldownTime/CooldownTime").GetComponent<TextMeshProUGUI>().SetText("Cooldown Time: " + Settings.cooldownTime);
    }
    public void decreaseTasks()
    {
        if(pointerTaskOptions==0)
        {
            return;
        }
        pointerTaskOptions--;
        Settings.tasks=Settings.getTaskOptions()[pointerTaskOptions];
        GameObject.Find("Canvas/Tasks/Tasks").GetComponent<TextMeshProUGUI>().SetText("Task per Player: " + Settings.tasks);
    }
    public void increaseTasks()
    {
        if(pointerTaskOptions>=Settings.getTaskOptions().Length-1)
        {
            return;
        }
        pointerTaskOptions++;
        Settings.tasks=Settings.getTaskOptions()[pointerTaskOptions];
        GameObject.Find("Canvas/Tasks/Tasks").GetComponent<TextMeshProUGUI>().SetText("Task per Player: " + Settings.tasks);
    }
     public void decreaseColor()
    {
        Settings.decreaseColorPointer();
        setColorMenu();
    }
    public void increaseColor()
    {
        Settings.increaseColorPointer();
        setColorMenu();
    }
    public void setColorMenu()
    {
        GameObject.Find("Canvas/Color/Color").GetComponent<TextMeshProUGUI>().color=Settings.getPlayerColor();
        //GameObject.Find("Canvas/Color/Senken/Text").GetComponent<TextMeshProUGUI>().color=Settings.getPreviousColor();
        //GameObject.Find("Canvas/Color/Steigern/Text").GetComponent<TextMeshProUGUI>().color=Settings.getNextColor();
    }
     public void decreaseRole()
    {
        Settings.decreaseRolePointer();
        GameObject.Find("Canvas/Role/Role").GetComponent<TextMeshProUGUI>().SetText("Role: " + Settings.getPlayerRole());
    }
    public void increaseRole()
    {
        Settings.increaseRolePointer();
        GameObject.Find("Canvas/Role/Role").GetComponent<TextMeshProUGUI>().SetText("Role: " + Settings.getPlayerRole());
    }
    public void startGame()
    {
        SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
    }
}

