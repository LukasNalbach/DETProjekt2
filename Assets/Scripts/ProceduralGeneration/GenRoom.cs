using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GenRoom {
    protected System.Random random;
    public Rectangle outerRect, corridor;
    protected Divider divider;
    protected GenRoom parentRoom, leftSubroom, rightSubroom;
    private int subroomCount = 0;

    public GenRoom() {
        random = new System.Random();
    }

    public GenRoom getLeftSubroom() {
        return leftSubroom;
    }

    public GenRoom getRightSubroom() {
        return leftSubroom;
    }

    public void setParentRoom(GenRoom GenRoom) {
        parentRoom = GenRoom;
    }

    public void setLeftSubRoom(GenRoom GenRoom) {
        leftSubroom = GenRoom;
    }

    public void setRightSubRoom(GenRoom GenRoom) {
        rightSubroom = GenRoom;
    }

    public int getSubroomCount() {
        return (leftSubroom != null ? 2 : 0) + leftSubroom.getSubroomCount() + rightSubroom.getSubroomCount();
    }
    public List<GenRoom> getSubrooms() {
        List<GenRoom> subRooms = new List<GenRoom>();
        if (leftSubroom != null) {
            subRooms.AddRange(leftSubroom.getSubrooms());
            subRooms.AddRange(rightSubroom.getSubrooms());
        }
        return subRooms;
    }

    public static GenRoom joinRooms(GenRoom leftSubroom, GenRoom rightSubroom) {
        GenRoom newRoot = new GenRoom();
        
        newRoot.setLeftSubRoom(leftSubroom);
        newRoot.setRightSubRoom(rightSubroom);

        leftSubroom.setParentRoom(newRoot);
        rightSubroom.setParentRoom(newRoot);

        return newRoot;
    }

    public void updateSubroom(GenRoom oldSubroom, GenRoom newSubroom) {
        if (leftSubroom == oldSubroom) {
            leftSubroom = newSubroom;
        } else if (rightSubroom == oldSubroom) {
            rightSubroom = newSubroom;
        } else {
            Debug.Log("no such room found");
        }
    }

    public GenRoom addSubroom(RealGenRoom newRoom) {
        if (leftSubroom != null) {
            GenRoom oldParent = parentRoom;

            GenRoom newRoot = joinRooms(this, newRoom);

            oldParent.updateSubroom(this, newRoot);
            newRoot.setParentRoom(oldParent);

            return newRoot;
        } else {
            int subroomsInLeftSubroom = leftSubroom.getSubroomCount();
            int subroomsInRightSubroom = rightSubroom.getSubroomCount();
            if (subroomsInLeftSubroom >= subroomsInRightSubroom) {
                return leftSubroom.addSubroom(newRoom);
            } else {
                return rightSubroom.addSubroom(newRoom);
            }
        }
    }

    public List<Rectangle> getRects() {
        List<Rectangle> rects = new List<Rectangle>();

        rects.Add(corridor);

        if (leftSubroom != null) {
            rects.AddRange(leftSubroom.getRects());
            rects.AddRange(rightSubroom.getRects());
        }

        return rects;
    }

    public void generate(Rectangle outerRect) {
        generate(outerRect, random.NextDouble() > 0.5 ? DividerType.Horizontal : DividerType.Vertical);
    }

    protected void generate(Rectangle outerRect, DividerType dividerType) {
        this.outerRect = outerRect;
        int dividerPos;

        DividerType nextDividersType = dividerType == DividerType.Horizontal ? DividerType.Vertical : DividerType.Horizontal;
        Rectangle leftOuterRect, rightOuterRect, tmpLeftOuterRect, tmpRightOuterRect;

        if (dividerType == DividerType.Vertical) {
            tmpLeftOuterRect = leftOuterRect;
            tmpRightOuterRect = rightOuterRect;

            leftOuterRect = new Rectangle(leftOuterRect.Y, leftOuterRect.X, leftOuterRect.Height, leftOuterRect.Width);
            rightOuterRect = new Rectangle(rightOuterRect.Y, rightOuterRect.X, rightOuterRect.Height, rightOuterRect.Width);
        }

        dividerPos = (int) (outerRect.X + outerRect.Width * (0.35 + 0.3 * random.NextDouble()));

        leftOuterRect = new Rectangle(outerRect.X, outerRect.Y, dividerPos - outerRect.X, outerRect.Height);
        rightOuterRect = new Rectangle(dividerPos, outerRect.Y, outerRect.X + outerRect.Width - dividerPos, outerRect.Height);

        if (dividerType == DividerType.Vertical) {
            leftOuterRect = tmpLeftOuterRect;
            rightOuterRect = tmpRightOuterRect;
        }

        divider = new Divider(dividerType, dividerPos);

        leftSubroom.generate(leftOuterRect, nextDividersType);
        rightSubroom.generate(rightOuterRect, nextDividersType);

        List<Rectangle> leftRects = leftSubroom.getRects();
        List<Rectangle> rightRects = rightSubroom.getRects();

        List<Rectangle> rightRectsOrig = rightRects.GetRange(0, rightRects.Count);

        int possibleCorrWidth, corrWidth, posX, posY, width, height;
        Rectangle tmpRectL, tmpRectR;

        {
            int lRand = random.Next(leftRects.Count);
            Rectangle rectRandL = leftRects[lRand];
            leftRects.Remove(rectRandL);

            {
                int rRand = random.Next(rightRects.Count);
                Rectangle rectRandR = rightRects[rRand];
                rightRects.Remove(rectRandR);

                Rectangle tmpRectSwap;

                if (dividerType == DividerType.Vertical) {
                    tmpRectL = rectRandL;
                    tmpRectR = rectRandR;

                    rectRandL = new Rectangle(rectRandL.Y, rectRandL.X, rectRandL.Height, rectRandL.Width);
                    rectRandR = new Rectangle(rectRandR.Y, rectRandR.X, rectRandR.Height, rectRandR.Width);
                }

                if (rectRandL.X < rectRandR.X || rectRandL.X > rectRandR.X + rectRandR.Width) {
                    tmpRectSwap = rectRandL;
                    rectRandL = rectRandR;
                    rectRandR = tmpRectSwap;
                }

                if (tmpRectSwap == null || rectRandL.X >= rectRandR.X && rectRandL.X <= rectRandR.X + rectRandR.Width) {
                    possibleCorrWidth = rectRandR.X + rectRandR.Width - rectRandL.X + 1;
                    corrWidth = random.Next(Math.Min(possibleCorrWidth, 3));

                    posX = rectRandL.X + (int) Math.Floor((double) corrWidth / 2) + random.Next(possibleCorrWidth - corrWidth);
                    width = posX + corrWidth;
                    if (rectRandL.Bottom > rectRandR.Top) {
                        posY = rectRandL.Bottom - 1;
                        height = rectRandL.Bottom - rectRandR.Top - 1;
                    } else {
                        posY = rectRandR.Bottom - 1;
                        height = rectRandR.Bottom - rectRandL.Top - 1;
                    }

                    corridor = new Rectangle(posX, posY, width, height);
                }

                if (tmpRectSwap != null) {
                    tmpRectSwap = rectRandL;
                    rectRandL = rectRandR;
                    rectRandR = tmpRectSwap;
                }

                if (dividerType == DividerType.Vertical) {
                    rectRandL = tmpRectL;
                    rectRandR = tmpRectR;
                    
                    corridor = new Rectangle(corridor.Y, corridor.X, corridor.Height, corridor.Width);
                }
            } while (corridor == null);

            rightRects = rightRectsOrig.GetRange(0, rightRects.Count);
        } while (corridor == null);
    }

    protected class Divider {
        public DividerType type;
        public int pos;
        public Divider(DividerType type, int pos) {
            this.type = type;
            this.pos = pos;
        }
    }

    protected enum DividerType {
        Horizontal,
        Vertical
    }
}