using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class Wald : OutsideRoom {
    public override void generateInside(WorldGenerator wGen, List<Rectangle> corridors, Rectangle rectInside, Rectangle rectOutside) {
        // create ground
        for (int x = innerRect.X; x < innerRect.X + innerRect.Width; x++) {
            for (int y = innerRect.Y; y < innerRect.Y + innerRect.Height; y++) {
                wGen.CreateGrassGround(new Vector2(x, y), 0.75, 0.25, 0);
            }
        }

        // place sign
        Vector2 posSign = new Vector2(0, 0);
        while (posSign.x == 0) {
            Vector2 pos = new Vector2((int) innerRect.X + 1 + random.Next(innerRect.Width - 2), (int) innerRect.Y + 1 + random.Next(innerRect.Height - 2));
            if (IsPosFree(pos, corridors, placedObjects)) {
                posSign = pos;
            }
        }
        GameObject sign = wGen.CreateAssetFromPrefab(new Vector2(posSign.x + 0.5f, posSign.y), "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Road Sign E.prefab");
        placedObjects.Add(sign);
        task = sign;
        Rectangle signRect = new Rectangle((int) posSign.x - 1, (int)  posSign.y - 4, 3, 5);

        // place krüge
        List<GameObject> kruege = new List<GameObject>();
        List<Rectangle> krugRects = new List<Rectangle>();
        while (kruege.Count < 2) {
            Vector2 pos = new Vector2((int) innerRect.X + random.Next(innerRect.Width), (int) innerRect.Y + random.Next(innerRect.Height));
            if (IsPosFree(pos, corridors, placedObjects)) {
                GameObject obj = wGen.CreateAssetFromPrefab(new Vector2(pos.x + 0.5f, pos.y), "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Pot B.prefab");
                WorldGenerator.Destroy(obj.GetComponent<Rigidbody2D>());
                placedObjects.Add(obj);
                kruege.Add(obj);
                krugRects.Add(new Rectangle((int) pos.x - 1, (int)  pos.y - 4, 3, 5));
            }
        }
        kruege[0].name = "Stopsabortage11";
        kruege[1].name = "Stopsabortage12";

        Rectangle ventRect = new Rectangle(0, 0, 0, 0);
        if (ventName != "") {
            Vector2 posVent = new Vector2(0, 0);
            while (posVent.x == 0) {
                Vector2 pos = new Vector2((int) innerRect.X + random.Next(innerRect.Width), (int) innerRect.Y + 2 + random.Next(innerRect.Height - 2));
                if (IsPosFree(pos, corridors, placedObjects)) {
                    posVent = pos;
                }
            }
            GameObject vent = wGen.CreateAssetFromPrefab(posVent + new Vector2(0.5f, 0), "Assets/Prefabs/Vent.prefab");
            ventRect = new Rectangle((int) posVent.x - 1, (int)  posVent.y - 4, 3, 5);
            placedObjects.Add(vent);
            vent.name = ventName;
        }

        // place random Plants
        int n = 2 * innerRect.Width * innerRect.Height;
        for (int i = 0; i < n; i++) {
            Vector2 pos = new Vector2((int) innerRect.X + random.Next(innerRect.Width), (int) innerRect.Y + random.Next(innerRect.Height));
            if (IsPosFree(pos, corridors, placedObjects)) {
                GameObject obj;

                double rn = random.NextDouble();

                if (rn <= 0.2) {
                    obj = wGen.CreateGrassOnTileWithProb(pos, 1);
                } else if (
                    rn <= 0.4 ||
                    VirtualGenRoom.IsCloserToThan(pos, signRect, "XY", 0) ||
                    VirtualGenRoom.IsCloserToThan(pos, krugRects, "XY", 0) ||
                    VirtualGenRoom.IsCloserToThan(pos, ventRect, "XY", 0)
                ) {
                    obj = wGen.CreateBushOnTile(pos);
                } else {
                    obj = wGen.CreateTreeOnTile(pos);
                }

                placedObjects.Add(obj);
            }
        }
    }
}