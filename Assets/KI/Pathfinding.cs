using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
     private CrewMate crewMateScript;
    public LinkedList<Vector3> calculatedPoints=new LinkedList<Vector3>();//points from the actuall Room grid calculated with A* or next checkpoint
    public LinkedList<PathPoint> nextPoints;//checkpoints and goal, way to this point is not jet calculated with A*
    public Pathfinding(CrewMate crewMateScript)
    {
        this.crewMateScript=crewMateScript;
    }
    public void calculateNextTaskGoal()
    {
        if(crewMateScript.taskToDo.Count==0)
        {
            return;
        }
        Vector3 start=crewMateScript.transform.position;
        int shortestSize=int.MaxValue;
        List<PathNode> bestPath=new List<PathNode>();
        foreach(Task task in crewMateScript.taskToDo)
        {
            PathfindingRoom pathfindingRoom=new PathfindingRoom(Game.Instance.GetComponent<WorldGenerator>().mapGrid);
            List<PathNode> pathToGoal=pathfindingRoom.FindPath(start, task.transform.position);
            if(pathToGoal==null)
            {
                Debug.Log("No Path available to positon "+task.transform.position);
            }
            else
            {
                int sizeWay=pathToGoal.Count;
                Debug.Log("Size from "+start+" to "+task.transform.position+" has size "+sizeWay);
                if(sizeWay<shortestSize)
                {
                    shortestSize=sizeWay;
                    bestPath=pathToGoal;
                }
            }
        }
        foreach(PathNode pn in bestPath)
        {
            calculatedPoints.AddLast(Game.Instance.GetComponent<WorldGenerator>().mapGrid.getWorldPosition(pn.x,pn.y));
        }
    }
    /*public void calculateNextGoalForTasks()
    {
        nextPoints=dijstra(myPlayer.GetComponent<CrewMate>().taskToDo);//needs graph
        calculateNextPoints();
    }
    public void calculateNextPoints()
    {
        
        PathPoint nextGoal=nextPoints.First.Value;
        nextPoints.RemoveFirst();
        if(nextGoal.checkpoint)
        {
            //?
            calculatedPoints.AddLast(nextGoal.position);
        }
        else
        {
            Vector3 start=myPlayer.transform.position;
            Room nextRoom=nextGoal.nextRoom;
            PathfindingRoom pathfindingRoom=new PathfindingRoom(nextRoom.roomGrid);
            List<PathNode> pathToGoal=pathfindingRoom.FindPath(start, nextGoal.position);
            foreach(PathNode pn in pathToGoal)
            {
                calculatedPoints.AddLast(nextRoom.roomGrid.getWorldPosition(pn.x,pn.y));
            }
        }
    }*/
    public bool hasNextPosition()
    {
        return calculatedPoints.Count>0;
    }
    public Vector3 getNextPosition()
    {
        if(calculatedPoints.Count>0)
        {
            return calculatedPoints.First.Value;
        }
        throw new System.NotSupportedException("First check with hasNextPosition!");
    }
    public void reachNextPosition()
    {
        calculatedPoints.RemoveFirst();
    }
}
