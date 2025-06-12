using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Wall controller implementation class
/// </summary>
public class wall : MonoBehaviour
{
    /// <summary>
    /// handle the trigger
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.BombEffect))
        {
            ObjectPool.instance.Add(ObjectType.Wall, gameObject);
        }
    }
}
