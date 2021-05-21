using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Room : MonoBehaviour
{
    public static List<Room> rooms = new List<Room>();

    private int roomNum {get; set;}

    private Task[] tasks = new Task[2];

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateRoom(int roomNum, Task task1, Task task2) {
        this.roomNum = roomNum;
        tasks[0] = task1;
        tasks[1] = task2;
        rooms.Add(this);
    }

    public static Room getRoom(int i) {
        return rooms[i-1];
    }

    public static List<Room> getRooms() {
        return rooms;
    }

    public int getRoomNum() {
        return roomNum;
    }

    public Task[] getTasks() {
        return tasks;
    }

    public Task getTask(int i) {
        return tasks[i-1];
    }
}