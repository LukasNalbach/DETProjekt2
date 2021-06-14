using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class Wald : OutsideRoom {
    public override void generateInside(List<Rectangle> corridors, Rectangle rectInside, Rectangle rectOutside) {
        WorldGenerator wGen = Game.Instance.GetComponent<WorldGenerator>();
        List<GameObject> placedObjects = new List<GameObject>();

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
        Rectangle signRect = new Rectangle((int) posSign.x - 1, (int)  posSign.y - 4, 3, 5);

        // place random Plants
        int n = 2 * innerRect.Width * innerRect.Height;
        for (int i = 0; i < n; i++) {
            Vector2 pos = new Vector2((int) innerRect.X + random.Next(innerRect.Width), (int) innerRect.Y + random.Next(innerRect.Height));
            if (IsPosFree(pos, corridors, placedObjects)) {
                GameObject obj;

                double rn = random.NextDouble();

                if (rn <= 0.2) {
                    obj = wGen.CreateGrassOnTileWithProb(pos, 1);
                } else if (rn <= 0.4 || VirtualGenRoom.CollidesWith(pos, signRect, "XY")) {
                    obj = wGen.CreateBushOnTile(pos);
                } else {
                    obj = wGen.CreateTreeOnTile(pos);
                }

                placedObjects.Add(obj);
            }
        }
    }
}