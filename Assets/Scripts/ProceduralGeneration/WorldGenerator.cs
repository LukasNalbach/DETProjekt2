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
    public Dictionary<RoomType,RealGenRoom> rooms = new Dictionary<RoomType,RealGenRoom>();
    public List<Rectangle> corridors = new List<Rectangle>();
    public List<Vector2> checkpoints = new List<Vector2>();
    public Rectangle worldArea;

    private GameObject minimapPlayer;
    private Dictionary<Task,GameObject> minimapTasks = new Dictionary<Task,GameObject>();
    private Dictionary<SabortageTask,GameObject> minimapSabotageTasks = new Dictionary<SabortageTask,GameObject>();
    public void Awake() {

        List<RealGenRoom> roomsInside = new List<RealGenRoom>(6);
        List<RealGenRoom> roomsOutside = new List<RealGenRoom>(4);

        roomsInside.Add(new Meetingraum());
        roomsInside.Add(new Opferstaette());
        roomsInside.Add(new Schatzkammer());
        roomsInside.Add(new Statuenraum());
        roomsInside.Add(new Thronsaal());

        roomsOutside.Add(new Grabstaette());
        roomsOutside.Add(new Brunnen());
        roomsOutside.Add(new Wald());
        roomsOutside.Add(new Lavagrube());
        roomsOutside.Add(new Steinbruch());

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

        worldArea = new Rectangle(-50, -50, 50 + random.Next(20), 50 + random.Next(20));
        root.generate(this, worldArea);
        //GameObject.Find("MinimapCamera").transform.position = ((Vector3) CenterPosition(worldArea)) + new Vector3Int(0, 0, -1000);

        foreach (GenRoom room in root.getSubrooms()) {
            if (room is RealGenRoom) {
                rooms.Add(GetRoomTypeFromObject((RealGenRoom) room), (RealGenRoom) room);
            } else if (room is VirtualGenRoom) {
                corridors.AddRange(((VirtualGenRoom) room).corridors);
            }
        }

        foreach (Rectangle corridor in corridors) {
            if (corridor.Width == 2) {
                checkpoints.Add(new Vector2(corridor.X + 1, corridor.Y));
                checkpoints.Add(new Vector2(corridor.X + 1, corridor.Y + corridor.Height));
            } else {
                checkpoints.Add(new Vector2(corridor.X, corridor.Y + 1));
                checkpoints.Add(new Vector2(corridor.X + corridor.Width, corridor.Y + 1));
            }
        }

        Debug.Log(checkpoints.Count);

        List<RealGenRoom> ventRoomsInside = new List<RealGenRoom>();
        List<RealGenRoom> ventRoomsOutside = new List<RealGenRoom>();
        List<GenRoom> realRoomsInside = rootInside.getSubrooms().Where((room) => room is RealGenRoom).ToList<GenRoom>();
        List<GenRoom> realRoomsOutside = rootOutside.getSubrooms().Where((room) => room is RealGenRoom).ToList<GenRoom>();

        int index = random.Next(realRoomsInside.Count);
        ventRoomsInside.Add((RealGenRoom) realRoomsInside[index]);
        realRoomsInside.RemoveAt(index);
        index = random.Next(realRoomsInside.Count);
        ventRoomsInside.Add((RealGenRoom) realRoomsInside[index]);
        realRoomsInside.RemoveAt(index);

        ventRoomsOutside.Add((RealGenRoom) realRoomsOutside[index]);
        realRoomsOutside.RemoveAt(index);
        index = random.Next(realRoomsOutside.Count);
        ventRoomsOutside.Add((RealGenRoom) realRoomsOutside[index]);
        realRoomsOutside.RemoveAt(index);

        ventRoomsInside[0].ventName = "Vent1";
        ventRoomsInside[1].ventName = "Vent2";

        ventRoomsOutside[0].ventName = "Vent3";
        ventRoomsOutside[1].ventName = "Vent4";

        root.generateOutside(this, corridors, rootInside.outerRect, rootOutside.outerRect);
        root.generateInside(this, corridors, rootInside.outerRect, rootOutside.outerRect);

        InsideRoom.generateRuinEdge(this, corridors, rootInside.outerRect, rootOutside.outerRect);
        OutsideRoom.GenerateForestOutside(this, corridors, rootInside.outerRect, rootOutside.outerRect);

        List<KeyValuePair<RoomType,RealGenRoom>> roomsList = rooms.ToList<KeyValuePair<RoomType,RealGenRoom>>();
        for (int i = 0; i < roomsList.Count; i++) {
            roomsList[i].Value.task.name = "Room" + i + "Task";
        }

        RealGenRoom meetingraum;
        rooms.TryGetValue(RoomType.Meetingraum, out meetingraum);
        Destroy(((Meetingraum) meetingraum).emergencyButton.GetComponent<Rigidbody2D>());
        
        GenerateMinimap();
        StartCoroutine(updateMinimap());
    }

    public void GenerateMinimap() {
        foreach (RealGenRoom room in rooms.Values.Where((room) => room is RealGenRoom)) {
            GenreateRectangleOnMinimap(room.innerRect);
        }
        foreach (Rectangle corridor in corridors) {
            GenreateRectangleOnMinimap(corridor);
        }
    }

    public IEnumerator updateMinimap() {
        while (true) {
            // remove old tasks
            foreach (KeyValuePair<Task,GameObject> minimapTask in minimapTasks.Where((t) => !t.Key.solvingVisible || !Game.Instance.allTasks.Contains(t.Key))) {
                Destroy(minimapTask.Value);
                minimapTasks.Remove(minimapTask.Key);
            }
            // create new Tasks
            foreach (Task task in Game.Instance.allTasks.Where((t) => t.solvingVisible && !minimapTasks.ContainsKey(t))) {
                GameObject minimapTask = GenreateTaskOnMinimap(task.gameObject.transform.position);
                minimapTasks.Add(task, minimapTask);
            }

            // remove old sabotageTasks
            foreach (KeyValuePair<SabortageTask,GameObject> minimapSabotageTask in minimapSabotageTasks.Where((t) => !t.Key.solvingVisible || !Game.Instance.allActiveSabortageTasks().Contains(t.Key))) {
                Destroy(minimapSabotageTask.Value);
                minimapTasks.Remove(minimapSabotageTask.Key);
            }
            // create new sabotageTasks
            foreach (SabortageTask sabotageTask in Game.Instance.allActiveSabortageTasks().Where((t) => t.solvingVisible && !minimapSabotageTasks.ContainsKey(t))) {
                GameObject minimapSabotageTask = GenreateTaskOnMinimap(sabotageTask.gameObject.transform.position);
                minimapTasks.Add(sabotageTask, minimapSabotageTask);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public GameObject GenreateTaskOnMinimap(Vector2 pos) {
        GameObject taskPrefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Task.prefab", typeof(GameObject)) as GameObject;
        GameObject task = Instantiate(taskPrefab, pos, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
        task.layer = 6;
        task.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
        return task;
    }

    public GameObject GenreateSabotageTaskOnMinimap(Vector2 pos) {
        GameObject sabotagePrefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Task.prefab", typeof(GameObject)) as GameObject;
        GameObject sabotage = Instantiate(sabotagePrefab, pos, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
        sabotage.layer = 6;
        sabotage.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
        return sabotage;
    }

    public static RoomType GetRoomTypeFromObject(RealGenRoom room) {
        return (RoomType) System.Enum.Parse(typeof(RoomType), room.GetType().ToString(), true);
    }

    public KeyValuePair<int,string> GetPlaceFromPos(Vector2 pos) {
        List<KeyValuePair<RoomType,RealGenRoom>> roomsList = rooms.ToList<KeyValuePair<RoomType,RealGenRoom>>();
        for (int i = 0; i < roomsList.Count; i++) {
            if (IsPosInRectangle(pos, roomsList[i].Value.innerRect)) {
                return new KeyValuePair<int,string>(i, roomsList[i].Key.ToString());
            }
        }
        for(int i = 0; i < corridors.Count; i++) {
            if (IsPosInRectangle(pos, corridors[i])) {
                return new KeyValuePair<int,string>(i, "Korridor " + i);
            }
        }
        return new KeyValuePair<int,string>(- 1, "Raum/Korridor nicht gefunden");
    }

    public static bool IsPosInRectangle(Vector2 pos, Rectangle rect) {
        return (
            pos.x >= rect.X && pos.x < rect.X + rect.Width &&
            pos.y >= rect.Y && pos.y < rect.Y + rect.Height
        );
    }

    public GameObject GenreateRectangleOnMinimap(Rectangle rect) {
        GameObject rectPrefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/rectPrefab.prefab", typeof(GameObject)) as GameObject;
        GameObject drawnRect = Instantiate(rectPrefab, new Vector2(rect.X, rect.Y), new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
        drawnRect.transform.localScale = new Vector2(rect.Width, -rect.Height);
        drawnRect.layer = 6;
        drawnRect.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
        drawnRect.GetComponent<SpriteRenderer>().color = new UnityEngine.Color(1f/3f, 1f/3f, 1f/3f, 1f);
        return drawnRect;
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

    public static void DestroyGameObject(GameObject obj) {
        Destroy(obj);
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