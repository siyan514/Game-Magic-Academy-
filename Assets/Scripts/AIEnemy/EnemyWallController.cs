using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWallController : MonoBehaviour
{
    private SpriteRenderer wallRenderer;
    private Collider2D wallCollider;
    private GameObject activeEnemy;

    private AIEnemy hiddenEnemy;

    public GameObject enemyPrefab;

    private void Awake()
    {
        wallRenderer = GetComponent<SpriteRenderer>();
        wallCollider = GetComponent<Collider2D>();

        if (enemyPrefab != null)
        {
            GameObject enemyObj = Instantiate(enemyPrefab, transform.position, Quaternion.identity, transform);
            hiddenEnemy = enemyObj.GetComponent<AIEnemy>();
            hiddenEnemy.gameObject.SetActive(false);  // 初始隐藏
        }
    }

    /// <summary>
    /// 重置为初始墙状态
    /// </summary>
    private void ResetEnemyWall()
    {
        tag = "Wall";
        gameObject.layer = LayerMask.NameToLayer("wall");
        wallCollider.isTrigger = false;

        if (wallRenderer != null)
        {
            wallRenderer.enabled = true;
        }

        // if (activeEnemy != null)
        // {
        //     Destroy(activeEnemy);
        //     activeEnemy = null;
        // }
    }

    /// <summary>
    /// 处理炸弹效果触发
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.BombEffect) && gameObject.CompareTag("Wall"))
        {
            PlayerBase bombOwner = null;
            BombEffect bombEffect = collision.GetComponent<BombEffect>();
            if (bombEffect != null)
            {
                bombOwner = bombEffect.GetBombOwner();
            }
            ActivateEnemy(bombOwner);
        }
    }

    /// <summary>
    /// 激活敌人
    /// </summary>
    private void ActivateEnemy(PlayerBase bombOwner)
    {
        tag = "Untagged";
        gameObject.layer = 0; // 默认层
        wallCollider.isTrigger = true;

        if (wallRenderer != null)
        {
            wallRenderer.enabled = false;
        }

        if (hiddenEnemy != null)
        {
            hiddenEnemy.gameObject.SetActive(true);
            hiddenEnemy.transform.SetParent(null);  // 从墙的子物体中脱离
            hiddenEnemy.Activate(bombOwner);
        }

        // // 实例化敌人
        // if (enemyPrefab != null)
        // {
        //     // 使用墙的世界坐标位置
        //     Vector3 spawnPosition = transform.position;

        //     // 添加调试信息
        //     Debug.Log($"Spawning enemy at wall position: {spawnPosition}");

        //     // 实例化敌人
        //     activeEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        //     // 确保敌人位置正确
        //     Debug.Log($"Enemy instantiated at: {activeEnemy.transform.position}");

        //     // 初始化敌人控制器
        //     AIEnemy enemy = activeEnemy.GetComponent<AIEnemy>();
        //     if (enemy != null)
        //     {
        //         enemy.Activate(bombOwner);
        //         print("enemy.Activate(null);");
        //         // 这里可以传递初始位置和炸弹所有者（暂时为null）
        //         // enemy.Initialize(new Vector2Int((int)transform.position.x, (int)transform.position.y), null);
        //     }
        //     Destroy(gameObject);
        // }
        // else
        // {
        //     Debug.LogError("Enemy prefab is not assigned on EnemyWallController");
        // }

        Destroy(gameObject);
    }


    /// <summary>
    /// 敌人被消灭后重置
    /// </summary>
    // public void OnEnemyDefeated()
    // {
    //     ResetEnemyWall();
    //     ObjectPool.instance.Add(ObjectType.EnemyWall, gameObject);
    // }
}