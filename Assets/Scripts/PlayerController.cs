using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //public GameObject bombPre;

    public int HP = 0;
    public int range = 0;
    public int bombCount = 1;
    public float bombTime = 0;

    private Animator anim;
    private float speed = 0.1f;
    private Rigidbody2D rig;
    private SpriteRenderer spriteRenderer;
    private Color color;
    private bool isInjured = false;
    
    /// <summary>
    /// Add the moving speed of player.
    /// </summary>
    /// <param name="value"></param>
    public void AddSpeed(float value = 0.25f)
    {
        speed += value;
        if (speed >= 0.15f) speed = 0.15f;
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rig = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        color = spriteRenderer.color;
    }

    /// <summary>
    /// Initialize method 
    /// </summary>
    /// <param name="range"></param>
    /// <param name="HP"></param>
    /// <param name="bombTime"></param>
    public void Init(int range, int HP, float bombTime)
    {
        this.range = range;
        this.HP = HP;
        this.bombTime = bombTime;
    }

    private void Update()
    {
        Move();
        CreateBomb();
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

    /// <summary>
    /// place potion bomb
    /// </summary>
    private void CreateBomb()
    {
        if (Input.GetKeyDown(KeyCode.Space) && bombCount > 0)
        {
            bombCount--;
            GameObject bomb = ObjectPool.instance.Get(ObjectType.Bomb, 
                new Vector3(Mathf.RoundToInt(transform.position.x),
                Mathf.RoundToInt(transform.position.y)));
            bomb.GetComponent<BombController>().Init(range, bombTime, () => bombCount++);
        }
    }

    /// <summary>
    /// Determine whether one has been hit by a bomb
    /// </summary>
    /// <param name="collider"></param>
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (isInjured) return;
        isInjured = true;
        if (collider.CompareTag(Tags.BombEffect))
        {
            HP--;
            StartCoroutine("Injured", 2f);
        }
        isInjured = false;
    }

    /// <summary>
    /// Animating the scene after being injured by an explosion
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    IEnumerator Injured(float time)
    {
        for(int i = 0; i < time * 2; i++)
        {
            color.a = 0.2f;
            spriteRenderer.color = color;
            yield return new WaitForSeconds(0.25f);
            color.a = 1;
            spriteRenderer.color = color;
            yield return new WaitForSeconds(0.25f);
        }
    }
}
