using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdateRoom : MonoBehaviour
{
    private Room currentRoom {get; set;}
    // Start is called before the first frame update
    void Start()
    {
        currentRoom = Room.getRoom(2);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Room newRoom = GetNewRoomIfChanged();
        if (newRoom) {
            currentRoom = newRoom;
            GameObject.Find("Canvas/currentRoom").GetComponent<TextMeshProUGUI>().SetText("Room: " + newRoom.getRoomNum());
        }
    }

    private Room GetNewRoomIfChanged() {
        for (int i = 1; i <= Room.getRooms().Count; i++) {
            if (i != currentRoom.getRoomNum()) {
                int j = 1;
                GameObject obj = GameObject.Find("Room" + i + "_" + j);
                while (obj) {
                    if (isInRectangle(gameObject.transform.position, obj.transform.position, obj.transform.localScale)) {
                        return Room.getRoom(i);
                    }
                    
                    j++;

                    obj = GameObject.Find("Room" + i + "_" + j);
                }
            }
        }
        return null;
    }

    public static bool isInRectangle(Vector2 pos, Vector2 rectPos, Vector2 rectScale) {
        return pos.x >= rectPos.x - rectScale.x/2 && pos.x <= rectPos.x + rectScale.x/2 && pos.y >= rectPos.y - rectScale.y/2 && pos.y <= rectPos.y + rectScale.y/2;
    }
}
