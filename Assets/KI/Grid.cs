using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid<T>
{
    public int widht;

    public int height;

    public T[,] gridArray;
   
   public float cellSize;

   public Vector3 originPosition;

   public Grid(int widht, int height, float cellSize, Vector3 originPosition)
   {
       this.widht=widht;
       this.height=height;
       gridArray=new T[widht,height];
        this.cellSize=cellSize;
        this.originPosition=originPosition;
   }
   public Vector3 getWorldPosition(int x, int y)
   {
       return new Vector3(x,y)*cellSize+originPosition;
   }
   public int[] getXY(Vector3 worldPosition)
   {
       int[] result=new int[2];
       result[0]=Mathf.FloorToInt((worldPosition-originPosition).x/cellSize);
       result[1]=Mathf.FloorToInt((worldPosition-originPosition).y/cellSize);
       return result;
   }
   public T getValue(int x, int y)
   {
       return gridArray[x,y];
   }
   public void setValue(int x, int y, T value)
   {
       gridArray[x,y]=value;
   }
}
