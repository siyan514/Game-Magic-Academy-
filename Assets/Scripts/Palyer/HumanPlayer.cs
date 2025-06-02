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

        // Handle movement input
        Vector2 moveInput = inputHandler.GetMovementInput();

        anim.SetFloat("Horizontal", moveInput.x);
        anim.SetFloat("Vertical", moveInput.y);

        if (moveInput != Vector2.zero)
        {
            Move(moveInput);
        }

        // Handle bomb placement
        if (inputHandler.GetBombInput())
        {
            PlaceBomb();
        }
    }

    public override void Move(Vector2 direction)
    {
        // Apply movement
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
            Die(); // Call death method
        }
        else
        {
            // Stun for 1 second
            StartCoroutine(DisableControlRoutine(1f));
            // Activate 3-second gray invincibility effect
            ActivateInvincibility(3f, false);
        }
    }

    protected override IEnumerator DisappearAfterDelay(float delay)
    {
        yield return base.DisappearAfterDelay(delay);

    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag(Tags.BombEffect))
        {
            TakeDamage();
        }
    }
}