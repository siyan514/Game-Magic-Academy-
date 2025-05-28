using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Animator anim;
    private float speed = 0.1f;
    private Rigidbody2D rig;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rig = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        Move();
    }

    /// <summary>
    /// move function
    /// </summary>
    private void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        anim.SetFloat("Horizontal", h);
        anim.SetFloat("Vertical", v);
        rig.MovePosition(transform.position + new Vector3(h, v) * speed);
    }
}
