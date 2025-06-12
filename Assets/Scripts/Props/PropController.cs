using System.Collections;
using UnityEngine;

/// <summary>
/// prop types class
/// </summary>
public enum PropType
{
    HealthPoint,
    Bomb,
    BombRange,
    Defence, 
    Speed 
}

/// <summary>
/// prop type prefeb class
/// </summary>
[System.Serializable]
public class PropTypePrefab
{
    public PropType type;
    public GameObject propPrefab;
    public string animationName;  // Add animation name field
}

/// <summary>
/// Prop controller classs
/// </summary>
public class PropController : MonoBehaviour
{
    [Header("Prop Configuration")]
    public PropTypePrefab[] propTypePrefabs;

    private GameObject activeProp;
    private PropType propType;
    private SpriteRenderer wallRenderer;
    private Collider2D wallCollider;

    private void Awake()
    {
        wallRenderer = GetComponent<SpriteRenderer>();
        wallCollider = GetComponent<Collider2D>();
    }

    /// <summary>
    /// Reset the prop to its initial wall state
    /// </summary>
    public void ResetProp()
    {
        tag = "Wall";
        gameObject.layer = LayerMask.NameToLayer("wall");
        wallCollider.isTrigger = false;
        wallCollider.enabled = true;

        if (wallRenderer != null)
        {
            wallRenderer.enabled = true;
        }

        if (activeProp != null)
        {
            Destroy(activeProp);
            activeProp = null;
        }
    }

    /// <summary>
    /// Handle trigger enter events for bomb effects
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.BombEffect) && gameObject.CompareTag("Wall"))
        {
            ActivateProp();
        }
    }

    /// <summary>
    /// Activate a random prop when the wall is destroyed
    /// </summary>
    private void ActivateProp()
    {
        // Disable wall properties
        tag = "Prop";
        gameObject.layer = 0; // Default layer
        wallCollider.isTrigger = true;

        if (wallRenderer != null)
        {
            wallRenderer.enabled = false;
        }

        // Select a random prop type
        if (propTypePrefabs == null || propTypePrefabs.Length == 0)
        {
            Debug.LogError("No prop prefabs configured!");
            return;
        }

        int index = Random.Range(0, propTypePrefabs.Length);
        propType = propTypePrefabs[index].type;

        // Instantiate the prop
        activeProp = Instantiate(
            propTypePrefabs[index].propPrefab,
            transform.position,
            Quaternion.identity
        );

        if (activeProp == null)
        {
            Debug.LogError("Failed to instantiate prop prefab");
            return;
        }

        activeProp.transform.SetParent(transform);
        activeProp.transform.localPosition = Vector3.zero;

        // Add and initialize collider handler
        PropColliderHandler propCollider = activeProp.GetComponent<PropColliderHandler>();
        if (propCollider == null)
        {
            propCollider = activeProp.AddComponent<PropColliderHandler>();
        }
        propCollider.Initialize(this);

        // Play prop animation safely
        PlayPropAnimation(activeProp, propTypePrefabs[index].animationName);
    }

    /// <summary>
    /// Safely play prop animation with fallback options
    /// </summary>
    private void PlayPropAnimation(GameObject prop, string animationName)
    {
        if (prop == null) return;

        // Try Animator system first
        Animator animator = prop.GetComponent<Animator>();
        if (animator != null)
        {
            // Check if animator controller is assigned
            if (animator.runtimeAnimatorController == null)
            {
                Debug.LogWarning("Animator controller missing on prop: " + prop.name);
                return;
            }

            // Try playing by animation name
            if (!string.IsNullOrEmpty(animationName))
            {
                animator.Play(animationName, 0, 0f);
                return;
            }

            // Fallback to first animation state
            if (animator.runtimeAnimatorController.animationClips.Length > 0)
            {
                string fallbackClip = animator.runtimeAnimatorController.animationClips[0].name;
                animator.Play(fallbackClip, 0, 0f);
                Debug.LogWarning("Using fallback animation: " + fallbackClip);
            }
            return;
        }

        // Fallback to legacy Animation system
        Animation legacyAnim = prop.GetComponent<Animation>();
        if (legacyAnim != null)
        {
            // Try playing by animation name
            if (!string.IsNullOrEmpty(animationName))
            {
                legacyAnim.Play(animationName);
                return;
            }

            // Fallback to first animation clip
            if (legacyAnim.clip != null)
            {
                legacyAnim.Play();
            }
            return;
        }

        // If no animation components, use simple visual effect
        StartCoroutine(PlayPulseEffect(prop));
    }

    /// <summary>
    /// Fallback visual effect when no animation is available
    /// </summary>
    private IEnumerator PlayPulseEffect(GameObject prop)
    {
        Vector3 originalScale = prop.transform.localScale;
        float duration = 1.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float scaleFactor = Mathf.PingPong(elapsed * 2f, 0.2f) + 0.9f;
            prop.transform.localScale = originalScale * scaleFactor;

            elapsed += Time.deltaTime;
            yield return null;
        }

        prop.transform.localScale = originalScale;
    }

    /// <summary>
    /// Handle prop pickup by a player
    /// </summary>
    public void OnPropPicked(PlayerBase player)
    {
        if (player == null)
        {
            Debug.LogWarning("Prop picked by null player reference");
            return;
        }

        // Apply prop effect based on type
        switch (propType)
        {
            case PropType.HealthPoint:
                player.AddHealth();
                Debug.Log($"Player {player.PlayerIndex} gained health");
                break;
            case PropType.Bomb:
                player.AddBomb();
                Debug.Log($"Player {player.PlayerIndex} gained bomb");
                break;
            case PropType.BombRange:
                player.AddRange();
                Debug.Log($"Player {player.PlayerIndex} increased bomb range");
                break;
            case PropType.Defence:
                player.ActivateInvincibility(8f);
                Debug.Log($"Player {player.PlayerIndex} activated invincibility");
                break;
            case PropType.Speed:
                player.AddSpeed();
                Debug.Log($"Player {player.PlayerIndex} increased speed");
                break;
            default:
                Debug.LogWarning($"Unhandled prop type: {propType}");
                break;
        }

        // Clean up and reset
        ResetProp();
        ObjectPool.instance.Add(ObjectType.Prop, gameObject);
    }
}