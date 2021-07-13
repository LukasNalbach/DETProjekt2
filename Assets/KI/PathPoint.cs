using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPoint 
{
    public Vector3 position;
    public bool checkpoint;
    public Room nextRoom; 
    public PathPoint(Vector3 position, bool checkpoint, Room nextRoom)
    {
        this.position=position;
        this.checkpoint=checkpoint;
        this.nextRoom=nextRoom;
    }
}
