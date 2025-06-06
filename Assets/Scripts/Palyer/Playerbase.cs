using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBase : MonoBehaviour
{
    // Player core attributes
    public int HP { get; protected set; }
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
    

    // Damage effect coroutine
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

    // Rainbow effect only for invincibility state
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

    // Gray invincibility coroutine
    protected IEnumerator TemporaryInvincibilityRoutine(float duration)
    {
        // Start gray flashing effect
        yield return StartCoroutine(InjuredEffect(duration));

        // Invincibility state ends
        IsInvincible = false;
        currentInvincibilityCoroutine = null;
    }

    // Disable control coroutine
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

        // Play death animation (if exists)
        // if (anim != null) anim.SetTrigger("Die");

        // Disappear from screen
        StartCoroutine(DisappearAfterDelay(0.5f));
    }

    // Delayed disappearance coroutine
    protected virtual IEnumerator DisappearAfterDelay(float delay)
    {
        // Optional: Add disappearance effect (like fade-out)
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