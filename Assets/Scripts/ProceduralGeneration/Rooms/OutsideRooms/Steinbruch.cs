using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class Steinbruch : OutsideRoom {
    public override void generateInside(List<Rectangle> corridors, Rectangle rectInside, Rectangle rectOutside) {
        WorldGenerator wGen = Game.Instance.GetComponent<WorldGenerator>();
        List<GameObject> placedObjects = new List<GameObject>();
        
        // set stone area
        Rectangle stoneRect = new Rectangle(innerRect.X + 1, innerRect.Y + 1, innerRect.Width - 2, innerRect.Height - 2);

        // create ground
        for (int x = innerRect.X; x < innerRect.X + innerRect.Width; x++) {
            for (int y = innerRect.Y; y < innerRect.Y + innerRect.Height; y++) {
                wGen.CreateGrassGround(new Vector2(x, y), 0.9, 0.1, 0);
            }
        }

        // place crate
        Vector2 posCrate = new Vector2(0, 0);
        while (posCrate.x == 0) {
            Vector2 pos = new Vector2((int) innerRect.X + 1 + random.Next(innerRect.Width - 2), (int) innerRect.Y + 1 + random.Next(innerRect.Height - 2));
            if (IsPosFree(pos, corridors, placedObjects)) {
                posCrate = pos;
            }
        }
        GameObject crate = wGen.CreateAssetFromPrefab(new Vector2(posCrate.x + 0.5f, posCrate.y), "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Crate.prefab");
        placedObjects.Add(crate);
        Rectangle crateRect = new Rectangle((int) posCrate.x - 1, (int)  posCrate.y - 4, 3, 5);

        // place stones
        string[] stones = {
            "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Stone T1.prefab",
            "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Stone T2.prefab",
            "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Stone T3.prefab",
            "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Stone T4.prefab",
            "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Stone T5.prefab",
            "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Stone T6.prefab",
            "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Stone T7.prefab"
        };
        int m = innerRect.Width * innerRect.Height / 2;
        for (int i = 0; i < m; i++) {
            Vector2 pos = new Vector2((int) stoneRect.X + random.Next(stoneRect.Width), (int) stoneRect.Y + random.Next(stoneRect.Height));
            if (IsPosFree(pos, corridors, placedObjects)) {
                GameObject obj;
                double rn = random.NextDouble();

                if (rn <= 0.8) {
                    obj = wGen.CreateAssetFromPrefab(wGen.RandomPosInMiddleOfTile(pos), stones[random.Next(stones.Length)]);
                } else if (rn <= 0.9) {
                    obj = wGen.CreateGrassOnTileWithProb(pos, 1);
                } else if (rn <= 0.95 || VirtualGenRoom.IsCloserToThan(pos, crateRect, "XY", 0)) {
                    obj = wGen.CreateBushOnTile(pos);
                } else {
                    obj = wGen.CreateTreeOnTile(pos);
                }

                placedObjects.Add(obj);
            }
        }
    }
}