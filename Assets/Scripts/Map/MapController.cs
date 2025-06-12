using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Map generation implementation class
/// </summary>
public class MapController : MonoBehaviour
{
    public Sprite[] wallSprites;
    public Sprite[] enemyWallSprites; 
    public Sprite[] propSprites;    

    public GameObject enemyWallPrefab; 
    [Range(0f, 1f)] public float enemyWallSpawnChance = 0.2f; 

    public GameObject enemyPrefab;


    private int X, Y;

    private int currentLevel = 0;
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

    public void initMap(int x, int y, int wallCount, int levelCount)
    {
        currentLevel = levelCount;
        ResetMap();
        Y = y;
        X = x;
        createFloor();
        createSuperWall();
        findEmptyPoint();
        CreateWall(wallCount);
        CreateEnemyWalls();
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
            // Create a new list to store the undestroyed objects
            List<GameObject> validObjects = new List<GameObject>();

            foreach (var obj in item.Value)
            {
                if (obj != null) // Check whether the object has not been destroyed
                {
                    validObjects.Add(obj);
                    ObjectPool.instance.Add(item.Key, obj);
                }
            }

            // Update the list in the dictionary
            item.Value.Clear();
            item.Value.AddRange(validObjects);
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
            spawnOuterWall(new Vector2(x, Y));
            spawnOuterWall(new Vector2(x, -Y - 2));
        }

        for (int y = -(Y + 1); y <= Y - 1; y++)
        {
            spawnOuterWall(new Vector2(-(X + 2), y));
            spawnOuterWall(new Vector2(X, y));
        }
    }
    /// <summary>
    /// spawn the super wall that cannot be destroyed
    /// </summary>
    /// <param name="pos"></param>
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

    private void spawnOuterWall(Vector2 pos)
    {
        Vector2 intPos = new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
        superWallPointList.Add(intPos);

        GameObject outerWall = ObjectPool.instance.Get(ObjectType.OuterWall, pos);
        if (poolObjectDic.ContainsKey(ObjectType.OuterWall) == false)
        {
            poolObjectDic.Add(ObjectType.OuterWall, new List<GameObject>());
        }
        else
        {
            poolObjectDic[ObjectType.OuterWall].Add(outerWall);
        }
    }
    /// <summary>
    /// create the floor
    /// </summary>
    private void createFloor()
    {
        for (int x = -(X + 1); x <= X - 1; x++)
        {
            for (int y = Y - 1; y >= -Y - 1; y--)
            {
                spawnFloor(new Vector2(x, y));
            }
        }
    }
    /// <summary>
    /// spawn the floor
    /// </summary>
    /// <param name="pos"></param>
    private void spawnFloor(Vector2 pos)
    {
        Vector2 intPos = new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));

        GameObject floor = ObjectPool.instance.Get(ObjectType.Floor, pos);
        if (poolObjectDic.ContainsKey(ObjectType.Floor) == false)
        {
            poolObjectDic.Add(ObjectType.Floor, new List<GameObject>());
        }
        else
        {
            poolObjectDic[ObjectType.Floor].Add(floor);
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

            if (wallSprites != null && wallSprites.Length > 0)
            {
                SpriteRenderer renderer = wall.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.sprite = wallSprites[Random.Range(0, wallSprites.Length)];
                }
            }

            if (poolObjectDic.ContainsKey(ObjectType.Wall) == false)
            {
                poolObjectDic.Add(ObjectType.Wall, new List<GameObject>());
            }
            poolObjectDic[ObjectType.Wall].Add(wall);
        }
    }
    /// <summary>
    /// create the enemy wall, enemy is hidden under the wall
    /// </summary>
    private void CreateEnemyWalls()
    {
        int count = 0;
        if (currentLevel == 1)
        {
            count = 5;
        }
        else if (currentLevel == 2)
        {
            count = 15;
        }
        else if (currentLevel == 3)
        {
            count = 20;
        }
        count = Mathf.Clamp(count, 0, emptyPointList.Count);

        for (int i = 0; i < count; i++)
        {
            if (emptyPointList.Count == 0) break;

            int index = Random.Range(0, emptyPointList.Count);
            Vector2 pos = emptyPointList[index];
            emptyPointList.RemoveAt(index);

            GameObject enemyWall = ObjectPool.instance.Get(ObjectType.EnemyWall, pos);

            if (enemyWallSprites != null && enemyWallSprites.Length > 0)
            {
                SpriteRenderer renderer = enemyWall.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.sprite = enemyWallSprites[Random.Range(0, enemyWallSprites.Length)];
                }
            }

            if (enemyWall == null)
            {
                Debug.LogError("Failed to get EnemyWall from pool");
                continue;
            }

            EnemyWallController controller = enemyWall.GetComponent<EnemyWallController>();
            if (controller != null)
            {
                controller.enemyPrefab = enemyPrefab;
            }

            if (poolObjectDic.ContainsKey(ObjectType.EnemyWall) == false)
            {
                poolObjectDic.Add(ObjectType.EnemyWall, new List<GameObject>());
            }
            poolObjectDic[ObjectType.EnemyWall].Add(enemyWall);
        }
    }

    /// <summary>
    /// generate prop
    /// </summary>
    private void createProps()
    {
        int count = 0;
        if (currentLevel == 1)
        {
            count = 18;
        }
        else if (currentLevel == 2)
        {
            count = 15;
        }
        else if (currentLevel == 3)
        {
            count = 12;
        }
        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, emptyPointList.Count);
            GameObject prop = ObjectPool.instance.Get(ObjectType.Prop, emptyPointList[index]);
            emptyPointList.RemoveAt(index);

            if (propSprites != null && propSprites.Length > 0)
            {
                SpriteRenderer renderer = prop.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.sprite = propSprites[Random.Range(0, propSprites.Length)];
                }
            }

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