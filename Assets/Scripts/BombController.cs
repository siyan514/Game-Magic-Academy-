using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BombController : MonoBehaviour
{
    public GameObject bombEffect;
    private int range;
    private Action aniFinAction;

    public void Init(int range, float explosiveTime, Action action)
    {
        this.range = range;
        StartCoroutine("ExplosiveTime", explosiveTime);
        aniFinAction = action;
    }

    IEnumerator ExplosiveTime(float time)
    {
        yield return new WaitForSeconds(time);
        if(aniFinAction != null) { aniFinAction(); }
        ObjectPool.instance.Get(ObjectType.BombEffect, transform.position);
        Boom(Vector2.left);
        Boom(Vector2.right);
        Boom(Vector2.down);
        Boom(Vector2.up);
        ObjectPool.instance.Add(ObjectType.Bomb, gameObject);
    }

    private void Boom(Vector2 dir) 
    {
        for(int i = 1;i <= range; i++)
        {
            Vector2 pos = (Vector2)transform.position + dir * i;
            if(GameController.instance.IsSuperWall(pos)) break;
            ObjectPool.instance.Get(ObjectType.BombEffect, pos);
        }
    }
}
