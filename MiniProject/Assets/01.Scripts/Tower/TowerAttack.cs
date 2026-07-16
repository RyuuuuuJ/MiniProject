using System.Net;
using System.Threading;
using UnityEngine;

public abstract class TowerAttack : MonoBehaviour
{
    [SerializeField] float attackRange = 3f;
    [SerializeField] float attackInterval = 1f;
    [SerializeField] protected int attackDamage = 10;
    [SerializeField] LayerMask enemyLayer;

    //공격 간격
    private float attackTimer;

    protected MonsterHp CurrentTarget { get; private set; }

    protected virtual void OnEnable()
    {
        attackTimer = 0f;
        CurrentTarget = null;
    }

    protected virtual void Update()
    {
        // 게임이 Playing 상태가 아닐 때만 공격을 중지합니다.
        if (GameManager.instance != null &&
            !GameManager.instance.IsPlaying)
        {
            return;
        }

       
        attackTimer -= Time.deltaTime;

        
        CurrentTarget = FindClosestTarget();

        
        if (CurrentTarget == null)
        {
            return;
        }

       
        if (attackTimer > 0f)
        {
            return;
        }
      
        Attack(CurrentTarget);
       
        attackTimer = attackInterval;

    }

    private MonsterHp FindClosestTarget()
    {
        Collider2D[] detectedEnemy = Physics2D.OverlapCircleAll(transform.position,attackRange,enemyLayer);

        MonsterHp closestMonster = null;

        float closestSqrDistance = float.MaxValue;

        foreach (Collider2D detectedCollider in detectedEnemy)
        {
            MonsterHp monster = detectedCollider.GetComponentInParent<MonsterHp>();


            if (monster == null || monster.isDead)
            {
                continue;
            }

            Vector3 direction =
                    monster.transform.position - transform.position;

            float sqrDistance = direction.sqrMagnitude;

            // 지금까지 찾은 몬스터보다 가까우면 타깃을 교체합니다.
            if (sqrDistance < closestSqrDistance)
            {
                closestSqrDistance = sqrDistance;
                closestMonster = monster;
            }
        }
        return closestMonster;
    }



    protected abstract void Attack(MonsterHp target);

    private void OnDrawGizmosSelected()
    {
        // Scene 창에서 선택한 타워의 공격 범위를 확인합니다.
        Gizmos.color = new Color(1f, 0.35f, 0.1f, 1f);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
