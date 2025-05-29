using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombEffect : MonoBehaviour
{
    /// <summary>
    /// After the bomb's special effect animation is played, it destroys itself.
    /// </summary>
    private void aniFin()
    {
        ObjectPool.instance.Add(ObjectType.BombEffect, gameObject);
    }
}
