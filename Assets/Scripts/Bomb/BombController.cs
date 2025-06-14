using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/// <summary>
/// Bomb explode implementation class
/// </summary>
public class BombController : MonoBehaviour
{
    public GameObject bombEffect;
    private int range;
    private Action aniFinAction;
    private PlayerBase bombOwner;

    public void Init(int range, float explosiveTime, Action action, PlayerBase owner)
    {
        this.range = range;
        this.bombOwner = owner;
        StartCoroutine("ExplosiveTime", explosiveTime);
        aniFinAction = action;
    }
    /// <summary>
    /// control the bomb explode
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    IEnumerator ExplosiveTime(float time)
    {
        yield return new WaitForSeconds(time);
        if (aniFinAction != null) { aniFinAction(); }
        CreateBombEffect(transform.position);

        Boom(Vector2.left);
        Boom(Vector2.right);
        Boom(Vector2.down);
        Boom(Vector2.up);
        ObjectPool.instance.Add(ObjectType.Bomb, gameObject);
    }
    /// <summary>
    /// control the bomb effect
    /// </summary>
    /// <param name="pos"></param>
    private void CreateBombEffect(Vector2 pos)
    {
        GameObject effect = ObjectPool.instance.Get(ObjectType.BombEffect, pos);
        BombEffect bombEffectComponent = effect.GetComponent<BombEffect>();
        if (bombEffectComponent != null)
        {
            bombEffectComponent.SetBombOwner(bombOwner);
        }
    }
    /// <summary>
    /// handle the bomb explode
    /// </summary>
    /// <param name="dir"></param>
    private void Boom(Vector2 dir)
    {
        for (int i = 1; i <= range; i++)
        {
            Vector2 pos = (Vector2)transform.position + dir * i;
            if (GameController.instance.IsSuperWall(pos)) break;
            CreateBombEffect(pos);
        }
    }
}