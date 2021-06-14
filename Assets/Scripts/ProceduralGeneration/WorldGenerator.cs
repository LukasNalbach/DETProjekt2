using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System;
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
        //roomsInside.Add(new Thronsaal());

        roomsOutside.Add(new Grabstaette());
        roomsOutside.Add(new Brunnen());
        roomsOutside.Add(new Wald());
        roomsOutside.Add(new Lavagrube());
        //roomsOutside.Add(new Zeltlager());

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

        VirtualGenRoom root;

        if (random.NextDouble() <= 0.5) {
            root = GenRoom.joinRooms(rootInside, rootOutside);
        } else {
            root = GenRoom.joinRooms(rootOutside, rootInside);
        }

        foreach (GenRoom room in root.getSubrooms()) {
            room.setRandom(random);
        }

        Rectangle worldArea = new Rectangle(50, 50, 50 + random.Next(20), 50 + random.Next(20));
        root.generate(worldArea);

        foreach (GenRoom room in root.getSubrooms()) {
            if (room is RealGenRoom) {
                rooms.Add(GetRoomTypeFromObject((RealGenRoom) room), ((RealGenRoom) room).innerRect);
            } else if (room is VirtualGenRoom) {
                corridors.AddRange(((VirtualGenRoom) room).corridors);
            }
        }

        root.generateOutside(corridors, rootInside.outerRect, rootOutside.outerRect);
        root.generateInside(corridors, rootInside.outerRect, rootOutside.outerRect);

        InsideRoom.generateRuinEdge(corridors, rootInside.outerRect, rootOutside.outerRect);
        OutsideRoom.GenerateForestOutside(corridors, rootInside.outerRect, rootOutside.outerRect);

        /*
        foreach (KeyValuePair<RoomType,Rectangle> room in rooms) {
            GenerateRectangle(room.Value);
        }
        foreach (Rectangle corridor in corridors) {
            GenerateRectangle(corridor);
        }
        */
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
            pos.x >= rect.X && pos.x < rect.X + rect.Width &&
            pos.y >= rect.Y && pos.y < rect.Y + rect.Height
        );
    }

    public void GenerateRectangle(Rectangle rect) {
        GameObject rectPrefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/rectPrefab.prefab", typeof(GameObject)) as GameObject;
        GameObject drawnRect = Instantiate(rectPrefab, new Vector2(rect.X, rect.Y), new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
        drawnRect.transform.localScale = new Vector2(rect.Width, -rect.Height);
    }

    public GameObject CreateLavaTile(Vector2 pos) {
        GameObject prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/lavaTile.prefab", typeof(GameObject)) as GameObject;
        GameObject obj = Instantiate(prefab, new Vector2(pos.x + 0.5f, pos.y + 0.5f), new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
        return obj;
    }

    public GameObject CreateColliderTile(Vector2 pos) {
        GameObject prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/ColliderTile.prefab", typeof(GameObject)) as GameObject;
        GameObject obj = Instantiate(prefab, new Vector2(pos.x + 0.5f, pos.y + 0.5f), new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
        return obj;
    }

    public GameObject CreateGrassOnTileWithProb(Vector2 pos, double prob) {
        if (random.NextDouble() <= prob) {
            return CreateGrass(RandomPosInMiddleOfTile(pos));
        }
        return null;
    }

    public Vector2 RandomPosInMiddleOfTile(Vector2 pos) {
        return new Vector2(pos.x + 0.2f + 0.6f * (float) random.NextDouble(), pos.y + 0.2f + 0.6f * (float) random.NextDouble());
    }

    public GameObject CreateBushOnTile(Vector2 pos) {
        return CreateBush(RandomPosInMiddleOfTile(pos));
    }

    public GameObject CreateAssetFromPrefab(Vector2 pos, string pathToPrefab) {
        GameObject prefab = AssetDatabase.LoadAssetAtPath(pathToPrefab, typeof(GameObject)) as GameObject;
        GameObject obj = Instantiate(prefab, pos, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
        return obj;
    }

    public GameObject CreateBush(Vector2 pos) {
        string[] types = {"T2", "T2", "T3", "T4", "T5", "T6"};
        string type = types[random.Next(6)];
        GameObject prefab = AssetDatabase.LoadAssetAtPath("Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Plant/PF Props Bush " + type + ".prefab", typeof(GameObject)) as GameObject;
        GameObject obj = Instantiate(prefab, pos, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
        return obj;
    }

    public GameObject CreateTreeOnTile(Vector2 pos) {
        return CreateTree(RandomPosInMiddleOfTile(pos));
    }

    public GameObject CreateTree(Vector2 pos) {
        string[] types = {"T2", "T2", "T3"};
        string type = types[random.Next(3)];
        GameObject prefab = AssetDatabase.LoadAssetAtPath("Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Plant/PF Props Tree " + type + ".prefab", typeof(GameObject)) as GameObject;
        GameObject obj = Instantiate(prefab, pos, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
        return obj;
    }

    public GameObject CreateGrass(Vector2 pos) {
        string[] types = {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O"};
        string type = types[random.Next(15)];
        GameObject prefab = AssetDatabase.LoadAssetAtPath("Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Plant/PF Grass " + type + ".prefab", typeof(GameObject)) as GameObject;
        GameObject obj = Instantiate(prefab, pos, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
        return obj;
    }

    public GameObject CreateGrassGround(Vector2 pos, double probNormal, double probFlowers, double probPartStone) {
        double rn = random.NextDouble();
        string typeString = "TX Tileset Grass";

        if (rn <= probNormal) {
            typeString += " " + random.Next(16);
        } else if (rn <= probNormal + probFlowers) {
            typeString += " Flower " + random.Next(16);
        } else if (rn <= probNormal + probFlowers + probPartStone) {
            int[] wrongNumbers = {40, 41, 48, 49, 56, 57};
            List<int> wrongNumbersList = wrongNumbers.ToList<int>();
            int rn2;
            do {
                rn2 = 34 + random.Next(28);
            } while (wrongNumbersList.Contains(rn2));
            typeString += "_" + rn2;
        } else {
            typeString += " Pavement " + random.Next(8);
        }

        return CreateSquareTile(pos, "Assets/Cainos/Pixel Art Top Down - Basic/Tile Palette/TP Grass/" + typeString + ".asset");
    }

    public GameObject CreateStoneGround(Vector2 pos) {
        return CreateSquareTile(pos, "Assets/Cainos/Pixel Art Top Down - Basic/Tile Palette/TP Stone Ground/TX Tileset Stone Ground_27.asset");
    }

    public GameObject CreateWall(Vector2 pos, string type) {
        GameObject prefab = AssetDatabase.LoadAssetAtPath("Assets/Cainos/Pixel Art Top Down - Basic/Tile Palette/TP Wall Prefabs/Stone Wall " + type + ".prefab", typeof(GameObject)) as GameObject;
        GameObject obj = Instantiate(prefab, new Vector2(pos.x + 0.5f, (pos.y + 0.5f)), new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
        return obj;
    }

    public GameObject CreateForest(Vector2 pos, bool allowTree) {
        CreateGrassGround(pos, 0.8, 0.2, 0.0);

        GameObject obj;

        if (allowTree && random.NextDouble() <= 0.05) {
            obj = CreateTreeOnTile(pos);
        } else {
            obj = CreateBushOnTile(pos);
        }

        return obj;
    }

    public static Rectangle GetRectangleFromTransform(Transform transform) {
        return new Rectangle((int) Math.Floor(transform.position.x), (int) Math.Floor(transform.position.y), (int) Math.Ceiling(transform.localScale.x), (int) Math.Ceiling(transform.localScale.y));
    }

    public GameObject CreateSquareTile(Vector2 pos, string pathToTile) {
        GameObject prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/rectPrefab.prefab", typeof(GameObject)) as GameObject;
        GameObject obj = Instantiate(prefab, new Vector2(pos.x + 0.5f, (pos.y + 0.5f)), new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
        UnityEngine.Sprite sprite = ((UnityEngine.Tilemaps.Tile) AssetDatabase.LoadAssetAtPath(pathToTile, typeof(UnityEngine.Tilemaps.Tile))).sprite;
        obj.GetComponent<SpriteRenderer>().sprite = sprite;
        return obj;
    }

    public static Vector2 CenterPosition(Rectangle rect) {
        return new Vector2(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
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