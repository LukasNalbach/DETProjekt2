using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
     private Player playerScript;
    public LinkedList<Vector3> calculatedPoints=new LinkedList<Vector3>();//points from the actuall Room grid calculated with A* or next checkpoint
    public LinkedList<PathPoint> nextPoints;//checkpoints and goal, way to this point is not jet calculated with A*
    public Pathfinding(Player playerScript)
    {
        this.playerScript=playerScript;
    }
    public void calculateNextTaskGoal(List<Task> goals)
    {
        if(goals.Count==0)
        {
            return;
        }
        Vector3 start=playerScript.transform.position;
        int shortestSize=int.MaxValue;
        List<PathNode> bestPath=new List<PathNode>();
        foreach(Task task in goals)
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
                //Debug.Log("Size from "+start+" to "+task.transform.position+" has size "+sizeWay);
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
    public void calculateSabortageGoals()
    {
        if(Game.Instance.allActiveSabortageTasks().Count==0)
        {
            Debug.Log("No Sabortage Tasks availible");
        }
        Vector3 start=playerScript.transform.position;
        Vector3 firstGoal=Game.Instance.allActiveSabortageTasks()[0].transform.position;
        PathfindingRoom pathfindingRoom=new PathfindingRoom(Game.Instance.GetComponent<WorldGenerator>().mapGrid);
        List<PathNode> pathToFirst=pathfindingRoom.FindPath(start, firstGoal);
        if(pathToFirst==null)
        {
            Debug.Log("No Path to Sabortage Task found");
            return;
        }
        foreach(PathNode pn in pathToFirst)
        {
            calculatedPoints.AddLast(Game.Instance.GetComponent<WorldGenerator>().mapGrid.getWorldPosition(pn.x,pn.y));
        }
        if(Game.Instance.allActiveSabortageTasks().Count==2)
        {
            Vector3 secoundGoal=Game.Instance.allActiveSabortageTasks()[1].transform.position;
            List<PathNode> pathToSecound=pathfindingRoom.FindPath(start, secoundGoal);
             if(pathToSecound==null)
            {
                Debug.Log("No Path to Sabortage Task found");
                return;
            }
            foreach(PathNode pn in pathToSecound)
            {
                calculatedPoints.AddLast(Game.Instance.GetComponent<WorldGenerator>().mapGrid.getWorldPosition(pn.x,pn.y));
            }
        }
        
    }
    public void calculatePath(Vector3 goal)
    {
         Vector3 start=playerScript.transform.position;
        Vector3 firstGoal=goal;
        PathfindingRoom pathfindingRoom=new PathfindingRoom(Game.Instance.GetComponent<WorldGenerator>().mapGrid);
        List<PathNode> pathToFirst=pathfindingRoom.FindPath(start, goal);
        if(pathToFirst==null)
        {
            Debug.Log("No Path to Goal found");
            return;
        }
        foreach(PathNode pn in pathToFirst)
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
        //Debug.Log("Reach point "+calculatedPoints.First.Value);
        calculatedPoints.RemoveFirst();
    }
    public void clearCalculatedPoints()
    {
        calculatedPoints.Clear();
    }
}
