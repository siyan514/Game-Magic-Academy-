using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : MonoBehaviour
{
    public GameObject bombEffect;
    private int range;

    public void Init(int range, float explosiveTime)
    {
        this.range = range;
        StartCoroutine("ExplosiveTime", explosiveTime);
    }

    IEnumerator ExplosiveTime(float time)
    {
        yield return new WaitForSeconds(time);
        Instantiate(bombEffect, transform.position, Quaternion.identity);
        Boom(Vector2.left);
        Boom(Vector2.right);
        Boom(Vector2.down);
        Boom(Vector2.up);
        Destroy(gameObject);
    }

    private void Boom(Vector2 dir) 
    {
        for(int i = 1;i <= range; i++)
        {
            Vector2 pos = (Vector2)transform.position + dir * i;
            if(GameController.instance.IsSuperWall(pos)) break;
            GameObject effect = Instantiate(bombEffect);
            effect.transform.position = pos;
        }
    }
}
