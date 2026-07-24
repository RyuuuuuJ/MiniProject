using UnityEngine;

public class WaveMonsterTracker : MonoBehaviour
{
    private MonsterHp monsterHp;
    private MonsterMovement monsterMovement;

    private WaveManager waveManager;

    // 중복처리 방지
    private bool hasReported;

    private void Awake()
    {
        monsterHp = GetComponent<MonsterHp>();
        monsterMovement = GetComponent<MonsterMovement>();
    }

    private void OnEnable()
    {
        hasReported = false;
        waveManager = null;

        monsterHp.onDie += HandleMonsterRemoved;
        monsterMovement.OnReachedBase += HandleMonsterRemoved;
    }

    private void OnDisable()
    {
        monsterHp.onDie -= HandleMonsterRemoved;
        monsterMovement.OnReachedBase -= HandleMonsterRemoved;

        waveManager = null;
    }
    public void Initialize(WaveManager manager)
    {
        waveManager = manager;
        hasReported = false;
    }

    private void HandleMonsterRemoved()
    {
        if (hasReported)
        {
            return;
        }

        hasReported = true;

        if (waveManager != null)
        {
            waveManager.NotifyMonsterRemoved();
        }

        waveManager = null;
    }
}
