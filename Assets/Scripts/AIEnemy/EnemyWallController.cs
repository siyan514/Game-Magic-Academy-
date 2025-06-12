using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// The special destructible wall that contains enemy implementation class
/// </summary>
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
            hiddenEnemy.gameObject.SetActive(false);  // Initial hiding enemy
        }
    }

    /// <summary>
    /// Reset to the initial wall state
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
    }

    /// <summary>
    /// Deal with the bomb effect trigger
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
    /// Activate the enemy
    /// </summary>
    private void ActivateEnemy(PlayerBase bombOwner)
    {
        tag = "Untagged";
        gameObject.layer = 0;
        wallCollider.isTrigger = true;

        if (wallRenderer != null)
        {
            wallRenderer.enabled = false;
        }

        if (hiddenEnemy != null)
        {
            hiddenEnemy.gameObject.SetActive(true);
            hiddenEnemy.transform.SetParent(null); 
            hiddenEnemy.Activate(bombOwner);
        }
        Destroy(gameObject);
    }

}