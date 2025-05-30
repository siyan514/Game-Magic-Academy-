using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType
{
    SuperWall,
    Wall,
    Prop,
    Bomb,
    BombEffect
}

[System.Serializable]
public class Type_Prefab
{
    public ObjectType type;
    public GameObject prefab;
}

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;
    public List<Type_Prefab> type_Prefabs = new List<Type_Prefab>();

    /// <summary>
    /// Obtain the prefab based on the object type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private GameObject GetPreByType(ObjectType type)
    {
        foreach (var item in type_Prefabs)
        {
            if (item.type == type) 
            { 
                return item.prefab;
            }
        }
        return null;
    }

    /// <summary>
    /// Dictionary of object types and their corresponding object pool relationships
    /// </summary>
    private Dictionary<ObjectType, List<GameObject>> dic = new Dictionary<ObjectType, List<GameObject>>();

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Retrieve items from the corresponding object pool based on the type of the object.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public GameObject Get(ObjectType type, Vector2 pos)
    {
        GameObject temp = null;
        //Check if there is an object pool in the dictionary that matches this type. If not, create one.
        if (dic.ContainsKey(type) == false)
            dic.Add(type, new List<GameObject>());
        //Check if there are any objects in this type of object pool
        if (dic[type].Count > 0)
        {
            int index = dic[type].Count - 1;
            temp = dic[type][index];
            dic[type].RemoveAt(index);
        }
        else
        {
            GameObject pre = GetPreByType(type);
            if (pre != null)
            {
                temp = Instantiate(pre, transform);
            }
        }
        temp.SetActive(true);
        temp.transform.position = pos;
        temp.transform.rotation = Quaternion.identity;
        return temp;
    }

    /// <summary>
    /// Recycle.
    /// </summary>
    /// <param name="type"></param>
    public void Add(ObjectType type, GameObject go)
    {
        //Determine whether the object has a corresponding object pool and whether the object does not exist in the object pool.
        if (dic.ContainsKey(type) && dic[type].Contains(go) == false)
        {
            //Put into the object pool.
            dic[type].Add(go);
        }
        go.SetActive(false);
    }
}
