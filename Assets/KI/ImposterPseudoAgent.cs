using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class ImposterPseudoAgent : CrewMatePseudoAgent
{
    public float kill;
    public float vent;
    public float changeVent;
    private bool ventUsed=false;
    private float timeStampLastUsedVent;
    private float timeStampLastChase;
    private Task nextFakeTask=null;
    private Player chasedCrewMate=null;
    private int crewMatesInView;
    int imposterMode =0;//0: fake Task and wait for opportunity, 1: chase CrewMate
    void Start()
    {
        playerScript=GetComponent<Imposter>();
         pathfinding=new Pathfinding(playerScript);
         greenSquarePrefab=AssetDatabase.LoadAssetAtPath("Assets/Prefabs/GreenSquare.prefab", typeof(GameObject)) as GameObject;
         redSquarePrefab=AssetDatabase.LoadAssetAtPath("Assets/Prefabs/RedSquare.prefab", typeof(GameObject)) as GameObject;
        timeStampLastUsedVent=time;
        doingTask=0;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerScript.activePlayer())
        {
            movement=new Vector2(0,0);
            movement[0] = Input.GetAxis("Horizontal");
            movement[1] = Input.GetAxis("Vertical");
            report = Input.GetKeyDown(KeyCode.Space)?1f:0;
            kill = Input.GetKeyDown(KeyCode.Return)?1f:0;
            vent = Input.GetKeyDown(KeyCode.V)?1f:0;
            changeVent=Input.GetKeyDown(KeyCode.Tab)?1f:0;
        }
    }
    void FixedUpdate()
    {
        if(!playerScript.activePlayer())
        {
            crewMatesInView=calculateCrewMatesInView();
            movement=calculateImposterMovement();
            report=0;
            kill=calculateKill();
            vent=calculateVent();
            changeVent=calculateChangeVent();
        }
    }
    Vector2 calculateImposterMovement()
    {
        if(crewMatesInView==1&&imposterMode!=1&&Game.Instance.killCooldown<=5f&&time-timeStampLastChase<=20f)
        {
            Debug.Log("Start chasing crewMate");
            imposterMode=1;
            timeStampLastChase=time;
            clearCalculatedPoints();
        }
        if(imposterMode==0)
        {
            //from crew Mate Agent
            return calculateMovement();
        }
        if(imposterMode==1)
        {
            if(crewMatesInView!=1||Game.Instance.killCooldown>5f)
            {
                chasedCrewMate=null;
                imposterMode=0;
                return new Vector3(0,0,0);
            }
            if(chasedCrewMate==null)
            {
                foreach(Player player in playerScript.playerInViewDistance)
                {
                    if(!player.isImposter())
                    {
                        chasedCrewMate=player;
                    }
                }
            }
            if(chasedCrewMate==null||!playerScript.playerInViewDistance.Contains(chasedCrewMate))
            {
                chasedCrewMate=null;
                imposterMode=0;
                return new Vector3(0,0,0);
            }
            if(pathfinding.calculatedPoints.Count>0&&
            Vector3.Distance(chasedCrewMate.transform.position, pathfinding.calculatedPoints.Last.Value)>=3f)
            {
                clearCalculatedPoints();
                calculateNextTaskGoal();
            }
            return calculateMovement();
        }
        return new Vector2(0,0);
    }
    float calculateKill()
    {
        return crewMatesInView==1?1.0f:0;
    }
    float calculateVent()
    {
        if(time-timeStampLastUsedVent>=10f&&((Imposter)playerScript).nearestVent()!=null)
        {
            if(crewMatesInView==0)
            {
                ventUsed=false;
                timeStampLastUsedVent=time;
                return 1f;
            }
        }
        if(((Imposter)playerScript).inVent()&&ventUsed)
        {
            if(crewMatesInView==0)
            {
                return 1f;
            }
        }
        return 0f;
    }
    float calculateChangeVent()
    {
        if(!ventUsed)
        {
            ventUsed=true;
            return 1f;
        }
        return 0f;
    }
    private int calculateCrewMatesInView()
    {
        int crewMatesInView=0;
        foreach(Player player in playerScript.playerInViewDistance)
        {
            if(!player.isImposter()&&player.isAlive())
            {
                crewMatesInView++;
            }
        }
        return crewMatesInView;
    }
    public override void calculateNextTaskGoal()
    {
        if(chasedCrewMate!=null)
        {
            pathfinding.calculatePath(chasedCrewMate.transform.position);
            Debug.Log("Calculate Path to CM"+chasedCrewMate.number);
        }
        else if(Game.Instance.activeSabortage!=null)
        {
            pathfinding.calculateSabortageGoals();
        }
        else
        {
            List<Task>goals=new List<Task>();
            if(nextFakeTask!=null&&Vector3.Distance(nextFakeTask.transform.position,playerScript.transform.position)<=5f)
            {
                nextFakeTask=null;
            }
            if(nextFakeTask==null)
            {
                Game.Shuffle(Game.Instance.allTasks,Game.Instance.random);
                foreach(Task task in Game.Instance.allTasks)
                {
                    if(Vector3.Distance(playerScript.transform.position,task.transform.position)>15f)
                    {
                        nextFakeTask=task;
                        break;
                    }
                }
                if(nextFakeTask==null)
                {
                    nextFakeTask=Game.Instance.allTasks[0];
                }
            }
            goals.Add(nextFakeTask);
            pathfinding.calculateNextTaskGoal(goals);
        }
        timeStampLastReachedGoal=time;
        if(Game.Instance.gridVisible)
        {
            foreach(Vector3 pathNodePosition in pathfinding.calculatedPoints)
            {
                GameObject greenSquare=Instantiate(greenSquarePrefab, pathNodePosition, new Quaternion());
                grennSquaresCurrentPath.Add(greenSquare);
            }
        }
        
    }

}
