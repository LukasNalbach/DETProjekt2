using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class VirtualGenRoom : GenRoom {
    public List<Rectangle> corridors = new List<Rectangle>();
    protected Divider divider;

    public override List<Rectangle> getRects() {
        List<Rectangle> rects = new List<Rectangle>();

        if (leftSubroom != null) {
            rects.AddRange(leftSubroom.getRects());
            rects.AddRange(rightSubroom.getRects());
        }

        rects.AddRange(corridors);

        return rects;
    }

    public void generateInside() {
        if (leftSubroom is RealGenRoom) {
            ((RealGenRoom) leftSubroom).generateInside();
        } else {
            ((VirtualGenRoom) leftSubroom).generateInside();
        }
        if (rightSubroom is RealGenRoom) {
            ((RealGenRoom) rightSubroom).generateInside();
        } else {
            ((VirtualGenRoom) rightSubroom).generateInside();
        }
    }

    public void generateOutside() {
        if (leftSubroom is RealGenRoom) {
            ((RealGenRoom) leftSubroom).generateOutside();
        } else {
            ((VirtualGenRoom) leftSubroom).generateOutside();
        }
        if (rightSubroom is RealGenRoom) {
            ((RealGenRoom) rightSubroom).generateOutside();
        } else {
            ((VirtualGenRoom) rightSubroom).generateOutside();
        }
    }

    public override String ToString() {
        return "V (" + leftSubroom.GetType() + ") (" + rightSubroom.GetType() + ")";
    }

    public void generate(Rectangle newOuterRect) {
        generate(newOuterRect, random.NextDouble() > 0.5 ? DividerType.Horizontal : DividerType.Vertical);
    }

    public void generate(Rectangle newOuterRect, DividerType dividerType) {
        outerRect = newOuterRect;
        int dividerPos;

        DividerType nextDividersType = dividerType == DividerType.Horizontal ? DividerType.Vertical : DividerType.Horizontal;
        Rectangle leftOuterRect, rightOuterRect, tmpOuterRect;

        if (dividerType == DividerType.Horizontal) {
            tmpOuterRect = outerRect;

            outerRect = Rotate(outerRect);
        }

        dividerPos = (int) (outerRect.X + outerRect.Width * (0.43 + 0.14 * random.NextDouble()));

        leftOuterRect = new Rectangle(outerRect.X, outerRect.Y, dividerPos - outerRect.X, outerRect.Height);
        rightOuterRect = new Rectangle(dividerPos, outerRect.Y, outerRect.X + outerRect.Width - dividerPos, outerRect.Height);

        if (dividerType == DividerType.Horizontal) {
            outerRect = tmpOuterRect;

            leftOuterRect = Rotate(leftOuterRect);
            rightOuterRect = Rotate(rightOuterRect);
        }

        divider = new Divider(dividerType, dividerPos);

        if (leftSubroom is VirtualGenRoom) {
            ((VirtualGenRoom) leftSubroom).generate(leftOuterRect, nextDividersType);
        } else {
            ((RealGenRoom) leftSubroom).generate(leftOuterRect);
        }
        if (rightSubroom is VirtualGenRoom) {
            ((VirtualGenRoom) rightSubroom).generate(rightOuterRect, nextDividersType);
        } else {
            ((RealGenRoom) rightSubroom).generate(rightOuterRect);
        }

        List<Rectangle> leftRectsOrig = leftSubroom.getRects();
        List<Rectangle> rightRectsOrig = rightSubroom.getRects();

        List<Rectangle> leftRects = new List<Rectangle>();
        List<Rectangle> rightRects = new List<Rectangle>();

        List<Rectangle> otherRects = new List<Rectangle>();
        otherRects.AddRange(leftRectsOrig);
        otherRects.AddRange(rightRectsOrig);

        int maxCorridorWidth, x, y, w, h, iPos, pos;
        Rectangle corridor, tmpRectL, tmpRectR, rectLOrig, rectROrig, tmpRectSwap;
        List<int> positions = new List<int>();
        bool found, swapped;

        leftRects.AddRange(leftRectsOrig);

        while (leftRects.Count > 0) {
            int iL = random.Next(leftRects.Count);
            Rectangle rectL = leftRects[iL];
            leftRects.RemoveAt(iL);
            rectLOrig = rectL;

            otherRects.Remove(rectLOrig);

            rightRects.AddRange(rightRectsOrig);

            while (rightRects.Count > 0) {
                int iR = random.Next(rightRects.Count);
                Rectangle rectR = rightRects[iR];
                rightRects.RemoveAt(iR);
                rectROrig = rectR;

                otherRects.Remove(rectROrig);

                if (dividerType == DividerType.Vertical) {
                    tmpRectL = rectL;
                    tmpRectR = rectR;
                    rectL = Rotate(rectL);
                    rectR = Rotate(rectR);
                }

                swapped = false;
                if (!CollidesWith(rectL, rectR, "X")) {
                    tmpRectSwap = rectL;
                    rectL = rectR;
                    rectR = tmpRectSwap;
                    swapped = true;
                }

                if (rectL.Left >= rectR.Left) {
                    maxCorridorWidth = Math.Min((rectR.Right - rectL.Left), rectL.Width);
                } else {
                    maxCorridorWidth = Math.Min((rectL.Right - rectR.Left), rectR.Width);
                }

                if (CollidesWith(rectL, rectR, "X")) {
                    if (rectL.Top >= rectR.Bottom) {
                        y = rectR.Bottom;
                        h = rectL.Top - rectR.Bottom;
                    } else {
                        y = rectL.Bottom;
                        h = rectR.Top - rectL.Bottom;
                    }

                    w = 2;
                    for (int i = 0; i <= maxCorridorWidth - w; i++) {
                        positions.Add(i);
                    }
                    found = false;

                    while (!found && positions.Count > 0) {
                        iPos = random.Next(positions.Count);
                        pos = positions[iPos];
                        positions.RemoveAt(iPos);

                        if (rectL.Left >= rectR.Left) {
                            x = rectL.Left + pos;
                        } else {
                            x = rectR.Left + pos;
                        }

                        if (dividerType == DividerType.Vertical) {
                            corridor = new Rectangle(y, x, h, w);
                        } else {
                            corridor = new Rectangle(x, y, w, h);
                        }

                        if (!Touches(corridor, otherRects, "XY") && !Touches(corridor, corridors, "XY")) {
                            corridors.Add(corridor);
                            found = true;
                        }
                    }
                    positions.Clear();
                }

                if (swapped) {
                    tmpRectSwap = rectL;
                    rectL = rectR;
                    rectR = tmpRectSwap;
                }

                if (dividerType == DividerType.Vertical) {
                    rectL = tmpRectL;
                    rectR = tmpRectR;
                }
                otherRects.Add(rectROrig);
            }
            otherRects.Add(rectLOrig);
        }

        int wantedCorridorAmount = Math.Min((int) Math.Ceiling((double) Math.Min(leftSubroom.getRoomCount(), rightSubroom.getRoomCount()) / 2), corridors.Count);
        Debug.Log(wantedCorridorAmount + " : " + getRoomCount());
        if (wantedCorridorAmount < corridors.Count) {
            corridors.Sort((r1, r2) => Math.Max(r1.Width, r1.Height) - Math.Max(r2.Width, r2.Height));
            corridors.RemoveRange(Math.Max(wantedCorridorAmount, 0), Math.Max(corridors.Count - wantedCorridorAmount, 0));
        }
    }

    public override int getRoomCount() {
        int n = 0;
        if (leftSubroom != null) {
            n += leftSubroom.getRoomCount() + rightSubroom.getRoomCount();
        }
        return n;
    }

    public static bool CollidesWith(Rectangle r1, Rectangle r2, String mode) {
        if (mode == "X") {
            return (
                (r1.Left >= r2.Left && r1.Left < r2.Right) ||
                (r1.Right > r2.Left && r1.Right <= r2.Right) ||
                (r1.Left < r2.Left && r1.Right > r2.Right)
            );
        } else if (mode == "Y") {
            return CollidesWith(Rotate(r1), Rotate(r2), "X");
        } else if (mode == "XY") {
            return CollidesWith(r1, r2, "X") && CollidesWith(r1, r2, "Y");
        } else {
            return false;
        }
    }

    public static bool CollidesWith(Rectangle rect, List<Rectangle> otherRects, String mode) {
        foreach (Rectangle otherRect in otherRects) {
            if (CollidesWith(rect, otherRect, mode)) {
                return true;
            }
        }
        return false;
    }

    public static bool Touches(Rectangle r1, Rectangle r2, String mode) {
        Rectangle r1_ = new Rectangle(r1.X - 1, r1.Y - 1, r1.Width + 2, r2.Height + 2);
        return CollidesWith(r1_, r2, mode);
    }

    public static bool Touches(Rectangle rect, List<Rectangle> otherRects, String mode) {
        foreach (Rectangle otherRect in otherRects) {
            if (Touches(rect, otherRect, mode)) {
                return true;
            }
        }
        return false;
    }

    public static Rectangle Rotate(Rectangle rect) {
        return new Rectangle(rect.Y, rect.X, rect.Height, rect.Width);
    }
}