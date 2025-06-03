// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class AIPlayer : PlayerBase
// {
//     private AIController aiController;
//     private List<Vector2> accessiblePoints = new List<Vector2>();


//     protected override void Awake()
//     {
//         base.Awake();
//         aiController = gameObject.GetComponent<AIController>();
//     }

//     public override void Init(int range, int hp, float bombTime, int index)
//     {
//         base.Init(range, hp, bombTime, index);
//         aiController.Init(this);
//     }

//     // 执行移动指令
//     public void ExecuteMove(Vector2 direction)
//     {
//         if (!IsActive) return;
//         Move(direction);
//     }

//     // 执行放置炸弹指令
//     public void ExecutePlaceBomb()
//     {
//         if (!IsActive) return;
//         PlaceBomb();
//     }

//     public override void Move(Vector2 direction)
//     {
//         // 应用移动
//         rig.MovePosition(rig.position + direction * speed);

//         // 更新动画参数
//         anim.SetFloat("Horizontal", direction.x);
//         anim.SetFloat("Vertical", direction.y);
//     }

//     public override void PlaceBomb()
//     {
//         if (BombCount <= 0) return;

//         UseBomb();
//         Vector3 bombPos = new Vector3(
//             Mathf.RoundToInt(transform.position.x),
//             Mathf.RoundToInt(transform.position.y)
//         );

//         GameObject bomb = ObjectPool.instance.Get(ObjectType.Bomb, bombPos);
//         bomb.GetComponent<BombController>().Init(Range, BombTime, () => AddBomb());
//     }

//     public override void TakeDamage()
//     {
//         if (IsInvincible || !IsActive) return;

//         HP--;

//         if (HP <= 0)
//         {
//             Die();
//         }
//         else
//         {
//             StartCoroutine(DisableControlRoutine(1f));
//             ActivateInvincibility(3f, false);
//         }
//     }

//     private void OnTriggerEnter2D(Collider2D collider)
//     {
//         if (collider.CompareTag(Tags.BombEffect))
//         {
//             TakeDamage();
//         }
//     }
// }