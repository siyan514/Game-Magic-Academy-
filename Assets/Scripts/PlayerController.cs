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
    private int playerIndex;
    private bool placeBomb = false;

    private bool isInvincible = false; // 是否处于无敌状态
    private float invincibleTimer = 0f; // 无敌时间计时器

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
    public void Init(int range, int HP, float bombTime, int index)
    {
        this.range = range;
        this.HP = HP;
        this.bombTime = bombTime;
        this.playerIndex = index;
    }

    private void Update()
    {
        Move();
        CreateBomb();

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0f)
            {
                isInvincible = false;
                spriteRenderer.color = Color.white; // 恢复颜色
            }
        }
    }

    /// <summary>
    /// 激活无敌状态
    /// </summary>
    public void ActivateInvincibility(float duration)
    {
        isInvincible = true;
        invincibleTimer = duration;
        StartCoroutine(InvincibleEffect());
    }

    IEnumerator InvincibleEffect()
    {
        Color originalColor = spriteRenderer.color;

        while (isInvincible)
        {
            Color[] rainbowColors = new Color[]
            {
            Color.red,
            Color.yellow,
            Color.green,
            Color.cyan,
            Color.blue,
            Color.magenta
            };

            for (int i = 0; i < rainbowColors.Length; i++)
            {
                Color startColor = rainbowColors[i];
                Color endColor = rainbowColors[(i + 1) % rainbowColors.Length];

                float elapsedTime = 0;
                float duration = 0.2f;

                while (elapsedTime < duration)
                {
                    spriteRenderer.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
            }
        }

        spriteRenderer.color = originalColor;
    }

    /// <summary>
    /// move function
    /// </summary>
    private void Move()
    {
        float h = 0, v = 0;

        if (playerIndex == 1)
        {
            h = Input.GetAxis("Horizontal"); // WASD
            v = Input.GetAxis("Vertical");
        }
        else if (playerIndex == 2)
        {
            h = Input.GetAxis("Horizontal2"); // direction key
            v = Input.GetAxis("Vertical2");
        }

        anim.SetFloat("Horizontal", h); 
        anim.SetFloat("Vertical", v);
        rig.MovePosition(transform.position + new Vector3(h, v) * speed);
    }

    /// <summary>
    /// place potion bomb
    /// </summary>
    private void CreateBomb()
    {
        //player1 use the space key to place bomb
        if (playerIndex == 1 && Input.GetKeyDown(KeyCode.Space))
        {
            placeBomb = true;
        }
        //player2 use the enter key to place bomb
        else if (playerIndex == 2 &&
                (Input.GetKeyDown(KeyCode.Return) ||
                 Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            placeBomb = true;
        }

        if (placeBomb && bombCount > 0)
        {
            bombCount--;
            GameObject bomb = ObjectPool.instance.Get(ObjectType.Bomb, 
                new Vector3(Mathf.RoundToInt(transform.position.x),
                Mathf.RoundToInt(transform.position.y)));
            bomb.GetComponent<BombController>().Init(range, bombTime, () => bombCount++);
        }
        placeBomb = false;
    }

    /// <summary>
    /// Determine whether one has been hit by a bomb
    /// </summary>
    /// <param name="collider"></param>
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (isInjured || isInvincible) return;
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
