using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class Brunnen : OutsideRoom {
    public override void generateInside(List<Rectangle> corridors, Rectangle rectInside, Rectangle rectOutside) {
        WorldGenerator wGen = Game.Instance.GetComponent<WorldGenerator>();

        // create ground
        for (int x = innerRect.X; x < innerRect.X + innerRect.Width; x++) {
            for (int y = innerRect.Y; y < innerRect.Y + innerRect.Height; y++) {
                wGen.CreateGrassGround(new Vector2(x, y), 0.8, 0.05, 0.15);
            }
        }

        // place brunnen
        Vector2 posBrunnen = new Vector2(0, 0);
        while (posBrunnen.x == 0) {
            Vector2 pos = new Vector2((int) innerRect.X + 1 + random.Next(innerRect.Width - 2), (int) innerRect.Y + 1 + random.Next(innerRect.Height - 2));
            if (IsPosFree(pos, corridors, placedObjects)) {
                posBrunnen = pos;
            }
        }
        GameObject brunnen = wGen.CreateAssetFromPrefab(new Vector2(posBrunnen.x + 0.5f, posBrunnen.y), "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Well.prefab");
        placedObjects.Add(brunnen);
        task = brunnen;
        Rectangle brunnenRect = new Rectangle((int) posBrunnen.x - 1, (int)  posBrunnen.y - 4, 3, 5);

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
        int n = innerRect.Width * innerRect.Height / 2;
        for (int i = 0; i < n; i++) {
            Vector2 pos = new Vector2((int) innerRect.X + random.Next(innerRect.Width), (int) innerRect.Y + random.Next(innerRect.Height));
            if (IsPosFree(pos, corridors, placedObjects)) {
                GameObject obj;

                double rn = random.NextDouble();

                if (rn <= 0.7) {
                    obj = wGen.CreateGrassOnTileWithProb(pos, 1);
                } else if (rn <= 0.9 || VirtualGenRoom.IsCloserToThan(pos, brunnenRect, "XY", 0) || VirtualGenRoom.IsCloserToThan(pos, ventRect, "XY", 0)) {
                    obj = wGen.CreateBushOnTile(pos);
                } else {
                    obj = wGen.CreateTreeOnTile(pos);
                }

                placedObjects.Add(obj);
            }
        }

        // place random pillars
        string[] pillars = {
            "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Rune Pillar Broken.prefab",
            "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Brick T1.prefab",
            "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Brick T2.prefab",
            "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Brick T3.prefab"
        };
        int m = innerRect.Width * innerRect.Height / 5;
        for (int i = 0; i < m; i++) {
            Vector2 pos = new Vector2((int) innerRect.X + random.Next(innerRect.Width), (int) innerRect.Y + random.Next(innerRect.Height));
            if (IsPosFree(pos, corridors, placedObjects)) {
                placedObjects.Add(wGen.CreateAssetFromPrefab(new Vector2(pos.x + 0.5f, pos.y), pillars[random.Next(pillars.Length)]));
            }
        }
    }
}