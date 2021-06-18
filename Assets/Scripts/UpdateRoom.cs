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
        currentRoom = Room.getRoom(0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        KeyValuePair<int,string> place = Game.Instance.GetComponent<WorldGenerator>().GetPlaceFromPos(gameObject.transform.position);
        if (place.Key != - 1) {
            if (gameObject.Equals(Game.Instance.GetComponent<swapPlayer>().currentPlayer)) {
                Game.Instance.GUI.updateRoom(place.Value);
            }
            if (!place.Value.StartsWith("Korridor")) {
                currentRoom = Room.getRoom(place.Key);
            }
        }
    }
    public Room getCurrentRoom()
    {
        return currentRoom;
    }
    //for emergencie Meeting
    public void setCurrentRoom(int roomNr)
    {
        Room.getRoom(roomNr);
    }

    public static bool isInRectangle(Vector2 pos, Vector2 rectPos, Vector2 rectScale) {
        return pos.x >= rectPos.x - rectScale.x/2 && pos.x <= rectPos.x + rectScale.x/2 && pos.y >= rectPos.y - rectScale.y/2 && pos.y <= rectPos.y + rectScale.y/2;
    }
}
