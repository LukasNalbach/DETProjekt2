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
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1) && !Game.Instance.GetComponent<Voting>()) {
            if (currentPlayerIndex+1 >= Game.Instance.allPlayers.Count) {
                currentPlayerIndex = 0;
            } else {
                currentPlayerIndex++;
            }
            currentPlayer.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
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
        }
        Game.Instance.GUI.setImposterGui(currentPlayer.GetComponent<Player>().isImposter());
    }
}
