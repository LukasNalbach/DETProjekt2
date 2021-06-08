using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public abstract class RealGenRoom : GenRoom {
    public Rectangle innerRect;

    public RealGenRoom() : base() {
        
    }

    public new List<Rectangle> getRects() {
        List<Rectangle> rects = new List<Rectangle>();
        rects.Add(innerRect);
        return rects;
    }

    public new void generate(Rectangle outerRect) {}
    protected new void generate(Rectangle outerRect, DividerType dividerType) {
        int x, y, w, h;

        x = outerRect.X + random.Next(outerRect.Width / 4);
        y = outerRect.Y + random.Next(outerRect.Height / 4);
        w = random.Next(3 * outerRect.Width / 4);
        h = random.Next(3 * outerRect.Height / 4);

        innerRect = new Rectangle(x, y, w, h);
    }
}