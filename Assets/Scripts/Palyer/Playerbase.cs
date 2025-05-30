using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBase : MonoBehaviour
{
    // 玩家核心属性
    public int HP { get; protected set; }
    public int BombCount { get; protected set; }
    public int Range { get; protected set; }
    public int PlayerIndex { get; protected set; }
    public float BombTime { get; protected set; }
    public bool IsInvincible { get; protected set; }
    public bool IsActive { get; protected set; } = true;

    // 组件引用
    protected Animator anim;
    protected Rigidbody2D rig;
    protected SpriteRenderer spriteRenderer;
    protected Color originalColor;

    // 移动速度
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
        // 停止现有的无敌效果
        if (currentInvincibilityCoroutine != null)
        {
            StopCoroutine(currentInvincibilityCoroutine);
        }

        // 恢复原始颜色
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        IsInvincible = true;

        // 根据参数启动不同类型无敌效果
        if (isRainbowEffect)
        {
            currentInvincibilityCoroutine = StartCoroutine(InvincibilityEffect(duration));
        }
        else
        {
            currentInvincibilityCoroutine = StartCoroutine(TemporaryInvincibilityRoutine(duration));
        }
    }

    // 玩家能力增强方法
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

    // 受伤效果协程
    protected IEnumerator InjuredEffect(float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            if (spriteRenderer != null)
            {
                // 半透明效果
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

    // 彩虹特效仅用于无敌状态
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

        // 新增：无敌状态结束
        IsInvincible = false;
        currentInvincibilityCoroutine = null;
    }

    // 新增：灰色无敌协程
    protected IEnumerator TemporaryInvincibilityRoutine(float duration)
    {
        // 启动灰色闪烁效果
        yield return StartCoroutine(InjuredEffect(duration));

        // 无敌状态结束
        IsInvincible = false;
        currentInvincibilityCoroutine = null;
    }

    // 禁用控制协程
    protected IEnumerator DisableControlRoutine(float duration)
    {
        IsActive = false;
        yield return new WaitForSeconds(duration);
        IsActive = true;
    }

    public virtual void Die()
    {
        // 停止所有协程
        StopAllCoroutines();

        // 设置状态
        IsActive = false;
        IsInvincible = true; // 防止任何后续伤害

        // 禁用碰撞体
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;

        // 禁用刚体
        if (rig != null) rig.simulated = false;

        // 播放死亡动画（如果有）
        if (anim != null) anim.SetTrigger("Die");

        // 从屏幕上消失（禁用或销毁）
        StartCoroutine(DisappearAfterDelay(0.5f));
    }

    // 新增：延迟消失协程
    protected virtual IEnumerator DisappearAfterDelay(float delay)
    {
        // 可选：添加消失效果（如渐隐）
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

        // 最终销毁对象
        Destroy(gameObject);
    }
}
