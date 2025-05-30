using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : PlayerBase
{
    private HumanInputHandler inputHandler;

    protected override void Awake()
    {
        base.Awake();
        inputHandler = gameObject.AddComponent<HumanInputHandler>();
    }

    public override void Init(int range, int hp, float bombTime, int index)
    {
        base.Init(range, hp, bombTime, index);
        inputHandler.PlayerIndex = index;
    }

    private void Update()
    {
        if (!IsActive) return;

        // 处理移动输入
        Vector2 moveInput = inputHandler.GetMovementInput();
        if (moveInput != Vector2.zero)
        {
            Move(moveInput);
        }

        // 处理炸弹放置
        if (inputHandler.GetBombInput())
        {
            PlaceBomb();
        }
    }

    public override void Move(Vector2 direction)
    {
        // 更新动画
        anim.SetFloat("Horizontal", direction.x);
        anim.SetFloat("Vertical", direction.y);

        // 应用移动
        rig.MovePosition(rig.position + direction * speed);
    }

    public override void PlaceBomb()
    {
        if (BombCount > 0)
        {
            UseBomb();
            Vector3 bombPos = new Vector3(
                Mathf.RoundToInt(transform.position.x),
                Mathf.RoundToInt(transform.position.y)
            );

            GameObject bomb = ObjectPool.instance.Get(ObjectType.Bomb, bombPos);
            bomb.GetComponent<BombController>().Init(Range, BombTime, () => AddBomb());
        }
    }

    public override void TakeDamage()
    {
        if (IsInvincible || !IsActive) return;

        HP--;

        if (HP <= 0)
        {

            Die(); // 调用死亡方法
        }
        else
        {
            // 僵直1秒
            StartCoroutine(DisableControlRoutine(1f));
            // 激活3秒灰色无敌效果
            ActivateInvincibility(3f, false);
        }
    }

    protected override IEnumerator DisappearAfterDelay(float delay)
    {
        // 播放死亡音效
        // AudioManager.Play("PlayerDeath");
        
        // 执行渐隐效果
        yield return base.DisappearAfterDelay(delay);

        // 可以在这里触发游戏结束事件等
        // GameManager.Instance.PlayerDied(PlayerIndex);
    }

    // public override void ActivateInvincibility(float duration)
    // {
    //     IsInvincible = true;
    //     StartCoroutine(InvincibilityEffect(duration));
    // }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag(Tags.BombEffect))
        {
            TakeDamage();
        }
    }
}