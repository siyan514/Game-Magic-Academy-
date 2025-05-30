using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //public GameObject bombPre;

    //public int HP = 0;
    //public int Range = 0;
    //public int BombCount = 1;
    //public float BombTime = 0;

    //private Animator anim;
    //private float speed = 0.1f;
    //private Rigidbody2D rig;
    //private SpriteRenderer spriteRenderer;
    //private Color color;
    //private bool isInjured = false;
    //private int playerIndex;
    //private bool placeBomb = false;

    //private bool isInvincible = false; 
    //private float invincibleTimer = 0f; 

    public int HP { get; private set; }
    public int BombCount { get; private set; }
    public int Range { get; private set; }
    public int PlayerIndex { get; private set; }
    public float BombTime { get; private set; }
    public bool IsInvincible { get; private set; }

    public bool IsActive { get; set; } = true;
    private float invincibleTimer = 0f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine invincibilityEffect;
    private bool isInjured = false;


    private void Awake()
    {
        //anim = GetComponent<Animator>();
        //rig = GetComponent<Rigidbody2D>();
        //spriteRenderer = GetComponent<SpriteRenderer>();
        //color = spriteRenderer.color;

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    /// <summary>
    /// Initialize method 
    /// </summary>
    /// <param name="range"></param>
    /// <param name="HP"></param>
    /// <param name="bombTime"></param>
    public void Init(int range, int hp, float bombTime, int index)
    {
        //this.Range = range;
        //this.HP = HP;
        //this.BombTime = bombTime;
        //this.PlayerIndex = index;

        Range = range;
        HP = hp;
        BombTime = bombTime;
        PlayerIndex = index;
        BombCount = 1;
        IsInvincible = false;
    }


    /// <summary>
    /// Add the moving speed of player.
    /// </summary>
    /// <param name="value"></param>
    public void AddSpeed(float value = 0.25f)
    {
        //speed += value;
        //if (speed >= 0.15f) speed = 0.15f;
        GetComponent<PlayerMovement>().AddSpeed(value);
    }

    public void AddBomb()
    {
        BombCount++;
    }

    public void AddRange()
    {
        Range++;
    }

    public void AddHealth()
    {
        HP++;
    }

    public void UseBomb()
    {
        if (BombCount > 0)
        {
            BombCount--;
        }
    }

    private void Update()
    {
        //Move();
        //CreateBomb();

        if (IsInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0f)
            {
                IsInvincible = false;
                spriteRenderer.color = originalColor;
            }
        }
    }

    /// <summary>
    /// Activate invincibility
    /// </summary>
    public void ActivateInvincibility(float duration)
    {
        //isInvincible = true;
        //invincibleTimer = duration;
        //StartCoroutine(InvincibleEffect());
        if (invincibilityEffect != null)
        {
            StopCoroutine(invincibilityEffect);
        }

        IsInvincible = true;
        invincibleTimer = duration;
        invincibilityEffect = StartCoroutine(InvincibilityEffect());
    }

    //IEnumerator InvincibleEffect()
    //{
    //    Color originalColor = spriteRenderer.color;

    //    while (isInvincible)
    //    {
    //        Color[] rainbowColors = new Color[]
    //        {
    //        Color.red,
    //        Color.yellow,
    //        Color.green,
    //        Color.cyan,
    //        Color.blue,
    //        Color.magenta
    //        };

    //        for (int i = 0; i < rainbowColors.Length; i++)
    //        {
    //            Color startColor = rainbowColors[i];
    //            Color endColor = rainbowColors[(i + 1) % rainbowColors.Length];

    //            float elapsedTime = 0;
    //            float duration = 0.2f;

    //            while (elapsedTime < duration)
    //            {
    //                spriteRenderer.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
    //                elapsedTime += Time.deltaTime;
    //                yield return null;
    //            }
    //        }
    //    }

    //    spriteRenderer.color = originalColor;
    //}
    private IEnumerator InvincibilityEffect()
    {
        float startTime = Time.time;

        while (Time.time < startTime + invincibleTimer)
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
                if (!IsInvincible) break;

                Color startColor = rainbowColors[i];
                Color endColor = rainbowColors[(i + 1) % rainbowColors.Length];

                float elapsedTime = 0;
                float duration = 0.2f;

                while (elapsedTime < duration && IsInvincible)
                {
                    spriteRenderer.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
            }
        }

        spriteRenderer.color = originalColor;
        IsInvincible = false;
    }

    /// <summary>
    /// ���˺���޵�Ч������ɫ��˸��
    /// </summary>
    private IEnumerator InjuredEffect(float duration)
    {
        isInjured = true;
        float timer = 0f;

        while (timer < duration)
        {
            Color tempColor = originalColor;
            tempColor.a = 0.2f;
            spriteRenderer.color = tempColor;
            yield return new WaitForSeconds(0.25f);
            timer += 0.25f;

            if (timer >= duration) break;

            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.25f);
            timer += 0.25f;
        }

        spriteRenderer.color = originalColor;
        isInjured = false;
    }


    public void TakeDamage()
    {
        if (IsInvincible || isInjured || !IsActive) return;

        HP--;
        StartCoroutine(DisableControlRoutine());
        IsInvincible = true;
        invincibleTimer = 3f;
        StartCoroutine(InjuredEffect(3f));
    }

    IEnumerator DisableControlRoutine()
    {
        IsActive = false;
        yield return new WaitForSeconds(1f); // 1秒硬直时间
        IsActive = true;
    }

    /// <summary>
    /// move function
    /// </summary>
    //private void Move()
    //{
    //    float h = 0, v = 0;

    //    if (playerIndex == 1)
    //    {
    //        h = Input.GetAxis("Horizontal"); // WASD
    //        v = Input.GetAxis("Vertical");
    //    }
    //    else if (playerIndex == 2)
    //    {
    //        h = Input.GetAxis("Horizontal2"); // direction key
    //        v = Input.GetAxis("Vertical2");
    //    }

    //    anim.SetFloat("Horizontal", h); 
    //    anim.SetFloat("Vertical", v);
    //    rig.MovePosition(transform.position + new Vector3(h, v) * speed);
    //}

    ///// <summary>
    ///// place potion bomb
    ///// </summary>
    //private void CreateBomb()
    //{
    //    //player1 use the space key to place bomb
    //    if (playerIndex == 1 && Input.GetKeyDown(KeyCode.Space))
    //    {
    //        placeBomb = true;
    //    }
    //    //player2 use the enter key to place bomb
    //    else if (playerIndex == 2 &&
    //            (Input.GetKeyDown(KeyCode.Return) ||
    //             Input.GetKeyDown(KeyCode.KeypadEnter)))
    //    {
    //        placeBomb = true;
    //    }

    //    if (placeBomb && BombCount > 0)
    //    {
    //        BombCount--;
    //        GameObject bomb = ObjectPool.instance.Get(ObjectType.Bomb, 
    //            new Vector3(Mathf.RoundToInt(transform.position.x),
    //            Mathf.RoundToInt(transform.position.y)));
    //        bomb.GetComponent<BombController>().Init(range, bombTime, () => BombCount++);
    //    }
    //    placeBomb = false;
    //}


    ///// <summary>
    ///// Determine whether one has been hit by a bomb
    ///// </summary>
    ///// <param name="collider"></param>
    //private void OnTriggerEnter2D(Collider2D collider)
    //{
    //    if (isInjured || isInvincible) return;
    //    isInjured = true;
    //    if (collider.CompareTag(Tags.BombEffect))
    //    {
    //        HP--;
    //        StartCoroutine("Injured", 2f);
    //    }
    //    isInjured = false;
    //}

    ///// <summary>
    ///// Animating the scene after being injured by an explosion
    ///// </summary>
    ///// <param name="time"></param>
    ///// <returns></returns>
    //IEnumerator Injured(float time)
    //{
    //    for (int i = 0; i < time * 2; i++)
    //    {
    //        color.a = 0.2f;
    //        spriteRenderer.color = color;
    //        yield return new WaitForSeconds(0.25f);
    //        color.a = 1;
    //        spriteRenderer.color = color;
    //        yield return new WaitForSeconds(0.25f);
    //    }
    //}
}
