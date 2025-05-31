using UnityEngine;

public class AIPlayer : PlayerBase
{
    private AIController aiController;
    private Vector2 nextDirection; // 添加方向变量

    protected override void Awake()
    {
        base.Awake();
        aiController = gameObject.AddComponent<AIController>();
    }

    public override void Init(int range, int hp, float bombTime, int index)
    {
        base.Init(range, hp, bombTime, index);
        aiController.Init(this);

        // 初始化方向
        nextDirection = Vector2.right;
    }

    // 修改为private void Update()，因为PlayerBase中没有定义Update
    private void Update()
    {
        if (!IsActive) return;

        // 基础随机移动 - 每60帧（约1秒）改变方向
        if (Time.frameCount % 300 == 0)
        {
            nextDirection = new Vector2(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f)
            ).normalized;

            // 调试输出
            Debug.Log($"[AI {PlayerIndex}] Changing direction to: {nextDirection}");
        }

        Move(nextDirection);
    }

    public override void Move(Vector2 direction)
    {
        // 应用移动，与HumanPlayer相同的逻辑
        rig.MovePosition(rig.position + direction * speed);

        // 更新动画参数
        anim.SetFloat("Horizontal", direction.x);
        anim.SetFloat("Vertical", direction.y);
    }

    public override void PlaceBomb()
    {
        // 简单实现炸弹放置逻辑
        if (BombCount > 0)
        {
            Debug.Log($"[AI {PlayerIndex}] Placing bomb!");
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
        // 复制HumanPlayer的受伤逻辑
        if (IsInvincible || !IsActive) return;

        Debug.Log($"[AI {PlayerIndex}] Taking damage! HP: {HP} -> {HP - 1}");

        HP--;

        if (HP <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(DisableControlRoutine(1f));
            ActivateInvincibility(3f, false);
        }
    }

    // 添加碰撞检测
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BombEffect")) // 确保标签匹配
        {
            TakeDamage();
        }
    }
}