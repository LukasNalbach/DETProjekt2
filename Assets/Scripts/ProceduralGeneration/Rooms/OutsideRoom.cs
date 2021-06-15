using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public abstract class OutsideRoom : RealGenRoom {

    public static void FillRectWithForest(Rectangle rect, bool allowTrees) {
        WorldGenerator wGen = Game.Instance.GetComponent<WorldGenerator>();

        for (int x = rect.X; x < rect.X + rect.Width; x++) {
            for (int y = rect.Y; y < rect.Y + rect.Height; y++) {
                wGen.CreateForest(new Vector2(x, y), allowTrees);
            }
        }
    }

    public static void GenerateForestOutside(List<Rectangle> corridors, Rectangle rectInside,  Rectangle rectOutside) {
        WorldGenerator wGen = Game.Instance.GetComponent<WorldGenerator>();
        if (rectInside.X + rectInside.Width == rectOutside.X) { // outside is right of inside
            int x = rectOutside.X;
            for (int y = rectOutside.Y - 10; y < rectOutside.Y + rectOutside.Height + 10; y++) {
                Vector2 pos = new Vector2(x, y);
                
                if (!VirtualGenRoom.IsCloserToThan(pos, corridors, "XY", 0)) {
                    wGen.CreateForest(pos, false);
                }
                
            }
            FillRectWithForest(new Rectangle(rectOutside.X + rectOutside.Width, rectOutside.Y - 10, 10, rectOutside.Height + 20), true);
            FillRectWithForest(new Rectangle(rectOutside.X + 1, rectOutside.Y - 10, rectOutside.Width - 1, 10), true);
            FillRectWithForest(new Rectangle(rectOutside.X + 1, rectOutside.Y + rectOutside.Height, rectOutside.Width - 1, 10), true);
        } else if (rectInside.X == rectOutside.X + rectOutside.Width) { // outside is left of inside
            int x = rectOutside.X + rectOutside.Width - 1;
            for (int y = rectOutside.Y - 10; y < rectOutside.Y + rectOutside.Height + 10; y++) {
                Vector2 pos = new Vector2(x, y);

                if (!VirtualGenRoom.IsCloserToThan(pos, corridors, "XY", 0)) {
                    wGen.CreateForest(pos, false);
                }
            }
            FillRectWithForest(new Rectangle(rectOutside.X - 10, rectOutside.Y - 10, 10, rectOutside.Height + 20), true);
            FillRectWithForest(new Rectangle(rectOutside.X, rectOutside.Y - 10, rectOutside.Width - 1, 10), true);
            FillRectWithForest(new Rectangle(rectOutside.X, rectOutside.Y + rectOutside.Height, rectOutside.Width - 1, 10), true);
        } else if (rectInside.Y + rectInside.Height == rectOutside.Y) { // outside is above inside
            int y = rectOutside.Y;
            for (int x = rectOutside.X - 10; x < rectOutside.X + rectOutside.Width + 10; x++) {
                Vector2 pos = new Vector2(x, y);

                if (!VirtualGenRoom.IsCloserToThan(pos, corridors, "XY", 0)) {
                    wGen.CreateForest(pos, false);
                }
            }
            FillRectWithForest(new Rectangle(rectOutside.X - 10, rectOutside.Y + 1, 10, rectOutside.Height - 1), true);
            FillRectWithForest(new Rectangle(rectOutside.X + rectOutside.Width, rectOutside.Y + 1, 10, rectOutside.Height - 1), true);
            FillRectWithForest(new Rectangle(rectOutside.X - 10, rectOutside.Y + rectOutside.Height, rectOutside.Width + 20, 10), true);
        } else { // outside is below inside
            int y = rectOutside.Y + rectOutside.Height - 1;
            for (int x = rectOutside.X - 10; x < rectOutside.X + rectOutside.Width + 10; x++) {
                Vector2 pos = new Vector2(x, y);
                
                if (!VirtualGenRoom.IsCloserToThan(pos, corridors, "XY", 0)) {
                    wGen.CreateForest(pos, false);
                }
            }
            FillRectWithForest(new Rectangle(rectOutside.X - 10, rectOutside.Y, 10, rectOutside.Height - 1), true);
            FillRectWithForest(new Rectangle(rectOutside.X + rectOutside.Width, rectOutside.Y, 10, rectOutside.Height - 1), true);
            FillRectWithForest(new Rectangle(rectOutside.X - 10, rectOutside.Y - 10, rectOutside.Width + 20, 10), true);
        }
    }
    public override void generateOutside(List<Rectangle> corridors, Rectangle rectInside, Rectangle rectOutside) {
        WorldGenerator wGen = Game.Instance.GetComponent<WorldGenerator>();
        for (int x = outerRect.X; x < outerRect.X + outerRect.Width; x++) {
            for (int y = outerRect.Y; y < outerRect.Y + outerRect.Height; y++) {
                GameObject spawnedObject;
                
                Vector2 pos = new Vector2(x, y);
                if (WorldGenerator.IsPosInRectangle(pos, innerRect)) { // in room
                    //spawnedObject = wGen.CreateGrassGround(pos, 0.7, 0.05, 0.25, 0.0);
                } else { // outside room

                    if (VirtualGenRoom.IsCloserToThan(pos, corridors, "XY", 0)) { // in corridor
                        spawnedObject = wGen.CreateGrassGround(pos, 0.25, 0.05, 0.7);
                    } else { // in wall
                        if (VirtualGenRoom.IsCloserToThan(pos, innerRect, "XY", 1) || VirtualGenRoom.IsCloserToThan(pos, corridors, "XY", 1)) {
                            wGen.CreateColliderTile(pos);
                        }

                        if (!VirtualGenRoom.IsCloserToThan(pos, rectInside, "XY", 1)) {
                            wGen.CreateForest(pos, true);
                        }
                    }
                }
            }
        }
    }
}