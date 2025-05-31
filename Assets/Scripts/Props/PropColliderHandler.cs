using UnityEngine;

public class PropColliderHandler : MonoBehaviour
{
    private PropController parentController;
    private BoxCollider2D propCollider;

    /// <summary>
    /// Initialize the collider handler with the parent controller
    /// </summary>
    /// <param name="controller">The parent prop controller</param>
    public void Initialize(PropController controller)
    {
        parentController = controller;
        propCollider = gameObject.AddComponent<BoxCollider2D>();
        propCollider.isTrigger = true;
        propCollider.size = new Vector2(0.5f, 0.5f);
    }

    /// <summary>
    /// Handle trigger enter events for player collision
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.Player))
        {
            PlayerBase player = collision.GetComponent<PlayerBase>();
            if (player != null)
            {
                parentController.OnPropPicked(player);
            }
        }
    }
}