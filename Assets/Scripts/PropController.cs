using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PropType
{
    HealthPoint,
    Bomb,
    BombRange,
    defence,
    speed
}

[System.Serializable]
public class PropType_Sprite
{
    public PropType type;
    public Sprite sp;
}

public class PropController : MonoBehaviour
{
    public PropType_Sprite[] propType_Sprites;
    private Sprite defultSp;
    private SpriteRenderer spriteRenderer;
    private PropType propType;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defultSp = spriteRenderer.sprite;
    }

    /// <summary>
    /// Reset all the prop
    /// </summary>
    private void ResetProp()
    {
        tag = "Wall";
        gameObject.layer = 6;
        GetComponent<Collider2D>().isTrigger = false;
        spriteRenderer.sprite = defultSp;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.BombEffect))
        {
            tag = "Untagged";
            gameObject.layer = 0;
            GetComponent<Collider2D>().isTrigger = true;
            int index = Random.Range(0, propType_Sprites.Length);
            spriteRenderer.sprite = propType_Sprites[index].sp;
            propType = propType_Sprites[index].type;

            StartCoroutine(PropAni());
        }
        //Contact the player and enhance the effect based on the type of the item.
        if (collision.CompareTag(Tags.Player))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();

            switch (propType) 
            {
                case PropType.HealthPoint:
                    playerController.HP++;
                    break;
                case PropType.Bomb:
                    playerController.bombCount++;
                    break; 
                case PropType.BombRange:
                    playerController.range++;
                    break; 
                case PropType.defence:
                    playerController.ActivateInvincibility(8f); // Activate 8-second invincibility
                    break;
                case PropType.speed:
                    playerController.AddSpeed();
                    break;
                default: 
                    break;
            }
            ResetProp();
            ObjectPool.instance.Add(ObjectType.Prop, gameObject);
        }
    }

    IEnumerator PropAni()
    {
        for (int i = 0; i < 2; i++)
        {
            spriteRenderer.color = Color.yellow;
            yield return new WaitForSeconds(0.25f);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.25f);
        }
    }
}
