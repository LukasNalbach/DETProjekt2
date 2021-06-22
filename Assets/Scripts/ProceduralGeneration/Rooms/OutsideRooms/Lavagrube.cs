using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class Lavagrube : OutsideRoom {
    public Rectangle lavaRect;
    public override void generateInside(WorldGenerator wGen, List<Rectangle> corridors, Rectangle rectInside, Rectangle rectOutside) {

        lavaRect = new Rectangle(innerRect.X + innerRect.Width / 3, innerRect.Y + innerRect.Height / 3, Math.Min(Math.Max(innerRect.Width / 3, 3), 7), Math.Min(Math.Max(innerRect.Height / 3, 3), 7));
        Rectangle lavaCollisionRect = new Rectangle(lavaRect.X, lavaRect.Y - 4, lavaRect.Width, lavaRect.Height + 4);

        Vector2 posRune = new Vector2(0, 0);
        while (posRune.x == 0) {
            Vector2 pos = new Vector2((int) innerRect.X + random.Next(innerRect.Width), (int) innerRect.Y + random.Next(innerRect.Height));
            if (!VirtualGenRoom.IsCloserToThan(pos, lavaRect, "XY", 1) && IsPosFree(pos, corridors, placedObjects)) {
                posRune = pos;
            }
        }
        GameObject rune = wGen.CreateAssetFromPrefab(posRune + new Vector2(0.5f, 0), "Assets/Cainos/Pixel Art Top Down - Basic/Prefab/Props/PF Props Rune Pillar X2.prefab");
        placedObjects.Add(rune);
        task = rune;
        Rectangle runeCollisionRect = new Rectangle((int) posRune.x - 1, (int)  posRune.y - 4, 3, 5);

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

        // create ground and lava
        for (int x = innerRect.X; x < innerRect.X + innerRect.Width; x++) {
            for (int y = innerRect.Y; y < innerRect.Y + innerRect.Height; y++) {
                Vector2 pos = new Vector2(x, y);
                if (WorldGenerator.IsPosInRectangle(pos, lavaRect)) {
                    wGen.CreateLavaTile(pos);
                    wGen.CreateColliderTile(pos);
                } else {
                    wGen.CreateGrassGround(pos, 0.8, 0.2, 0);
                }
            }
        }

        // place random Plants
        int n = innerRect.Width * innerRect.Height / 2;
        for (int i = 0; i < n; i++) {
            Vector2 pos = new Vector2((int) innerRect.X + random.Next(innerRect.Width), (int) innerRect.Y + random.Next(innerRect.Height));
            if (
                !VirtualGenRoom.IsCloserToThan(pos, ventRect, "XY", 0) &&
                !VirtualGenRoom.IsCloserToThan(pos, runeCollisionRect, "XY", 0) &&
                !VirtualGenRoom.IsCloserToThan(pos, lavaCollisionRect, "XY", 0) &&
                IsPosFree(pos, corridors, placedObjects)
            ) {
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