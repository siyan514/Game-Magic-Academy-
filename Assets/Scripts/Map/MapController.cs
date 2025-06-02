using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    private int X, Y;
    private List<Vector2> emptyPointList = new List<Vector2>();
    private List<Vector2> superWallPointList = new List<Vector2>();
    public List<Vector2> accessiblePointList = new List<Vector2>();
    private Dictionary<ObjectType, List<GameObject>> poolObjectDic = new Dictionary<ObjectType, List<GameObject>>();

    /// <summary>
    /// Determine whether the current position is an actual wall
    /// </summary>
    /// <param name="pos">Current position</param>
    /// <returns></returns>
    public bool IsSuperWall(Vector2 pos)
    {
        Vector2 intPos = new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
        return superWallPointList.Contains(intPos);
    }

    /// <summary>
    /// By default, player 1 is located at the top left corner of the map, 
    /// and player 2 is located at the bottom right corner of the map.
    /// </summary>
    /// <param name="playerIndex"></param>
    /// <returns></returns>
    public Vector2 GetPlayerPos(int playerIndex)
    {
        if (playerIndex == 1) return new Vector2(-(X + 1), Y - 1);
        if (playerIndex == 2) return new Vector2(X - 1, -Y - 1);
        if (playerIndex == 3) return new Vector2(X - 1, Y - 1);
        if (playerIndex == 4) return new Vector2(-X - 1, -Y - 1);
        return new Vector2(1, 1);
    }

    public void initMap(int x, int y, int wallCount)
    {
        ResetMap();
        Y = y;
        X = x;
        createSuperWall();
        findEmptyPoint();
        CreateWall(wallCount);
        createProps();
    }

    /// <summary>
    /// Reset the map.
    /// </summary>
    private void ResetMap()
    {
        emptyPointList.Clear();
        superWallPointList.Clear();
        foreach (var item in poolObjectDic)
        {
            foreach (var obj in item.Value)
            {
                ObjectPool.instance.Add(item.Key, obj);
            }
        }
        poolObjectDic.Clear();
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
                spawnSuperWall(new Vector2(x, y));
            }
        }

        for (int x = -(X + 2); x <= X; x++)
        {
            spawnSuperWall(new Vector2(x, Y));
            spawnSuperWall(new Vector2(x, -Y - 2));
        }

        for (int y = -(Y + 1); y <= Y - 1; y++)
        {
            spawnSuperWall(new Vector2(-(X + 2), y));
            spawnSuperWall(new Vector2(X, y));
        }
    }

    private void spawnSuperWall(Vector2 pos)
    {
        Vector2 intPos = new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
        superWallPointList.Add(intPos);

        GameObject superWall = ObjectPool.instance.Get(ObjectType.SuperWall, pos);
        if (poolObjectDic.ContainsKey(ObjectType.SuperWall) == false)
        {
            poolObjectDic.Add(ObjectType.SuperWall, new List<GameObject>());
        }
        else
        {
            poolObjectDic[ObjectType.SuperWall].Add(superWall);
        }
    }

    /// <summary>
    /// find the empty point on the map
    /// </summary>
    private void findEmptyPoint()
    {
        for (int x = -(X + 1); x <= X - 1; x++)
        {
            if (-(X + 1) % 2 == x % 2)
            {
                for (int y = -(Y + 1); y <= Y - 1; y++)
                {
                    emptyPointList.Add(new Vector2(x, y));
                    accessiblePointList.Add(new Vector2(x, y));
                }
            }
            else
            {
                for (int y = -(Y + 1); y <= Y - 1; y += 2)
                {
                    emptyPointList.Add(new Vector2(x, y));
                    accessiblePointList.Add(new Vector2(x, y));
                }
            }
        }
        emptyPointList.Remove(new Vector2(-(X + 1), Y - 1));
        emptyPointList.Remove(new Vector2(-(X + 1), Y - 2));
        emptyPointList.Remove(new Vector2(-X, Y - 1));

        emptyPointList.Remove(new Vector2(X - 2, -Y - 1));
        emptyPointList.Remove(new Vector2(X - 1, -Y - 1));
        emptyPointList.Remove(new Vector2(X - 1, -Y));

        emptyPointList.Remove(new Vector2(X - 1, Y - 1));
        emptyPointList.Remove(new Vector2(X - 1, Y - 2));
        emptyPointList.Remove(new Vector2(X - 2, Y - 1));

        emptyPointList.Remove(new Vector2(-X - 1, -Y - 1));
        emptyPointList.Remove(new Vector2(-X - 1, -Y));
        emptyPointList.Remove(new Vector2(-X, -Y - 1));
    }

    /// <summary>
    /// create the obstacles that can be destroyed
    /// </summary>
    private void CreateWall(int wallCount)
    {
        if (wallCount >= emptyPointList.Count)
        {
            wallCount = (int)(emptyPointList.Count * 0.7f);
        }
        for (int i = 0; i < wallCount; i++)
        {
            int index = Random.Range(0, emptyPointList.Count);
            GameObject wall = ObjectPool.instance.Get(ObjectType.Wall, emptyPointList[index]);
            emptyPointList.RemoveAt(index);
            if (poolObjectDic.ContainsKey(ObjectType.Wall) == false)
            {
                poolObjectDic.Add(ObjectType.Wall, new List<GameObject>());
            }
            else
            {
                poolObjectDic[ObjectType.Wall].Add(wall);
            }
        }
    }

    /// <summary>
    /// generate prop
    /// </summary>
    private void createProps()
    {
        int count = Random.Range(5, 10 + (int)(emptyPointList.Count * 0.05f));
        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, emptyPointList.Count);
            GameObject prop = ObjectPool.instance.Get(ObjectType.Prop, emptyPointList[index]);
            emptyPointList.RemoveAt(index);
            if (poolObjectDic.ContainsKey(ObjectType.Prop) == false)
            {
                poolObjectDic.Add(ObjectType.Prop, new List<GameObject>());
            }
            else
            {
                poolObjectDic[ObjectType.Prop].Add(prop);
            }
        }
    }
}
