using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GUI 
{
    public void updateRoom(int roomNr)
    {
        GameObject.Find("Canvas/currentRoom").GetComponent<TextMeshProUGUI>().SetText("Room: " + roomNr);
    }

    public void updateKillCooldown(int newKillCooldown)
    {
        GameObject.Find("Canvas/KillCooldown").GetComponent<TextMeshProUGUI>().SetText("Kill Cooldown: " + newKillCooldown);
    }

    public void updateTaskProgress(int newTaskProgressInDegree)
    {
        GameObject.Find("Canvas/TaskProgress").GetComponent<TextMeshProUGUI>().SetText("Task Progress: " + newTaskProgressInDegree+"%");

    }

}
