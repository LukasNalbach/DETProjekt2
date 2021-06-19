using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class GenRoom {
    protected System.Random random;
    public Rectangle outerRect;
    protected GenRoom leftSubroom, rightSubroom;

    public GenRoom() {

    }

    public void setRandom(System.Random random) {
        this.random = random;
    }

    public GenRoom getLeftSubroom() {
        return leftSubroom;
    }

    public GenRoom getRightSubroom() {
        return leftSubroom;
    }

    public void setLeftSubRoom(GenRoom GenRoom) {
        leftSubroom = GenRoom;
    }

    public void setRightSubRoom(GenRoom GenRoom) {
        rightSubroom = GenRoom;
    }

    public abstract int getRoomCount();

    public int getDepth() {
        if (leftSubroom == null) return 1;
        return 1 + Math.Max(leftSubroom.getDepth(), rightSubroom.getDepth());
    }

    public bool isBalanced() {
        return getDepth() == 1 || Math.Abs(leftSubroom.getDepth() - rightSubroom.getDepth()) <= 1 && leftSubroom.isBalanced() && rightSubroom.isBalanced();
    }
    public List<GenRoom> getSubrooms() {
        List<GenRoom> subRooms = new List<GenRoom>();
        subRooms.Add(this);
        if (leftSubroom != null) {
            subRooms.AddRange(leftSubroom.getSubrooms());
            subRooms.AddRange(rightSubroom.getSubrooms());
        }
        return subRooms;
    }

    public static VirtualGenRoom joinRooms(GenRoom leftSubroom, GenRoom rightSubroom) {
        VirtualGenRoom newRoot = new VirtualGenRoom();
        
        newRoot.setLeftSubRoom(leftSubroom);
        newRoot.setRightSubRoom(rightSubroom);

        return newRoot;
    }

    public GenRoom addSubroom(GenRoom newRoom) {
        if (leftSubroom == null) {
            return joinRooms(this, newRoom);
        } else {
            int depthLeft = leftSubroom.getDepth();
            int depthRight = rightSubroom.getDepth();

            if (isBalanced()) {
                if (depthLeft <= depthRight) {
                    leftSubroom = leftSubroom.addSubroom(newRoom);
                } else {
                    rightSubroom = rightSubroom.addSubroom(newRoom);
                }
            } else {
                bool balancedLeft = leftSubroom.isBalanced();
                bool balancedRight = rightSubroom.isBalanced();

                if (balancedLeft) {
                    if (balancedRight) {
                        if (depthLeft <= depthRight) {
                            leftSubroom = leftSubroom.addSubroom(newRoom);
                        } else {
                            rightSubroom = rightSubroom.addSubroom(newRoom);
                        }
                    } else {
                        rightSubroom = rightSubroom.addSubroom(newRoom);
                    }
                } else {
                    if (balancedRight) {
                        leftSubroom = leftSubroom.addSubroom(newRoom);
                    } else {
                        if (depthLeft <= depthRight) {
                            leftSubroom = leftSubroom.addSubroom(newRoom);
                        } else {
                            rightSubroom = rightSubroom.addSubroom(newRoom);
                        }
                    }
                }
            }
            return this;
        }
    }

    public abstract List<Rectangle> getRects();

    public class Divider {
        public DividerType type;
        public int pos;
        public Divider(DividerType type, int pos) {
            this.type = type;
            this.pos = pos;
        }
    }

    public enum DividerType {
        Horizontal,
        Vertical
    }
}