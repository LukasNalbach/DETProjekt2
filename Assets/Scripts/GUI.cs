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
        var textField=GameObject.Find("Canvas/KillCooldown");
        if(textField!=null)
        {
            textField.GetComponent<TextMeshProUGUI>().SetText("Kill Cooldown: " + newKillCooldown);
        }
    }

    public void updateTaskProgress(int newTaskProgressInDegree)
    {
         var textField=GameObject.Find("Canvas/TaskProgress");
        if(textField!=null)
        {
            textField.GetComponent<TextMeshProUGUI>().SetText("Task Progress: " + newTaskProgressInDegree+"%");
        }
    }

}
