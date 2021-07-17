using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swapPlayer : MonoBehaviour
{
    public GameObject currentPlayer;
    public int currentPlayerIndex;
    // Start is called before the first frame update
    void Start()
    {
        Game.Instance.GUI.setImposterGui(currentPlayer.GetComponent<Player>().isImposter());
        Game.Instance.GUI.setSelectSabotageGui(false);
    }

    // Update is called once per frame
    void Update() {}

    private void next() {
        do {
            if (currentPlayerIndex+1 >= Game.Instance.allPlayers.Count) {
                currentPlayerIndex = 0;
            } else {
                currentPlayerIndex++;
            }
        } while (!Game.Instance.allPlayers[currentPlayerIndex].isAlive());
        swap();
    }

    public void swapTo(int i) {
        currentPlayerIndex = i;
        swap();
    }

    public void swap() {
        currentPlayer.GetComponent<Cainos.PixelArtTopDown_Basic.TopDownCharacterController>().active = false;

        currentPlayer = Game.Instance.allPlayers[currentPlayerIndex].gameObject;

        currentPlayer.GetComponent<Cainos.PixelArtTopDown_Basic.TopDownCharacterController>().active = true;
        Cainos.PixelArtTopDown_Basic.CameraFollow cameraFollow = GameObject.Find("Main Camera").GetComponent<Cainos.PixelArtTopDown_Basic.CameraFollow>();
        cameraFollow.target = currentPlayer.transform;
        cameraFollow.transform.position = currentPlayer.transform.position + cameraFollow.offset;

        foreach(Task task in Game.Instance.allTasks)
        {
            task.setDeactivated();
        }

        Game.Instance.GUI.setImposterGui(currentPlayer.GetComponent<Player>().isImposter());
    }
}
