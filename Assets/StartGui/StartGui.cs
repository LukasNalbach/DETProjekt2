using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Drawing;
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


    public new void Awake()
    {
        Settings = GameSettings.Load();
        DontDestroyOnLoad(Settings);
        WorldGenerator worldGenerator = gameObject.AddComponent<WorldGenerator>();
        StartCoroutine(cameraMovement(worldGenerator.worldArea));
    }

    private IEnumerator cameraMovement(Rectangle worldArea) {
        System.Random random = new System.Random();
        GameObject camera = GameObject.Find("MainCamera");
        Vector2 cameraOffset = new Vector2(camera.GetComponent<Camera>().pixelWidth / 2, camera.GetComponent<Camera>().pixelHeight / 2);

        camera.transform.position = new Vector2(worldArea.X + random.Next(worldArea.Width), worldArea.Y + random.Next(worldArea.Height));
        float movementAngle = (float) random.NextDouble() * 360;

        yield return new WaitForEndOfFrame();
        while (true) {
            Vector2 newPos = camera.transform.position + new Vector3(Mathf.Cos(movementAngle), Mathf.Sin(movementAngle), 0) * Time.deltaTime;

            if (!WorldGenerator.IsPosInRectangle(newPos, worldArea)) {
                if (newPos.x < worldArea.X) { // left of worldArea
                    movementAngle = (float) random.NextDouble() * 180f;
                } else if (newPos.y < worldArea.Y) { // right of worldArea
                    movementAngle = 180f + (float) random.NextDouble() * 180f;
                } else if (newPos.x > worldArea.X + worldArea.Width) { // above of worldArea
                    movementAngle = 90f + (float) random.NextDouble() * 180f;
                } else { // below worldArea
                    movementAngle = (270f + (float) random.NextDouble() * 180f) % 360f;
                }
            } else {
                camera.transform.position = newPos;
                yield return new WaitForEndOfFrame();
            }
        }
    }
    public new void Start()
    {
        switchToMainMenu();
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void switchToMainMenu() {
        GUI.setActiveRecursive(GameObject.Find("Canvas/Options"), false);
        GUI.setActiveRecursive(GameObject.Find("Canvas/MainMenu"), true);
        GameObject.Find("Canvas/Options").transform.SetSiblingIndex(1);
        GameObject.Find("Canvas/MainMenu").transform.SetSiblingIndex(0);
    }

    public void switchToSettings() {
        GUI.setActiveRecursive(GameObject.Find("Canvas/Options"), true);
        GUI.setActiveRecursive(GameObject.Find("Canvas/MainMenu"), false);
        GameObject.Find("Canvas/Options").transform.SetSiblingIndex(0);
        GameObject.Find("Canvas/MainMenu").transform.SetSiblingIndex(1);

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

        GameObject.Find("Canvas/Options/panelNumberPlayers/valueNumberPlayers").GetComponent<TextMeshProUGUI>().SetText(Settings.numberPlayers.ToString());
        GameObject.Find("Canvas/Options/panelNumberImposters/valueNumberImposters").GetComponent<TextMeshProUGUI>().SetText(Settings.numberImposters.ToString());
        GameObject.Find("Canvas/Options/panelPlayerSpeed/valuePlayerSpeed").GetComponent<TextMeshProUGUI>().SetText(Settings.playerSpeed.ToString());
        GameObject.Find("Canvas/Options/panelViewDistance/valueViewDistance").GetComponent<TextMeshProUGUI>().SetText(Settings.viewDistance.ToString());
        //GameObject.Find("Canvas/KillDistance/KillDistance").GetComponent<TextMeshProUGUI>().SetText("Kill Distance: " + Settings.killDistance);
        GameObject.Find("Canvas/Options/panelCooldownTime/valueCooldownTime").GetComponent<TextMeshProUGUI>().SetText(Settings.cooldownTime.ToString());
        GameObject.Find("Canvas/Options/panelTasksPerPlayer/valueTasksPerPlayer").GetComponent<TextMeshProUGUI>().SetText(Settings.tasks.ToString());
        setColorMenu();
        GameObject.Find("Canvas/Options/panelPlayerRole/valuePlayerRole").GetComponent<TextMeshProUGUI>().SetText(Settings.getPlayerRole());
        
        //GameObject.Find("Canvas/Tasks/Senken").GetComponent<TextMeshProUGUI>().color = Color.red;
    }
    void Update()
    {

    }
     public void OnQuitClicked()
    {
    #if UNITY_EDITOR
        //Debug.Log("Quit");
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
        GameObject.Find("Canvas/Options/panelNumberPlayers/valueNumberPlayers").GetComponent<TextMeshProUGUI>().SetText(Settings.numberPlayers.ToString());
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
        GameObject.Find("Canvas/Options/panelNumberPlayers/valueNumberPlayers").GetComponent<TextMeshProUGUI>().SetText(Settings.numberPlayers.ToString());
    }
    public void decreaseImposterAmount()
    {
        if(Settings.numberImposters<=1)
        {
            return;
        }
        Settings.numberImposters--;
        GameObject.Find("Canvas/Options/panelNumberImposters/valueNumberImposters").GetComponent<TextMeshProUGUI>().SetText(Settings.numberImposters.ToString());
    }
    public void increaseImposterAmount()
    {
        if(Settings.numberImposters>=Settings.getMaxImposters(Settings.numberPlayers))
        {
            return;
        }
        Settings.numberImposters++;
        GameObject.Find("Canvas/Options/panelNumberImposters/valueNumberImposters").GetComponent<TextMeshProUGUI>().SetText(Settings.numberImposters.ToString());
    }
    public void decreasePlayerSpeed()
    {
        if(pointerPlayerSpeedOptions==0)
        {
            return;
        }
        pointerPlayerSpeedOptions--;
        Settings.playerSpeed=Settings.getPlayerSpeedOptions()[pointerPlayerSpeedOptions];
        GameObject.Find("Canvas/Options/panelPlayerSpeed/valuePlayerSpeed").GetComponent<TextMeshProUGUI>().SetText(Settings.playerSpeed.ToString());
    }
    public void increasePlayerSpeed()
    {
        if(pointerPlayerSpeedOptions>=Settings.getPlayerSpeedOptions().Length-1)
        {
            return;
        }
        pointerPlayerSpeedOptions++;
        Settings.playerSpeed=Settings.getPlayerSpeedOptions()[pointerPlayerSpeedOptions];
        GameObject.Find("Canvas/Options/panelPlayerSpeed/valuePlayerSpeed").GetComponent<TextMeshProUGUI>().SetText(Settings.playerSpeed.ToString());
    }
    public void decreaseViewDistance()
    {
        if(pointerViewDistanceOptions==0)
        {
            return;
        }
        pointerViewDistanceOptions--;
        Settings.viewDistance=Settings.getViewDistanceOptions()[pointerViewDistanceOptions];
        GameObject.Find("Canvas/Options/panelViewDistance/valueViewDistance").GetComponent<TextMeshProUGUI>().SetText(Settings.viewDistance.ToString());
    }
    public void increaseViewDistance()
    {
        if(pointerViewDistanceOptions>=Settings.getViewDistanceOptions().Length-1)
        {
            return;
        }
        pointerViewDistanceOptions++;
        Settings.viewDistance=Settings.getViewDistanceOptions()[pointerViewDistanceOptions];
        GameObject.Find("Canvas/Options/panelViewDistance/valueViewDistance").GetComponent<TextMeshProUGUI>().SetText(Settings.viewDistance.ToString());
    }
    /*
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
    */
    public void decreaseCooldownTime()
    {
        if(pointerCooldownTimeOptions==0)
        {
            return;
        }
        pointerCooldownTimeOptions--;
        Settings.cooldownTime=Settings.getCooldownTimeOptions()[pointerCooldownTimeOptions];
        GameObject.Find("Canvas/Options/panelCooldownTime/valueCooldownTime").GetComponent<TextMeshProUGUI>().SetText(Settings.cooldownTime.ToString());
    }
    
    public void increaseCooldownTime()
    {
        if(pointerCooldownTimeOptions>=Settings.getCooldownTimeOptions().Length-1)
        {
            return;
        }
        pointerCooldownTimeOptions++;
        Settings.cooldownTime=Settings.getCooldownTimeOptions()[pointerCooldownTimeOptions];
        GameObject.Find("Canvas/Options/panelCooldownTime/valueCooldownTime").GetComponent<TextMeshProUGUI>().SetText(Settings.cooldownTime.ToString());
    }
    public void decreaseTasks()
    {
        if(pointerTaskOptions==0)
        {
            return;
        }
        pointerTaskOptions--;
        Settings.tasks=Settings.getTaskOptions()[pointerTaskOptions];
        GameObject.Find("Canvas/Options/panelTasksPerPlayer/valueTasksPerPlayer").GetComponent<TextMeshProUGUI>().SetText(Settings.tasks.ToString());
    }
    public void increaseTasks()
    {
        if(pointerTaskOptions>=Settings.getTaskOptions().Length-1)
        {
            return;
        }
        pointerTaskOptions++;
        Settings.tasks=Settings.getTaskOptions()[pointerTaskOptions];
        GameObject.Find("Canvas/Options/panelTasksPerPlayer/valueTasksPerPlayer").GetComponent<TextMeshProUGUI>().SetText(Settings.tasks.ToString());
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
        GameObject.Find("Canvas/Options/panelPlayerColor/valuePlayerColor").GetComponent<UnityEngine.UI.Image>().color=Settings.getPlayerColor();
        //GameObject.Find("Canvas/Color/Senken/Text").GetComponent<TextMeshProUGUI>().color=Settings.getPreviousColor();
        //GameObject.Find("Canvas/Color/Steigern/Text").GetComponent<TextMeshProUGUI>().color=Settings.getNextColor();
    }
     public void decreaseRole()
    {
        Settings.decreaseRolePointer();
        GameObject.Find("Canvas/Options/panelPlayerRole/valuePlayerRole").GetComponent<TextMeshProUGUI>().SetText(Settings.getPlayerRole());
    }
    public void increaseRole()
    {
        Settings.increaseRolePointer();
        GameObject.Find("Canvas/Options/panelPlayerRole/valuePlayerRole").GetComponent<TextMeshProUGUI>().SetText(Settings.getPlayerRole());
    }
    public void startGame()
    {
        SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
    }
}

