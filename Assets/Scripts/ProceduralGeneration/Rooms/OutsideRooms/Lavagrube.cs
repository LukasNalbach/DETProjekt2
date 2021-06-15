using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class Lavagrube : OutsideRoom {
    public override void generateInside(List<Rectangle> corridors, Rectangle rectInside, Rectangle rectOutside) {
        WorldGenerator wGen = Game.Instance.GetComponent<WorldGenerator>();
        List<GameObject> placedObjects = new List<GameObject>();

        Rectangle lavaRect = new Rectangle(innerRect.X + innerRect.Width / 3, innerRect.Y + innerRect.Height / 3, Math.Min(Math.Max(innerRect.Width / 3, 3), 7), Math.Min(Math.Max(innerRect.Height / 3, 3), 7));
        Rectangle lavaCollisionRect = new Rectangle(lavaRect.X, lavaRect.Y - 4, lavaRect.Width, lavaRect.Height + 4);

        // create ground and lava
        for (int x = innerRect.X; x < innerRect.X + innerRect.Width; x++) {
            for (int y = innerRect.Y; y < innerRect.Y + innerRect.Height; y++) {
                Vector2 pos = new Vector2(x, y);
                if (WorldGenerator.IsPosInRectangle(pos, lavaRect)) {
                    wGen.CreateLavaTile(pos);
                } else {
                    wGen.CreateGrassGround(pos, 0.8, 0.2, 0);
                }
            }
        }

        // place random Plants
        int n = innerRect.Width * innerRect.Height / 2;
        for (int i = 0; i < n; i++) {
            Vector2 pos = new Vector2((int) innerRect.X + random.Next(innerRect.Width), (int) innerRect.Y + random.Next(innerRect.Height));
            if (!VirtualGenRoom.IsCloserToThan(pos, lavaCollisionRect, "XY", 0) && IsPosFree(pos, corridors, placedObjects)) {
                GameObject obj;

                double rn = random.NextDouble();

                if (rn <= 0.3) {
                    obj = wGen.CreateGrassOnTileWithProb(pos, 1);
                } else if (rn <= 0.6) {
                    obj = wGen.CreateBushOnTile(pos);
                } else {
                    obj = wGen.CreateTreeOnTile(pos);
                }

                placedObjects.Add(obj);
            }
        }
    }
}