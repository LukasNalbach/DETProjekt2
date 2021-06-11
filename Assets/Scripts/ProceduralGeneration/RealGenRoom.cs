using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public abstract class RealGenRoom : GenRoom {
    public Rectangle innerRect;

    public override List<Rectangle> getRects() {
        List<Rectangle> rects = new List<Rectangle>();
        rects.Add(innerRect);
        return rects;
    }

    public override String ToString() {
        return "R";
    }

    public void generate(Rectangle newOuterRect) {
        outerRect = newOuterRect;

        int x, y, w, h;

        x = (int) Math.Ceiling((double) outerRect.Width / 11 + random.Next(outerRect.Width / 11));
        y = (int) Math.Ceiling((double) outerRect.Height / 11 + random.Next(outerRect.Height / 11));
        w = (int) Math.Floor((double) 9 * outerRect.Width / 11 + random.Next(outerRect.Width / 11) - x);
        h = (int) Math.Floor((double) 9 * outerRect.Height / 11 + random.Next(outerRect.Height / 11) - y);

        innerRect = new Rectangle(outerRect.X + x, outerRect.Y + y, w, h);
    }

    public override int getRoomCount() {
        return 1;
    }
    public abstract void generateInside();
    public abstract void generateOutside();
}