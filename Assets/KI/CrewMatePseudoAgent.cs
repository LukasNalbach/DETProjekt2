using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class CrewMatePseudoAgent : PseudoAgent
{
   
     public Player playerScript;
     public Pathfinding pathfinding;
    public int mode=0;//0: normal movement, 1: just x movement, 2: just y movement
    public GameObject greenSquarePrefab;
    public GameObject redSquarePrefab;
    public float timeStampLastReachedGoal=0f;
    public List<GameObject>grennSquaresCurrentPath=new List<GameObject>();
    void Awake()
    {
        playerScript=GetComponent<CrewMate>();
         pathfinding=new Pathfinding(playerScript);
         greenSquarePrefab=AssetDatabase.LoadAssetAtPath("Assets/Prefabs/GreenSquare.prefab", typeof(GameObject)) as GameObject;
         redSquarePrefab=AssetDatabase.LoadAssetAtPath("Assets/Prefabs/RedSquare.prefab", typeof(GameObject)) as GameObject;
    }
    void FixedUpdate()
    {
        if(playerScript.activePlayer())
        {
            movement=new Vector2(0,0);
            movement[0] = Input.GetAxis("Horizontal");
            movement[1] = Input.GetAxis("Vertical");
            doingTask = Input.GetKey(KeyCode.Return)?1f:0;
            report = Input.GetKey(KeyCode.Space)?1f:0;
        }
        else if(!playerScript.isAlive())
        {
            movement=new Vector3(0,0,0);
            doingTask=0;
            report=0;
        }
        else{
            movement=calculateMovement();
            doingTask=1;
            report=1;
        }
    }
    public Vector3 calculateMovement()
    {
        if(playerScript.immobile())
        {
            return new Vector3(0,0,0);
        }
        if(mode==0)
        {
            return calculateNormalMovement();
        }
        else
        {
            return calculate1DMovement();
        }
    }
    public Vector3 calculateNormalMovement()
    {
        Vector3 nextStep=new Vector3(0,0,0);
        if(pathfinding.hasNextPosition()&&time-timeStampLastReachedGoal<1f)
        {
            nextStep= pathfinding.getNextPosition();
        }
        else
        {
            if(pathfinding.hasNextPosition())
            {
                //Debug.Log("Problem with node at position "+pathfinding.getNextPosition()+ ", Player positon: "+crewMateScript.transform.position);
                if(Game.Instance.gridVisible)
                {
                    GameObject redSquare=Instantiate(redSquarePrefab, pathfinding.getNextPosition(), new Quaternion());
                    redSquare.AddComponent<Destruction>();
                }
                mode=1;
                timeStampLastReachedGoal=time;
                return new Vector3(0,0,0);
            }
            calculateNextTaskGoal();
            if(pathfinding.hasNextPosition())
            {
                nextStep= pathfinding.getNextPosition();
            }
            else
            {
                return new Vector3(0,0,0);
            }
        }
        if(Vector3.Distance(playerScript.transform.position,nextStep)<=0.5f)
        {
            pathfinding.reachNextPosition();
            if(Game.Instance.gridVisible)
            {
                Destroy(grennSquaresCurrentPath[0]);
                grennSquaresCurrentPath.RemoveAt(0);
            }
            timeStampLastReachedGoal=time;
            if(pathfinding.hasNextPosition())
            {
                nextStep= pathfinding.getNextPosition();
            }
            else
            {
                return new Vector3(0,0,0);
            }
        }
        Vector3 movement=(nextStep-playerScript.transform.position);
        //Debug.Log(movement);
        return movement;
    }
    public Vector3 calculate1DMovement()
    {
        //mode!=0
        if(!pathfinding.hasNextPosition())
        {
            mode=0;
            return new Vector3(0,0,0);
        }
        if(time-timeStampLastReachedGoal>1f)
        {
            timeStampLastReachedGoal=time;
            if(mode==1)
            {
                //Debug.Log("X Movement failed, start Y movement");
                //crewMateScript.StartCoroutine(crewMateScript.coWaiting(5f));
                mode=2;
                return new Vector3(0,0,0);
            }
            else
            {
                //Debug.Log("1 D Movement failed");
                //crewMateScript.StartCoroutine(crewMateScript.coWaiting(5f));
                playerScript.StartCoroutine(coBlackMarkingGrid(pathfinding.getNextPosition()));
                mode=0;
                clearCalculatedPoints();
                return calculateNormalMovement();
            }
        }
        Vector3 nextStep=pathfinding.getNextPosition();
        if(mode==1||mode==3)
        {
            if(Vector3.Distance(playerScript.transform.position,nextStep)<=0.2f)
            {
                //Debug.Log("Goal reached just with X");
                mode=0;
                timeStampLastReachedGoal=time;
                return new Vector3(0,0,0);
            }
            else if(Mathf.Abs(nextStep.x-playerScript.transform.position.x)<=0.1f)
            {
                if(mode==3)
                {
                    //Debug.Log("Mode 3 Failed, Calculat new Path");
                    playerScript.StartCoroutine(coBlackMarkingGrid(pathfinding.getNextPosition()));
                    mode=0;
                    clearCalculatedPoints();
                    return calculateNormalMovement();
                }
                else
                {
                    //Debug.Log("X Goal reached");
                    //crewMateScript.StartCoroutine(crewMateScript.coWaiting(5f));
                    mode=2;
                    timeStampLastReachedGoal=time;
                    return new Vector3(0,0,0);
                }
            }
            else
            {
                Vector3 movement=new Vector3(nextStep.x-playerScript.transform.position.x,0,0);
                //Debug.Log("X movement: "+movement);
                return movement;
            }
        }
        else //if(mode==2)
        {
            if(Vector3.Distance(playerScript.transform.position,nextStep)<=0.2f)
            {
                //Debug.Log("Goal reached with Y");
                mode=0;
                timeStampLastReachedGoal=time;
                return new Vector3(0,0,0);
            }
            else if(Mathf.Abs(nextStep.y-playerScript.transform.position.y)<=0.1f)
            {
                //Debug.Log("Y Goal reached");
                mode=3;
                timeStampLastReachedGoal=time;
                return new Vector3(0,0,0);
            }
            else
            {
                Vector3 movement=new Vector3(0,nextStep.y-playerScript.transform.position.y,0);
                //Debug.Log("Y movement: "+movement);
                return movement;
            }
        }
    }
    public virtual void calculateNextTaskGoal()
    {
        if(Game.Instance.activeSabortage!=null)
        {
            pathfinding.calculateSabortageGoals();
        }
        else
        {
            pathfinding.calculateNextTaskGoal(((CrewMate)playerScript).taskToDo);
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
    public void clearCalculatedPoints()
    {
        if(Game.Instance.gridVisible)
        {
            foreach(GameObject greenSquare in grennSquaresCurrentPath)
            {
                Destroy(greenSquare);
            }
            grennSquaresCurrentPath.Clear();
        }
        pathfinding.clearCalculatedPoints();
    }
    public IEnumerator coBlackMarkingGrid(Vector3 position)
    {
        //Debug.Log("Black Mark Grid at"+position);
         Grid<bool> worldGrid=Game.Instance.GetComponent<WorldGenerator>().mapGrid;
        int[] gridPosition=worldGrid.getXY(position);
        worldGrid.setValue(gridPosition[0], gridPosition[1],false);
        yield return new WaitForSeconds(5f);
        worldGrid.setValue(gridPosition[0], gridPosition[1],true);
    }
    public override void startSabortage()
    {
        clearCalculatedPoints();
        calculateNextTaskGoal();
        mode=0;
    }
    public override void stopSabortage()
    {
        clearCalculatedPoints();
        calculateNextTaskGoal();
        mode=0;
    }
    
}
