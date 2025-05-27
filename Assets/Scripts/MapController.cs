using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
     public GameObject superWallPre;
     private int X, Y;

    private void Awake()
    {
        initMap(8, 9);
    }

    public void initMap(int x, int y)
    {
        Y = y;
        X = x;
        createSuperWall();
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
            GameObject wall = Instantiate(superWallPre, transform);
            wall.transform.position = new Vector2(x, y);
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

            for(int y = -(Y + 1); y <= Y-1; y++)
            {

            }
        }
    }
}
