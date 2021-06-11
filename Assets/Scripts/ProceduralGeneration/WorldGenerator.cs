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

        roomsOutside.Add(new Grabstaette());
        roomsOutside.Add(new Hafen());
        roomsOutside.Add(new Wald());
        roomsOutside.Add(new Lavagrube());
        roomsOutside.Add(new Zeltlager());

        Shuffle<RealGenRoom>(roomsInside);
        Shuffle<RealGenRoom>(roomsOutside);

        GenRoom rootInside = roomsInside[0];
        GenRoom rootOutside = roomsOutside[0];
        roomsInside.RemoveAt(0);
        roomsOutside.RemoveAt(0);

        while (roomsInside.Count != 0) {
            int randomIndex = random.Next(roomsInside.Count);
            rootInside = rootInside.addSubroom(roomsInside[randomIndex]);
            roomsInside.RemoveAt(randomIndex);
        }
        while (roomsOutside.Count != 0) {
            int randomIndex = random.Next(roomsOutside.Count);
            rootOutside = rootOutside.addSubroom(roomsOutside[randomIndex]);
            roomsOutside.RemoveAt(randomIndex);
        }

        VirtualGenRoom root = GenRoom.joinRooms(rootInside, rootOutside);

        foreach (GenRoom room in root.getSubrooms()) {
            room.setRandom(random);
        }

        Rectangle worldArea = new Rectangle(50, 50, 44 + random.Next(6), 44 + random.Next(6));
        root.generate(worldArea);

        List<Rectangle> corridors = new List<Rectangle>();

        foreach (GenRoom room in root.getSubrooms()) {
            if (room is RealGenRoom) {
                Rectangle rect = ((RealGenRoom) room).innerRect;
                GenerateRectangle(rect.X, rect.Y, rect.Width, rect.Height);

                Rectangle rect2 = ((RealGenRoom) room).outerRect;
                GenerateRectangleBackground(rect2.X, rect2.Y, rect2.Width, rect2.Height);
            } else if (room is VirtualGenRoom) {
                corridors.AddRange(((VirtualGenRoom) room).corridors);
            }
        }

        foreach (Rectangle corridor in corridors) {
            GenerateRectangle(corridor.X, corridor.Y, corridor.Width, corridor.Height);
        }

        root.generateOutside();
        root.generateInside();
    }

    public void GenerateRectangle(int x, int y, int w, int h) {
        GameObject rectPrefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/rectPrefab.prefab", typeof(GameObject)) as GameObject;
        GameObject rect = Instantiate(rectPrefab, new Vector2(x, y), new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
        rect.transform.localScale = new Vector2(w, -h);
    }

    public void GenerateRectangleBackground(int x, int y, int w, int h) {
        GameObject rectPrefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/rectPrefabBackground.prefab", typeof(GameObject)) as GameObject;
        GameObject rect = Instantiate(rectPrefab, new Vector2(x, y), new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
        rect.transform.localScale = new Vector2(w, -h);
        rect.GetComponent<SpriteRenderer>().color = new UnityEngine.Color((float) random.NextDouble(), (float) random.NextDouble(), (float) random.NextDouble());
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