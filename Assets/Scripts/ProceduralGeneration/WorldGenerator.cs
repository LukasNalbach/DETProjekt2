using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
public class WorldGenerator : MonoBehaviour
{
    private System.Random random;
    public void Awake() {
        random = new System.Random();

        List<RealGenRoom> roomsInside = new List<RealGenRoom>(6);
        List<RealGenRoom> roomsOutside = new List<RealGenRoom>(4);

        roomsInside.Add(new Meetingraum());
        roomsInside.Add(new Opferstaette());
        roomsInside.Add(new Schatzkammer());
        roomsInside.Add(new Statuenraum());
        roomsInside.Add(new Thronsaal());

        roomsOutside.Add(new Hafen());
        roomsOutside.Add(new Grabstaette());
        roomsOutside.Add(new Wald());
        roomsOutside.Add(new Lavagrube());

        Shuffle<RealGenRoom>(roomsInside);
        Shuffle<RealGenRoom>(roomsOutside);

        GenRoom rootInside = new GenRoom();
        GenRoom rootOutside = new GenRoom();
        while (roomsInside.Count != 0) {
            int randomIndex = random.Next(roomsInside.Count);
            rootInside.addSubroom(roomsInside[randomIndex]);
            roomsInside.RemoveAt(randomIndex);
        }
        while (roomsOutside.Count != 0) {
            int randomIndex = random.Next(roomsOutside.Count);
            rootOutside.addSubroom(roomsOutside[randomIndex]);
            roomsOutside.RemoveAt(randomIndex);
        }
        GenRoom root = GenRoom.joinRooms(rootInside, rootOutside);

        Rectangle worldArea = new Rectangle(0, 0, 600, 400);
        root.generate(worldArea);
    }

    private void Shuffle<T>(IList<T> list)
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
}