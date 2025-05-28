using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public GameObject superWallPre, wallPre, propPre;
    private int X, Y;
    private List<Vector2> emptyPointList = new List<Vector2>();
    private List<Vector2> superWallPointList = new List<Vector2>();

    /// <summary>
    /// Determine whether the current position is an actual wall
    /// </summary>
    /// <param name="pos">Current position</param>
    /// <returns></returns>
    public bool IsSuperWall(Vector2 pos)
    {
        if(superWallPointList.Contains(pos)) 
            return true;
        return false;
    }

    public Vector2 GetPlayerPos()
    {
        return new Vector2(-(X + 1), Y - 1);
    }

    public void initMap(int x, int y, int wallCount)
    {
        Y = y;
        X = x;
        createSuperWall();
        findEmptyPoint();
        Debug.Log(emptyPointList.Count);
        CreateWall(wallCount);
        createProps();
    }

    /// <summary>
    /// generate the super wall that cannot be destroyed
    /// </summary>
    private void createSuperWall()
    {
        for (int x = -X; x < X; x += 2)
        {
            for (int y = -Y; y < Y; y += 2)
            {
                spawnSuperWall(new Vector2(x,y));
            }
        }

        for (int x = -(X + 2); x <= X; x++)
        {
            spawnSuperWall(new Vector2(x, Y));
            spawnSuperWall(new Vector2(x, -Y - 2));
        }

        for (int y = -(Y + 1); y <= Y-1; y++)
        {
            spawnSuperWall(new Vector2(-(X + 2), y));
            spawnSuperWall(new Vector2(X, y));
        }
    }

    private void spawnSuperWall(Vector2 pos)
    {
        superWallPointList.Add(pos);
        GameObject wall = Instantiate(superWallPre, transform);
        wall.transform.position = pos;
    }

    /// <summary>
    /// find the empty point on the map
    /// </summary>
    private void findEmptyPoint()
    {
        for(int x =  -(X + 1); x <= X - 1; x++)
        {
            if(-(X + 1) % 2 == x % 2)
            {
                for (int y = -(Y + 1); y <= Y - 1; y++)
                {
                    emptyPointList.Add(new Vector2(x, y));
                }
            }
            else
            {
                for (int y = -(Y + 1); y <= Y - 1; y += 2)
                {
                    emptyPointList.Add(new Vector2(x, y));
                }
            }
        }
        emptyPointList.Remove(new Vector2(-(X+1),Y-1));
        emptyPointList.Remove(new Vector2(-(X+1),Y-2));
        emptyPointList.Remove(new Vector2(-X,Y-1));
    }

    /// <summary>
    /// create the obstacles that can be destroyed
    /// </summary>
    private void CreateWall(int wallCount)
    {
        if(wallCount >= emptyPointList.Count)
        {
            wallCount = (int)(emptyPointList.Count*0.7f);
        }
        for(int i = 0; i < wallCount; i++)
        {
            int index = Random.Range(0, emptyPointList.Count);
            GameObject wall = Instantiate(wallPre, transform);
            wall.transform.position = emptyPointList[index];

            emptyPointList.RemoveAt(index);
        }
    }

    /// <summary>
    /// generate prop
    /// </summary>
    private void createProps()
    {
        int count = Random.Range(0, 2 + (int)(emptyPointList.Count*0.05f));
        for (int i = 0;i < count;i++)
        {
            GameObject prop = Instantiate(propPre, transform);
            int index = Random.Range(0, emptyPointList.Count);
            prop.transform.position = emptyPointList[index];

            emptyPointList.RemoveAt(index);
        }
    }
}
