using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AIEnemy : MonoBehaviour
{
    public enum EnemyState { Hidden, Active, Chasing, Wandering, Dead }

    [Header("Enemy Settings")]
    public float wanderingSpeed = 0.5f;
    public float moveSpeed = 2f;
    public int damageToPlayer = 1;
    public float attackCooldown = 3f;
    public float pathUpdateInterval = 0.5f;
    private float rayDistance = 1f;

    [Header("Pathfinding Settings")]
    public bool enablePathDrawing = true;  // 是否启用路径绘制
    public Color pathColor = Color.red;     // 路径绘制颜色

    [Header("References")]
    public GameObject explosionPrefab;
    protected Animator anim;
    protected Rigidbody2D rig;
    protected SpriteRenderer spriteRenderer;

    private EnemyState currentState = EnemyState.Hidden;
    private Transform targetPlayer;
    private List<Vector2Int> currentPath;
    private int currentWaypointIndex;
    private float lastPathUpdate = -10f;
    private float lastAttackTime = -10f;
    private PlayerBase bombOwner;
    private Vector2Int gridPosition;
    private Vector2Int targetGridPosition;
    private Vector3 lastMoveDirection = Vector3.right;

    [Header("Wandering Settings")]
    private int dirID;
    private Vector2 dirVector;
    public bool canMove = true;





    private void Awake()
    {
        anim = GetComponent<Animator>();
        rig = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentPath = new List<Vector2Int>();
        SetState(EnemyState.Hidden);
        WanderDir(Random.Range(0, 4));
    }

    public void Initialize(Vector2Int position, PlayerBase owner)
    {
        gridPosition = position;
        bombOwner = owner;
        transform.position = new Vector3(position.x, position.y, 0);
    }

    public void Activate(PlayerBase owner)
    {
        bombOwner = owner;
        if (currentState == EnemyState.Hidden)
        {
            SetState(EnemyState.Active);
            StartCoroutine(ActivationRoutine());
        }
    }

    private System.Collections.IEnumerator ActivationRoutine()
    {
        SetState(EnemyState.Active);
        GetComponent<SpriteRenderer>().enabled = true;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (anim != null) anim.SetTrigger("Activate");
        yield return new WaitForSeconds(0.8f);

        SetState(EnemyState.Chasing);
        if (col != null) col.enabled = true;

        FindTargetPlayer();

        // 开始寻路
        UpdatePath();
    }

    private void FindTargetPlayer()
    {
        if (bombOwner != null && bombOwner.IsActive)
        {
            targetPlayer = bombOwner.transform;
            return;
        }
        else
        {
            // 对齐到整数格子
            transform.position = new Vector3(
                Mathf.RoundToInt(transform.position.x),
                Mathf.RoundToInt(transform.position.y),
                0
            );

            SetState(EnemyState.Wandering);
            print(currentState);
            return;
        }
    }

    private void Update()
    {
        if (currentState == EnemyState.Chasing)
        {
            if (targetPlayer == null)
            {
                FindTargetPlayer();
                return;
            }

            if (bombOwner == null || !bombOwner.IsActive)
            {
                return;
            }

            // 定期更新路径
            if (Time.time - lastPathUpdate > pathUpdateInterval)
            {
                UpdatePath();
            }

            // 沿路径移动
            MoveAlongPath();
        }
        else if (currentState == EnemyState.Wandering)
        {
            if (canMove)
            {
                // rig.MovePosition((Vector2)transform.position + dirVector * wanderingSpeed * Time.deltaTime);
                transform.position += (Vector3)dirVector * wanderingSpeed * Time.deltaTime;
                HandleFacing(dirVector); // 添加这行处理朝向

                // 更新动画参数
                if (anim != null)
                {
                    anim.SetFloat("Horizontal", dirVector.x);
                    anim.SetFloat("Vertical", dirVector.y);
                }
            }
            else
            {
                WanderChangeDir();
            }
        }
    }

    private void WanderDir(int dir)
    {
        dirID = dir;
        transform.position = new Vector3(
        Mathf.RoundToInt(transform.position.x),
        Mathf.RoundToInt(transform.position.y),
        0
        );
        switch (dirID)
        {
            case 0:
                dirVector = Vector2.up;
                break;
            case 1:
                dirVector = Vector2.down;
                break;
            case 2:
                dirVector = Vector2.left;
                break;
            case 3:
                dirVector = Vector2.right;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 更新到目标的路径
    /// </summary>
    private void UpdatePath()
    {
        if (targetPlayer == null) return;

        lastPathUpdate = Time.time;

        // 获取当前网格位置和目标网格位置
        Vector2Int currentGridPos = new Vector2Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.y)
        );

        Vector2Int targetGridPos = new Vector2Int(
            Mathf.RoundToInt(targetPlayer.position.x),
            Mathf.RoundToInt(targetPlayer.position.y)
        );

        // 如果目标位置改变，重新计算路径
        if (targetGridPos != targetGridPosition || currentPath == null || currentPath.Count == 0)
        {
            targetGridPosition = targetGridPos;
            currentPath = AStarPathfinding.FindPath(currentGridPos, targetGridPos);
            currentWaypointIndex = 0;

            // 如果起始点就在路径中，跳过它
            if (currentPath != null && currentPath.Count > 0 && currentPath[0] == currentGridPos)
            {
                currentWaypointIndex = 1;
            }
        }
    }

    /// <summary>
    /// 沿着计算出的路径移动
    /// </summary>
    private void MoveAlongPath()
    {
        if (currentPath == null || currentPath.Count == 0) return;
        if (currentWaypointIndex >= currentPath.Count) return;

        Vector2Int targetWaypoint = currentPath[currentWaypointIndex];
        Vector3 targetWorldPos = new Vector3(targetWaypoint.x, targetWaypoint.y, 0);

        // 计算移动方向
        Vector3 direction = (targetWorldPos - transform.position).normalized;

        // 移动到目标点
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetWorldPos,
            moveSpeed * Time.deltaTime
        );

        HandleFacing(direction);

        // 更新动画参数
        if (anim != null)
        {
            anim.SetFloat("Horizontal", direction.x);
            anim.SetFloat("Vertical", direction.y);
        }

        // 检查是否到达当前路径点
        float distanceToWaypoint = Vector3.Distance(transform.position, targetWorldPos);
        if (distanceToWaypoint < 0.1f)
        {
            currentWaypointIndex++;
        }
    }

    private void HandleFacing(Vector3 moveDirection)
    {
        if (spriteRenderer == null) return;

        if (Mathf.Abs(moveDirection.x) > 0.1f)
        {
            lastMoveDirection = moveDirection;
        }

        if (lastMoveDirection.x > 0.1f)
        {
            spriteRenderer.transform.localScale = new Vector3(
                Mathf.Abs(spriteRenderer.transform.localScale.x),
                spriteRenderer.transform.localScale.y,
                spriteRenderer.transform.localScale.z
            );
        }
        else if (lastMoveDirection.x < -0.1f)
        {
            spriteRenderer.transform.localScale = new Vector3(
                -Mathf.Abs(spriteRenderer.transform.localScale.x),
                spriteRenderer.transform.localScale.y,
                spriteRenderer.transform.localScale.z
            );
        }
    }

    /// <summary>
    /// 绘制路径（仅在启用时）
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, 1, 0));
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -1, 0));
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(1, 0, 0));
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(-1, 0, 0));

        if (!enablePathDrawing || currentPath == null || currentPath.Count == 0) return;

        Gizmos.color = pathColor;

        // 绘制路径线段
        for (int i = 0; i < currentPath.Count - 1; i++)
        {
            Vector3 start = new Vector3(currentPath[i].x, currentPath[i].y, 0);
            Vector3 end = new Vector3(currentPath[i + 1].x, currentPath[i + 1].y, 0);
            Gizmos.DrawLine(start, end);
        }

        // 绘制路径点
        for (int i = 0; i < currentPath.Count; i++)
        {
            Vector3 point = new Vector3(currentPath[i].x, currentPath[i].y, 0);

            if (i == currentWaypointIndex)
            {
                // 当前目标点用不同颜色
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(point, 0.2f);
                Gizmos.color = pathColor;
            }
            else
            {
                Gizmos.DrawWireSphere(point, 0.15f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.BombEffect))
        {
            Die();
        }
        else if (collision.CompareTag(Tags.Player))
        {
            PlayerBase player = collision.gameObject.GetComponent<PlayerBase>();
            if (player != null && player.IsActive)
            {
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    player.TakeDamage();
                    lastAttackTime = Time.time;
                }
            }
        }

        if (collision.CompareTag(Tags.Wall) || collision.CompareTag(Tags.SuperWall))
        {
            if (currentState == EnemyState.Wandering)
            {
                transform.position = new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
                WanderChangeDir();
            }
        }
    }

    private void WanderChangeDir()
    {
        List<int> possibleDirs = new List<int>();

        // 检查四个方向是否可走
        if (!Physics2D.Raycast(transform.position, Vector2.up, rayDistance, 1 << 6)) possibleDirs.Add(0);
        if (!Physics2D.Raycast(transform.position, Vector2.down, rayDistance, 1 << 6)) possibleDirs.Add(1);
        if (!Physics2D.Raycast(transform.position, Vector2.left, rayDistance, 1 << 6)) possibleDirs.Add(2);
        if (!Physics2D.Raycast(transform.position, Vector2.right, rayDistance, 1 << 6)) possibleDirs.Add(3);

        // 如果当前方向仍然可行，优先保持（避免频繁转向）
        if (possibleDirs.Contains(dirID))
        {
            return; // 不改变方向
        }
        // 否则随机选一个新方向
        else if (possibleDirs.Count > 0)
        {
            canMove = true;
            int newDir = possibleDirs[Random.Range(0, possibleDirs.Count)];
            WanderDir(newDir);
        }
        else
        {
            canMove = false;
        }
    }



    public void Die()
    {
        currentState = EnemyState.Dead;

        SetState(EnemyState.Dead);

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;

        if (rig != null) rig.simulated = false;
        Destroy(gameObject, 0.1f);

        if (GameUIController.instance != null)
            GameUIController.instance.UpdateEnemyCount(GameUIController.instance.totalEnemies - 1);
    }

    private void SetState(EnemyState newState)
    {
        currentState = newState;

        if (newState == EnemyState.Hidden)
        {
            GetComponent<Collider2D>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
        }
        else
        {
            GetComponent<Collider2D>().enabled = true;
            GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}