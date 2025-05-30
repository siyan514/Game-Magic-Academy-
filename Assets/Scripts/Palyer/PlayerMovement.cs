using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rig;
    private PlayerController playerController;
    private float speed = 0.1f;
    //private bool isInjured = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rig = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
    }

    public void AddSpeed(float value = 0.25f)
    {
        speed += value;
        if (speed >= 0.15f) speed = 0.15f;
    }

    private void Update()
    {
        // HandleMovement();
        // HandleBombPlacement();
    }

    public void HandleMovement()
    {
        float h = 0, v = 0;

        if (playerController.PlayerIndex == 1)
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
        }
        else if (playerController.PlayerIndex == 2)
        {
            h = Input.GetAxis("Horizontal2");
            v = Input.GetAxis("Vertical2");
        }

        anim.SetFloat("Horizontal", h);
        anim.SetFloat("Vertical", v);
        rig.MovePosition(transform.position + new Vector3(h, v) * speed);
    }

    public void HandleBombPlacement()
    {
        bool placeBomb = false;
        int playerIndex = playerController.PlayerIndex;

        if (playerIndex == 1 && Input.GetKeyDown(KeyCode.Space))
            placeBomb = true;
        else if (playerIndex == 2 && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
            placeBomb = true;

        if (placeBomb && playerController.BombCount > 0)
        {
            playerController.UseBomb();
            Vector3 bombPos = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
            GameObject bomb = ObjectPool.instance.Get(ObjectType.Bomb, bombPos);
            bomb.GetComponent<BombController>().Init(playerController.Range, playerController.BombTime,
                () => playerController.AddBomb());
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag(Tags.BombEffect))
        {
            playerController.TakeDamage();
        }
    }
}
