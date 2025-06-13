using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// AI enemy implementation class
/// </summary>
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
    public bool enablePathDrawing = true;
    public Color pathColor = Color.red;

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

        if (rig != null) rig.simulated = false;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        if (spriteRenderer != null) spriteRenderer.enabled = false;
    }

    /// <summary>
    /// Initialize the AI enemy
    /// </summary>
    /// <param name="position"></param>
    /// <param name="owner"></param>
    public void Initialize(Vector2Int position, PlayerBase owner)
    {
        gridPosition = position;
        bombOwner = owner;
        transform.position = new Vector3(position.x, position.y, 0);
    }


    /// <summary>
    /// get the target player, change the enemy state to active
    /// </summary>
    /// <param name="owner">The player who destroys the current obstacle</param>
    public void Activate(PlayerBase owner)
    {
        bombOwner = owner;
        if (currentState == EnemyState.Hidden)
        {
            SetState(EnemyState.Active);

            if (rig != null) rig.simulated = true;
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = true;

            StartCoroutine(ActivationRoutine());
        }
    }

    /// <summary>
    /// If the target palyer is alive than chase the target player
    /// </summary>
    /// <returns></returns>
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
        //update the chase path
        UpdatePath();
    }

    /// <summary>
    /// find the target player
    /// </summary>
    private void FindTargetPlayer()
    {
        if (bombOwner != null && bombOwner.IsActive)
        {
            targetPlayer = bombOwner.transform;
            return;
        }
        else
        {
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

            //update the path
            if (Time.time - lastPathUpdate > pathUpdateInterval)
            {
                UpdatePath();
            }

            // move along the path
            MoveAlongPath();
        }
        else if (currentState == EnemyState.Wandering)
        {
            if (canMove)
            {
                transform.position += (Vector3)dirVector * wanderingSpeed * Time.deltaTime;
                HandleFacing(dirVector);

                //update the animation
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
    /// <summary>
    /// provide the direction of wander state
    /// </summary>
    /// <param name="dir"></param>
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
    /// Update the path to the target player
    /// </summary>
    private void UpdatePath()
    {
        if (targetPlayer == null) return;

        lastPathUpdate = Time.time;

        // Obtain the current grid position and the target grid position
        Vector2Int currentGridPos = new Vector2Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.y)
        );

        Vector2Int targetGridPos = new Vector2Int(
            Mathf.RoundToInt(targetPlayer.position.x),
            Mathf.RoundToInt(targetPlayer.position.y)
        );

        // If the target position changes, recalculate the path
        if (targetGridPos != targetGridPosition || currentPath == null || currentPath.Count == 0)
        {
            targetGridPosition = targetGridPos;
            currentPath = AStarPathfinding.FindPath(currentGridPos, targetGridPos);
            currentWaypointIndex = 0;

            // If the starting point is in the path, skip it
            if (currentPath != null && currentPath.Count > 0 && currentPath[0] == currentGridPos)
            {
                currentWaypointIndex = 1;
            }
        }
    }

    /// <summary>
    /// Move along the calculated path
    /// </summary>
    private void MoveAlongPath()
    {
        if (currentPath == null || currentPath.Count == 0) return;
        if (currentWaypointIndex >= currentPath.Count) return;

        Vector2Int targetWaypoint = currentPath[currentWaypointIndex];
        Vector3 targetWorldPos = new Vector3(targetWaypoint.x, targetWaypoint.y, 0);

        // Calculate the direction of movement
        Vector3 direction = (targetWorldPos - transform.position).normalized;

        // Move to the target point
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetWorldPos,
            moveSpeed * Time.deltaTime
        );

        HandleFacing(direction);

        // Update the animation parameters
        if (anim != null)
        {
            anim.SetFloat("Horizontal", direction.x);
            anim.SetFloat("Vertical", direction.y);
        }

        // Check whether the current path point has been reached
        float distanceToWaypoint = Vector3.Distance(transform.position, targetWorldPos);
        if (distanceToWaypoint < 0.1f)
        {
            currentWaypointIndex++;
        }
    }
    /// <summary>
    /// handle the face of enemy
    /// </summary>
    /// <param name="moveDirection"></param>
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
    /// Draw the path (only when enabled)
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, 1, 0));
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -1, 0));
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(1, 0, 0));
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(-1, 0, 0));

        if (!enablePathDrawing || currentPath == null || currentPath.Count == 0) return;

        Gizmos.color = pathColor;

        // Draw the path line segments
        for (int i = 0; i < currentPath.Count - 1; i++)
        {
            Vector3 start = new Vector3(currentPath[i].x, currentPath[i].y, 0);
            Vector3 end = new Vector3(currentPath[i + 1].x, currentPath[i + 1].y, 0);
            Gizmos.DrawLine(start, end);
        }

        // Draw the path points
        for (int i = 0; i < currentPath.Count; i++)
        {
            Vector3 point = new Vector3(currentPath[i].x, currentPath[i].y, 0);

            if (i == currentWaypointIndex)
            {
                // The current target point is in different colors
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
    /// <summary>
    /// handle the trggier
    /// </summary>
    /// <param name="collision"></param>
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
    /// <summary>
    /// change the direction of wander state
    /// </summary>
    private void WanderChangeDir()
    {
        List<int> possibleDirs = new List<int>();

        // Check whether the four directions are navigable
        if (!Physics2D.Raycast(transform.position, Vector2.up, rayDistance, 1 << 6)) possibleDirs.Add(0);
        if (!Physics2D.Raycast(transform.position, Vector2.down, rayDistance, 1 << 6)) possibleDirs.Add(1);
        if (!Physics2D.Raycast(transform.position, Vector2.left, rayDistance, 1 << 6)) possibleDirs.Add(2);
        if (!Physics2D.Raycast(transform.position, Vector2.right, rayDistance, 1 << 6)) possibleDirs.Add(3);

        // If the current direction is still feasible, give priority to maintaining it (avoid frequent turns)
        if (possibleDirs.Contains(dirID))
        {
            return;
        }
        // Otherwise, randomly select a new direction
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

    /// <summary>
    /// change the state to dead
    /// </summary>
    public void Die()
    {
        currentState = EnemyState.Dead;

        SetState(EnemyState.Dead);

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;

        if (rig != null) rig.simulated = false;

        Destroy(gameObject, 0.1f);

    }
    /// <summary>
    /// set the current state of enemy
    /// </summary>
    /// <param name="newState"></param>
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
