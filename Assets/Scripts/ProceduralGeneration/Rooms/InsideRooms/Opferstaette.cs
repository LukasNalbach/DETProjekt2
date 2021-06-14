using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class Opferstaette : InsideRoom {
    public override void generateInside(List<Rectangle> corridors, Rectangle rectInside, Rectangle rectOutside) {
        WorldGenerator wGen = Game.Instance.GetComponent<WorldGenerator>();
        List<GameObject> placedObjects = new List<GameObject>();
        
        // set altar position
        string placementMode = "";
        Rectangle safeRect = new Rectangle(innerRect.X + 2, innerRect.Y + 2, innerRect.Width - 4, innerRect.Height - 4);
        Vector2 posAltar;

        if (innerRect.Width >= innerRect.Height) { // sideways 
            float y = innerRect.Y + ((float) innerRect.Height) / 2;
            if (random.NextDouble() <= 0.5) { // altar is left
                placementMode = "LEFT";
                posAltar = new Vector2(safeRect.X, y);
            } else { // altar is right
                placementMode = "RIGHT";
                posAltar = new Vector2(safeRect.X + safeRect.Width - 1, y);
            }
        } else { // altar is bottom
            placementMode = "BOTTOM";
            float x = innerRect.X + ((float) innerRect.Width) / 2;
            posAltar = new Vector2(x, safeRect.Y);
        }

        // create altar
        GameObject altar = wGen.CreateAssetFromPrefab(posAltar, "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Altar.prefab");
        placedObjects.Add(altar);

        // create ground
        for (int x = innerRect.X; x < innerRect.X + innerRect.Width; x++) {
            for (int y = innerRect.Y; y < innerRect.Y + innerRect.Height; y++) {
                wGen.CreateGrassGround(new Vector2(x, y), 0, 0, 0);
            }
        }

        // create benches
        if (placementMode == "LEFT") {
            for (int x = (int) posAltar.x + 3; x < Math.Min(safeRect.X + safeRect.Width, safeRect.X + 10); x = x + 2) {
                for (int y = safeRect.Y; y < safeRect.Y + safeRect.Height; y = y + 3) {
                    placedObjects.Add(wGen.CreateAssetFromPrefab(new Vector2(x + 0.5f, y), "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Stone Bench W.prefab"));
                }
            }
        } else if (placementMode == "RIGHT") {
            for (int x = (int) posAltar.x - 3; x >= Math.Max(safeRect.X, posAltar.x - 10); x = x - 2) {
                for (int y = safeRect.Y; y < safeRect.Y + safeRect.Height; y = y + 3) {
                    placedObjects.Add(wGen.CreateAssetFromPrefab(new Vector2(x + 0.5f, y), "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Stone Bench E.prefab"));
                }
            }
        } else {
            for (int y = (int) posAltar.y + 3; y < Math.Min(safeRect.Y + safeRect.Height, safeRect.Y + 10); y = y + 2) {
                for (int x = safeRect.X; x < safeRect.X + safeRect.Width; x = x + 3) {
                    placedObjects.Add(wGen.CreateAssetFromPrefab(new Vector2(x + 0.5f, y), "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Stone Bench S.prefab"));
                }
            }
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
            "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Pillar Broken.prefab",
            "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Pillar.prefab"
        };
        int n = innerRect.Width * innerRect.Height / 20;
        for (int i = 0; i < n; i++) {
            Vector2 pos = new Vector2((int) innerRect.X + random.Next(innerRect.Width), (int) innerRect.Y + random.Next(innerRect.Height));
            if (IsPosFree(pos, corridors, placedObjects)) {
                placedObjects.Add(wGen.CreateAssetFromPrefab(new Vector2(pos.x + 0.5f, pos.y), pillars[random.Next(pillars.Length)]));
            }
        }
    }
}