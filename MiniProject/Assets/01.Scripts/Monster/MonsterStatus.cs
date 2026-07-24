using UnityEngine;

//몬스터에게 적용된 상태이상
public class MonsterStatus : MonoBehaviour
{
    private MonsterMovement monsterMove;

    public bool isSlowed;
    private float slowEnd;

    private void Awake()
    {
        TryGetComponent(out monsterMove);
    }

    private void OnEnable()
    {
        ResetEffect();
    }

    private void OnDisable()
    {
        ResetEffect();
    }

    private void Update()
    {
        if(isSlowed && Time.time >= slowEnd)
        {
            RemoveSlow();
        }
    }

    public void ApplySlow(float speedMultiplier, float duration)
    {
        if(monsterMove == null || duration <= 0f)
        {
            return;
        }

        isSlowed = true;

        slowEnd = Time.time + duration;

        monsterMove.SetSpeedMultiplier(speedMultiplier);
    }

    private void RemoveSlow()
    {
        isSlowed = false;
        slowEnd = 0f;

        if(monsterMove != null)
        {
            monsterMove.ResetSpeedMultiplier();
        }
    }

    private void ResetEffect()
    {
        isSlowed = false;
        slowEnd = 0f;

        if (monsterMove != null)
        {
            monsterMove.ResetSpeedMultiplier();
        }
    }
}
