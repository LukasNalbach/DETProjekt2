using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class CreateMap : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject taskPrefab;
    public GameObject sabortagePrefab;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void updateMap()
    {
        Player activePlayer=Game.Instance.activePlayer();
        Vector3 activePlayerPos=activePlayer.transform.position;
        Instantiate(playerPrefab, activePlayerPos, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
        if(activePlayer.isImposter())
        {
            foreach(Sabortage sabortage in Game.Instance.allSabortages)
            {
                Instantiate(sabortage.startButton,sabortage.getPosition(),new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
            }
        }  
        else
        {
            CrewMate crewMate=(CrewMate)activePlayer;
            foreach(Task task in crewMate.taskToDo)
            {
                Instantiate(taskPrefab, task.transform.position, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
            }
        }
        foreach(Task sabortageTask in Game.Instance.allActiveSabortageTasks())
        {
            Instantiate(sabortagePrefab, sabortageTask.transform.position, new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
        }     
    }
}
