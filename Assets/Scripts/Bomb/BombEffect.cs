using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombEffect : MonoBehaviour
{
    private PlayerBase bombOwner;

    public void SetBombOwner(PlayerBase owner)
    {
        bombOwner = owner;
    }

    public PlayerBase GetBombOwner()
    {
        return bombOwner;
    }

    /// <summary>
    /// After the bomb's special effect animation is played, it destroys itself.
    /// </summary>
    private void aniFin()
    {
        bombOwner = null;
        ObjectPool.instance.Add(ObjectType.BombEffect, gameObject);
    }
}