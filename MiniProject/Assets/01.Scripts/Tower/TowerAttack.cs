using System.Net;
using System.Threading;
using UnityEngine;

//타워가 공격할 상대와 시간을 결정
//공격 범위 관리
//범위 안의 몬스터 탐색
//가장 가까운 몬스터 선택
//공격 간격 계산
//공격할 시간이 되면 Attack() 호출
public abstract class TowerAttack : MonoBehaviour
{
    // 한 번에 탐색할 수 있는 최대 Collider 수
    private const int DetectionBufferSize = 32;

    [SerializeField] float attackRange = 3f;
    [SerializeField] float attackInterval = 1f;
    [SerializeField] protected int attackDamage = 10;
    [SerializeField] LayerMask enemyLayer;

    //공격 간격
    private float attackTimer;

    private readonly Collider2D[] detectionBuffer = new Collider2D[DetectionBufferSize];

    private ContactFilter2D enemyFilter;

    protected LayerMask EnemyLayer => enemyLayer;

    protected ContactFilter2D EnemyFilter => enemyFilter;
    protected MonsterHp CurrentTarget { get; private set; }
    protected virtual void Awake()
    {
        enemyFilter = new ContactFilter2D();

        enemyFilter.SetLayerMask(enemyLayer);
        enemyFilter.useTriggers = true;
    }

    protected virtual void OnEnable()
    {
        attackTimer = 0f;
        CurrentTarget = null;
    }

    protected virtual void Update()
    {
        // 게임이 Playing 상태가 아닐 때만 공격을 중지합니다.
        if (GameManager.instance != null && !GameManager.instance.IsPlaying)
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
        int detectedCount = Physics2D.OverlapCircle(transform.position,attackRange,enemyFilter,detectionBuffer);

        MonsterHp closestMonster = null;

        float closestSqrDistance = float.MaxValue;

        for (int i = 0; i < detectedCount; i++)
        {
            Collider2D detectedCollider = detectionBuffer[i];

            if (detectedCollider == null)
            {
                continue;
            }

            MonsterHp monster =
                detectedCollider.GetComponentInParent<MonsterHp>();

            if (monster == null || monster.isDead || !monster.gameObject.activeInHierarchy)
            {
                continue;
            }

            Vector3 direction =monster.transform.position - transform.position;

            float sqrDistance = direction.sqrMagnitude;

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
