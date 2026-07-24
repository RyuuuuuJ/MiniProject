using UnityEngine;

/// <summary>
/// 번개 타워의 공격을 담당합니다.
/// 첫 번째 몬스터를 공격한 뒤 주변 몬스터에게 공격을 연결합니다.
/// </summary>
public class LightningTower : TowerAttack
{
    private const int SearchBufferSize = 32;
    private const int MaxChainCount = 8;

    // 첫 번째 몬스터를 포함한 최대 공격 횟수
    [SerializeField]
    [Range(1, MaxChainCount)]
    private int chainCount = 3;

    // 다음 몬스터를 찾는 거리
    [SerializeField] private float chainRange = 2.5f;

    // 타워에서 번개가 시작되는 위치
    [SerializeField] private Transform lightningPoint;

    [SerializeField] private LightningAttack[] lightAttack;

    private readonly Collider2D[] searchBuffer = new Collider2D[SearchBufferSize];

    protected override void Attack(MonsterHp target)
    {
        if (!IsValidTarget(target))
        {
            return;
        }

        int maximumHits = Mathf.Min(chainCount, lightAttack.Length);

        if (maximumHits <= 0)
        {
            return;
        }

        Vector3 startPosition =lightningPoint != null? lightningPoint.position: transform.position;

        MonsterHp currentTarget = target;

        for (int i = 0; i < maximumHits; i++)
        {
            if (!IsValidTarget(currentTarget))
            {
                break;
            }

            Vector3 targetPosition = currentTarget.transform.position;

            if (lightAttack[i] != null)
            {
                lightAttack[i].Play(startPosition, targetPosition);
            }

            currentTarget.TakeDamage(attackDamage);

            startPosition = targetPosition;

            currentTarget =FindNextTarget(targetPosition, currentTarget);
        }
    }

    private MonsterHp FindNextTarget(Vector3 searchPosition,MonsterHp excludedTarget)
    {
        int detectedCount = Physics2D.OverlapCircle(searchPosition,chainRange,EnemyFilter,searchBuffer);

        MonsterHp closestTarget = null;

        float closestSqrDistance = float.MaxValue;

        for (int i = 0; i < detectedCount; i++)
        {
            Collider2D detectedCollider = searchBuffer[i];

            if (detectedCollider == null)
            {
                continue;
            }

            MonsterHp candidate = detectedCollider.GetComponentInParent<MonsterHp>();

            // 방금 공격한 몬스터는 바로 다시 선택 X
            if (!IsValidTarget(candidate) || candidate == excludedTarget)
            {
                continue;
            }

            Vector3 direction = candidate.transform.position - searchPosition;

            float sqrDistance = direction.sqrMagnitude;

            if (sqrDistance < closestSqrDistance)
            {
                closestSqrDistance = sqrDistance;
                closestTarget = candidate;
            }
        }

        return closestTarget;
    }

    private bool IsValidTarget(MonsterHp target)
    {
        return target != null && !target.isDead && target.gameObject.activeInHierarchy;
    }
}