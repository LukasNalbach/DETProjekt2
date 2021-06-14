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

    public void generateInside(List<Rectangle> corridors, Rectangle rectInside, Rectangle rectOutside) {
        if (leftSubroom is RealGenRoom) {
            ((RealGenRoom) leftSubroom).generateInside(corridors, rectInside, rectOutside);
        } else {
            ((VirtualGenRoom) leftSubroom).generateInside(corridors, rectInside, rectOutside);
        }
        if (rightSubroom is RealGenRoom) {
            ((RealGenRoom) rightSubroom).generateInside(corridors, rectInside, rectOutside);
        } else {
            ((VirtualGenRoom) rightSubroom).generateInside(corridors, rectInside, rectOutside);
        }
    }

    public void generateOutside(List<Rectangle> corridors, Rectangle rectInside, Rectangle rectOutside) {
        if (leftSubroom is RealGenRoom) {
            ((RealGenRoom) leftSubroom).generateOutside(corridors, rectInside, rectOutside);
        } else {
            ((VirtualGenRoom) leftSubroom).generateOutside(corridors, rectInside, rectOutside);
        }
        if (rightSubroom is RealGenRoom) {
            ((RealGenRoom) rightSubroom).generateOutside(corridors, rectInside, rectOutside);
        } else {
            ((VirtualGenRoom) rightSubroom).generateOutside(corridors, rectInside, rectOutside);
        }
    }

    public override String ToString() {
        return "V (" + leftSubroom.GetType() + ") (" + rightSubroom.GetType() + ")";
    }

    public void generate(Rectangle newOuterRect) {
        if (newOuterRect.Width > newOuterRect.Height) {
            generate(newOuterRect, DividerType.Vertical);
        } else {
            generate(newOuterRect, DividerType.Horizontal);
        }
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

                if (rectL.X >= rectR.X) {
                    maxCorridorWidth = Math.Min((rectR.X + rectR.Width - rectL.X), rectL.Width);
                } else {
                    maxCorridorWidth = Math.Min((rectL.X + rectL.Width - rectR.X), rectR.Width);
                }

                if (CollidesWith(rectL, rectR, "X")) {
                    if (rectL.Y + rectL.Height >= rectR.Y) {
                        y = rectR.Y + rectR.Height;
                        h = rectL.Y - (rectR.Y + rectR.Height);
                    } else {
                        y = rectL.Y + rectL.Height;
                        h = rectR.Y - (rectL.Y + rectL.Height);
                    }

                    w = 2;
                    for (int i = 1; i <= maxCorridorWidth - (w + 1); i++) {
                        positions.Add(i);
                    }
                    found = false;

                    while (!found && positions.Count > 0) {
                        iPos = random.Next(positions.Count);
                        pos = positions[iPos];
                        positions.RemoveAt(iPos);

                        if (rectL.X >= rectR.X) {
                            x = rectL.X + pos;
                        } else {
                            x = rectR.X + pos;
                        }

                        corridor = new Rectangle(x, y, w, h);
                    
                        if (dividerType == DividerType.Vertical) {
                            corridor = Rotate(corridor);
                        }

                        if (!Touches2(corridor, otherRects, "XY") && !Touches2(corridor, corridors, "XY")) {
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

    public static Rectangle Rotate(Rectangle rect) {
        return new Rectangle(rect.Y, rect.X, rect.Height, rect.Width);
    }

    public static bool CollidesWith(Vector2 pos, Rectangle r2, String mode) {
        Rectangle r1 = new Rectangle((int) pos.x, (int) pos.y, 1, 1);
        return CollidesWith(r1, r2, mode);
    }

    public static bool CollidesWith(Vector2 pos, List<Rectangle> otherRects, String mode) {
        Rectangle rect = new Rectangle((int) pos.x, (int) pos.y, 1, 1);
        return CollidesWith(rect, otherRects, mode);
    }

    public static bool CollidesWith(Rectangle r1, Rectangle r2, String mode) {
        if (mode == "X") {
            return (
                (r1.X >= r2.X && r1.X < r2.X + r2.Width) ||
                (r1.X + r1.Width > r2.X && r1.X + r1.Width <= r2.X + r2.Width) ||
                (r1.X < r2.X && r1.X + r1.Width > r2.X + r2.Width)
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
        Rectangle r2_ = new Rectangle(r2.X - 1, r2.Y - 1, r2.Width + 2, r2.Height + 2);
        return CollidesWith(r1, r2_, mode);
    }

    public static bool Touches(Rectangle rect, List<Rectangle> otherRects, String mode) {
        foreach (Rectangle otherRect in otherRects) {
            if (Touches(rect, otherRect, mode)) {
                return true;
            }
        }
        return false;
    }

    public static bool Touches(Vector2 pos, Rectangle r2, String mode) {
        Rectangle r1 = new Rectangle((int) pos.x, (int) pos.y, 1, 1);
        return Touches(r1, r2, mode);
    }

    public static bool Touches(Vector2 pos, List<Rectangle> otherRects, String mode) {
        Rectangle rect = new Rectangle((int) pos.x, (int) pos.y, 1, 1);
        return Touches(rect, otherRects, mode);
    }

    public static bool Touches2(Rectangle r1, Rectangle r2, String mode) {
        Rectangle r2_ = new Rectangle(r2.X - 2, r2.Y - 2, r2.Width + 4, r2.Height + 4);
        return CollidesWith(r1, r2_, mode);
    }

    public static bool Touches2(Rectangle rect, List<Rectangle> otherRects, String mode) {
        foreach (Rectangle otherRect in otherRects) {
            if (Touches2(rect, otherRect, mode)) {
                return true;
            }
        }
        return false;
    }

    public static bool Touches2(Vector2 pos, Rectangle r2, String mode) {
        Rectangle r1 = new Rectangle((int) pos.x, (int) pos.y, 1, 1);
        return Touches2(r1, r2, mode);
    }

    public static bool Touches2(Vector2 pos, List<Rectangle> otherRects, String mode) {
        Rectangle rect = new Rectangle((int) pos.x, (int) pos.y, 1, 1);
        return Touches2(rect, otherRects, mode);
    }
}