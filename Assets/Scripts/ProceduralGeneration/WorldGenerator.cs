using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
public class WorldGenerator : MonoBehaviour
{
    private System.Random random = new System.Random();
    public Dictionary<RoomType,Rectangle> rooms = new Dictionary<RoomType,Rectangle>();
    public List<Rectangle> corridors = new List<Rectangle>();
    public void Awake() {

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

        foreach (GenRoom room in root.getSubrooms()) {
            if (room is RealGenRoom) {
                rooms.Add(GetRoomTypeFromObject((RealGenRoom) room), ((RealGenRoom) room).innerRect);
            } else if (room is VirtualGenRoom) {
                corridors.AddRange(((VirtualGenRoom) room).corridors);
            }
        }

        root.generateOutside();
        root.generateInside();
    }

    public static RoomType GetRoomTypeFromObject(RealGenRoom room) {
        return (RoomType) System.Enum.Parse(typeof(RoomType), room.GetType().ToString(), true);
    }

    public string GetNameFromPos(Vector2 pos) {
        foreach (KeyValuePair<RoomType,Rectangle> room in rooms) {
            if (IsPosInRectangle(pos, room.Value)) {
                return room.Key.ToString();
            }
        }
        for(int i = 0; i < corridors.Count; i++) {
            if (IsPosInRectangle(pos, corridors[i])) {
                return "Korridor " + i;
            }
        }
        return "Raum/Korridor nicht gefunden";
    }

    public static bool IsPosInRectangle(Vector2 pos, Rectangle rect) {
        return (
            pos.x >= rect.Left && pos.x < rect.Right &&
            pos.y >= rect.Top && pos.x < rect.Bottom
        );
    }

    public void GenerateRectangle(Rectangle rect) {
        GameObject rectPrefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/rectPrefab.prefab", typeof(GameObject)) as GameObject;
        GameObject drawnRect = Instantiate(rectPrefab, new Vector2(rect.X, rect.Y), new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
        drawnRect.transform.localScale = new Vector2(rect.Width, -rect.Height);
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