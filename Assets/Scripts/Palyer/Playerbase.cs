using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// this is an abstract base class of player
/// </summary>
public abstract class PlayerBase : MonoBehaviour
{
    // Player core attributes
    public int HP { get; protected set; }
    public int winNum { get; set; }
    public int BombCount { get; protected set; }
    public int Range { get; protected set; }
    public int PlayerIndex { get; protected set; }
    public float BombTime { get; protected set; }
    public bool IsInvincible { get; protected set; }
    public bool IsActive { get; protected set; } = true;


    // Component references
    protected Animator anim;
    protected Rigidbody2D rig;
    protected SpriteRenderer spriteRenderer;
    protected Color originalColor;

    // Movement speed
    protected float speed = 0.1f;

    protected Coroutine currentInvincibilityCoroutine;

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        rig = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;
    }

    public virtual void Init(int range, int hp, float bombTime, int index)
    {
        winNum = 0;
        Range = range;
        HP = hp;
        BombTime = bombTime;
        PlayerIndex = index;
        BombCount = 1;
        IsInvincible = false;
    }

    public abstract void Move(Vector2 direction);
    public abstract void PlaceBomb();
    public abstract void TakeDamage();
    public virtual void ActivateInvincibility(float duration, bool isRainbowEffect = true)
    {
        // Stop existing invincibility effect
        if (currentInvincibilityCoroutine != null)
        {
            StopCoroutine(currentInvincibilityCoroutine);
        }

        // Restore original color
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        IsInvincible = true;

        // Start different invincibility effects based on parameter
        if (isRainbowEffect)
        {
            currentInvincibilityCoroutine = StartCoroutine(InvincibilityEffect(duration));
        }
        else
        {
            currentInvincibilityCoroutine = StartCoroutine(TemporaryInvincibilityRoutine(duration));
        }
    }

    // Player power-up methods
    public virtual void AddSpeed(float value = 0.25f)
    {
        speed += value;
        if (speed >= 0.15f) speed = 0.15f;
    }

    public virtual void AddBomb()
    {
        BombCount++;
    }

    public virtual void AddRange()
    {
        Range++;
    }

    public virtual void AddHealth()
    {
        HP++;
    }

    public virtual void UseBomb()
    {
        if (BombCount > 0) BombCount--;
    }
    /// <summary>
    /// Damage effect coroutine
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    protected IEnumerator InjuredEffect(float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            if (spriteRenderer != null)
            {
                // Semi-transparent effect
                Color tempColor = originalColor;
                tempColor.a = 0.2f;
                spriteRenderer.color = tempColor;
            }
            yield return new WaitForSeconds(0.25f);
            timer += 0.25f;

            if (timer >= duration) break;

            if (spriteRenderer != null)
                spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.25f);
            timer += 0.25f;
        }
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }
    /// <summary>
    /// Rainbow effect only for invincibility state
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    protected IEnumerator InvincibilityEffect(float duration)
    {
        float startTime = Time.time;
        while (Time.time < startTime + duration)
        {
            Color[] rainbowColors = {
            Color.red, Color.yellow, Color.green,
            Color.cyan, Color.blue, Color.magenta
        };

            for (int i = 0; i < rainbowColors.Length; i++)
            {
                Color startColor = rainbowColors[i];
                Color endColor = rainbowColors[(i + 1) % rainbowColors.Length];
                float elapsedTime = 0f;
                float colorDuration = 0.2f;

                while (elapsedTime < colorDuration)
                {
                    if (spriteRenderer != null)
                        spriteRenderer.color = Color.Lerp(startColor, endColor, elapsedTime / colorDuration);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
            }
        }
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        // Invincibility state ends
        IsInvincible = false;
        currentInvincibilityCoroutine = null;
    }
    /// <summary>
    /// Gray invincibility coroutine
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    protected IEnumerator TemporaryInvincibilityRoutine(float duration)
    {
        // Start gray flashing effect
        yield return StartCoroutine(InjuredEffect(duration));

        // Invincibility state ends
        IsInvincible = false;
        currentInvincibilityCoroutine = null;
    }
    /// <summary>
    /// Disable control coroutine
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    protected IEnumerator DisableControlRoutine(float duration)
    {
        IsActive = false;
        yield return new WaitForSeconds(duration);
        IsActive = true;
    }

    public virtual void Die()
    {
        // Stop all coroutines
        StopAllCoroutines();

        // Set state
        IsActive = false;
        IsInvincible = true; // Prevent any further damage

        // Disable collider
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;

        // Disable rigidbody physics
        if (rig != null) rig.simulated = false;


        // Disappear from screen
        StartCoroutine(DisappearAfterDelay(0.5f));
    }
    /// <summary>
    /// Delayed disappearance coroutine
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    protected virtual IEnumerator DisappearAfterDelay(float delay)
    {
        float timer = 0f;
        while (timer < delay)
        {
            if (spriteRenderer != null)
            {
                Color newColor = spriteRenderer.color;
                newColor.a = Mathf.Lerp(1f, 0f, timer / delay);
                spriteRenderer.color = newColor;
            }
            timer += Time.deltaTime;
            yield return null;
        }

        // Finally destroy the object
        Destroy(gameObject);
    }
}