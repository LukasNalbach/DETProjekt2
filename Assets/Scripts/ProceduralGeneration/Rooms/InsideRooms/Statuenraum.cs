using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class Statuenraum : InsideRoom {
    public override void generateInside(WorldGenerator wGen, List<Rectangle> corridors, Rectangle rectInside, Rectangle rectOutside) {
        Debug.Log("Starting to generate " + this.GetType() + ", Vents " + (ventName != "" ? "enabled" : "disabled") + "innerRect: " + innerRect.Width + "x" + innerRect.Height);
        // set statue position
        string placementMode = "";
        Rectangle safeRect = new Rectangle(innerRect.X + 2, innerRect.Y + 2, innerRect.Width - 4, innerRect.Height - 4);
        Vector2 posStatue;

        if (innerRect.Width >= innerRect.Height) { // sideways 
            int y = innerRect.Y + innerRect.Height / 2;
            if (random.NextDouble() <= 0.5) { // statue is left
                placementMode = "LEFT";
                posStatue = new Vector2(safeRect.X, y);
            } else { // statue is right
                placementMode = "RIGHT";
                posStatue = new Vector2(safeRect.X + safeRect.Width - 1, y);
            }
        } else { // statue is bottom
            placementMode = "BOTTOM";
            int x = innerRect.X + innerRect.Width / 2;
            posStatue = new Vector2(x, safeRect.Y);
        }

        // create statue
        GameObject statue = wGen.CreateAssetFromPrefab(posStatue + new Vector2(0.5f, 0.0f), "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Statue.prefab");
        placedObjects.Add(statue);
        task = statue;

        // set statue podest area
        Rectangle podestRect = new Rectangle((int) posStatue.x - 1, (int) posStatue.y - 1, 3, 3);

        // create ground and statue podest
        for (int x = innerRect.X; x < innerRect.X + innerRect.Width; x++) {
            for (int y = innerRect.Y; y < innerRect.Y + innerRect.Height; y++) {
                Vector2 pos = new Vector2(x, y);
                if (VirtualGenRoom.IsCloserToThan(pos, podestRect, "XY", 0)) {
                    wGen.CreateStoneGround(new Vector2(x, y));
                } else {
                    wGen.CreateGrassGround(pos, 0.04, 0.01, 0.5);
                }
            }
        }

        // create benches
        if (placementMode == "LEFT") {
            for (int x = (int) posStatue.x + 3; x < Math.Min(safeRect.X + safeRect.Width, safeRect.X + 10); x = x + 2) {
                for (int y = safeRect.Y; y < safeRect.Y + safeRect.Height; y = y + 3) {
                    placedObjects.Add(wGen.CreateAssetFromPrefab(new Vector2(x + 0.5f, y), "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Stone Bench W.prefab"));
                }
            }
        } else if (placementMode == "RIGHT") {
            for (int x = (int) posStatue.x - 3; x >= Math.Max(safeRect.X, posStatue.x - 10); x = x - 2) {
                for (int y = safeRect.Y; y < safeRect.Y + safeRect.Height; y = y + 3) {
                    placedObjects.Add(wGen.CreateAssetFromPrefab(new Vector2(x + 0.5f, y), "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Stone Bench E.prefab"));
                }
            }
        } else {
            for (int y = (int) posStatue.y + 3; y < Math.Min(safeRect.Y + safeRect.Height, safeRect.Y + 10); y = y + 2) {
                for (int x = safeRect.X; x < safeRect.X + safeRect.Width; x = x + 3) {
                    placedObjects.Add(wGen.CreateAssetFromPrefab(new Vector2(x + 0.5f, y), "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Stone Bench S.prefab"));
                }
            }
        }

        if (ventName != "") {
            Vector2 posVent = new Vector2(0, 0);
            while (posVent.x == 0) {
                Vector2 pos = new Vector2((int) innerRect.X + random.Next(innerRect.Width), (int) innerRect.Y + 2 + random.Next(innerRect.Height - 2));
                if (IsPosFree(pos, corridors, placedObjects)) {
                    posVent = pos;
                }
            }
            GameObject vent = wGen.CreateAssetFromPrefab(posVent + new Vector2(0.5f, 0), "Assets/Prefabs/Vent.prefab");
            placedObjects.Add(vent);
            vent.name = ventName;
        }

        // create objects at the wall
        string[] wallObjects = {
            "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Crate Small.prefab",
            "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Pot A.prefab",
            "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Pot B.prefab",
            "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Pot C.prefab",
            "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Stone Cube.prefab"
        };
        for (int x = innerRect.X; x < innerRect.X + innerRect.Width; x += 1 +random.Next(1)) {
            if (random.NextDouble() <= 0.3) {
                int[] ys = {innerRect.Y, innerRect.Y + innerRect.Height - 1};

                foreach (int y in ys) {
                    Vector2 pos = new Vector2(x, y);
                    if (IsPosFree(pos, corridors, placedObjects)) {
                        placedObjects.Add(wGen.CreateAssetFromPrefab(new Vector2(x + 0.5f, y), wallObjects[random.Next(wallObjects.Length)]));
                    }
                }
            }
        }
        for (int y = innerRect.Y; y < innerRect.Y + innerRect.Height; y += 1 +random.Next(1)) {
            if (random.NextDouble() <= 0.3) {
                int[] xs = {innerRect.X, innerRect.X + innerRect.Width - 1};

                foreach (int x in xs) {
                    Vector2 pos = new Vector2(x, y);
                    if (IsPosFree(pos, corridors, placedObjects)) {
                        placedObjects.Add(wGen.CreateAssetFromPrefab(new Vector2(x + 0.5f, y), wallObjects[random.Next(wallObjects.Length)]));
                    }
                }
            }
        }

        // place random pillars
        string[] pillars = {
            "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Rune Pillar Broken.prefab",
            "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Rune Pillar X2.prefab",
            "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Rune Pillar X3.prefab"
        };
        int n = innerRect.Width * innerRect.Height / 20;
        for (int i = 0; i < n; i++) {
            Vector2 pos = new Vector2((int) innerRect.X + random.Next(innerRect.Width), (int) innerRect.Y + random.Next(innerRect.Height));
            if (IsPosFree(pos, corridors, placedObjects)) {
                placedObjects.Add(wGen.CreateAssetFromPrefab(new Vector2(pos.x + 0.5f, pos.y), pillars[random.Next(pillars.Length)]));
            }
        }

        Debug.Log(this.GetType() + " generated");
    }
}