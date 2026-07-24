using UnityEngine;
using System;


//몬스터 웨이브 관리
public class WaveManager : MonoBehaviour
{
    public enum WaveState
    {
        Waiting,    // 시작 대기
        Spawning,   // 몬스터 생성 중
        Fighting,   // 생성 완료, 남은 몬스터 처리 중
        Cleared     // 웨이브 클리어
    }

    [SerializeField] private MonsterSpawn monsterSpawn;

    [SerializeField] private WaveData[] waves;

    private int currentWaveIndex = -1;
    private int aliveMonsterCount;
    private bool spawnFinished;

    public WaveState CurrentState { get; private set; }
    public int AliveMonsterCount => aliveMonsterCount;
    public int CurrentWaveNumber => currentWaveIndex + 1;
    public int TotalWaveCount => waves != null ? waves.Length : 0;

    private int remainingMonsterCount;
    public int RemainingMonsterCount => remainingMonsterCount;

    public event Action<int> OnRemainingMonsterChanged;
    public event Action<int, int> OnWaveChanged;
    public event Action<int> OnAliveMonsterChanged;
    public event Action<WaveState> OnWaveStateChanged;
    public event Action OnAllWavesCleared;

    private void Awake()
    {
        SetState(WaveState.Waiting);
    }

    private void OnEnable()
    {
        if (monsterSpawn == null)
        {
            return;
        }

        monsterSpawn.OnMonsterSpawned += HandleMonsterSpawned;
        monsterSpawn.OnSpawnFinished += HandleSpawnFinished;
    }

    private void OnDisable()
    {
        if (monsterSpawn == null)
        {
            return;
        }

        monsterSpawn.OnMonsterSpawned -= HandleMonsterSpawned;
        monsterSpawn.OnSpawnFinished -= HandleSpawnFinished;
    }

    //웨이브 시작
    public void StartNextWave()
    {
        if (CurrentState == WaveState.Spawning || CurrentState == WaveState.Fighting)
        {
            Debug.Log("이미 웨이브가 진행 중입니다.");
            return;
        }

        if (monsterSpawn == null)
        {
            Debug.LogError("WaveManager에 MonsterSpawn이 연결되지 않았습니다.");
            return;
        }

        int nextWaveIndex = currentWaveIndex + 1;

        if (waves == null || nextWaveIndex >= waves.Length)
        {
            Debug.Log("더 이상 진행할 웨이브가 없습니다.");
            return;
        }

        WaveData nextWave = waves[nextWaveIndex];
        aliveMonsterCount = 0;

        // WaveData의 설정으로 진행
        remainingMonsterCount = nextWave.TotalMonsterCount;

        spawnFinished = false;

        if (nextWave == null)
        {
            Debug.LogError($"{nextWaveIndex + 1}번 WaveData가 없습니다.");
            return;
        }

        currentWaveIndex = nextWaveIndex;
        aliveMonsterCount = 0;
        spawnFinished = false;

        SetState(WaveState.Spawning);

        OnWaveChanged?.Invoke(CurrentWaveNumber, TotalWaveCount);
        OnAliveMonsterChanged?.Invoke(aliveMonsterCount);
        OnRemainingMonsterChanged?.Invoke(remainingMonsterCount);

        Debug.Log(
            $"Wave {CurrentWaveNumber}/{TotalWaveCount} 시작"
        );

        if (!monsterSpawn.StartWave(nextWave))
        {
            SetState(WaveState.Waiting);
        }
    }

    //몬스터 스폰 관리
    private void HandleMonsterSpawned(GameObject monster)
    {
        if (!monster.TryGetComponent(out WaveMonsterTracker tracker))
        {
            Debug.LogError(
                $"{monster.name}에 WaveMonsterTracker가 없습니다."
            );

            // 풀로 몬스터 반환
            ObjectPoolManager.instance.ReturnObject(monster);
            return;
        }

        tracker.Initialize(this);

        aliveMonsterCount++;

        OnAliveMonsterChanged?.Invoke(aliveMonsterCount);
    }
    // 몬스터 스폰 완료
    private void HandleSpawnFinished()
    {
        spawnFinished = true;

        if (aliveMonsterCount > 0)
        {
            SetState(WaveState.Fighting);
        }

        TryClearWave();
    }

    //몬스터 사라짐 안내
    public void NotifyMonsterRemoved()
    {
        if (aliveMonsterCount > 0)
        {
            aliveMonsterCount--;

            OnAliveMonsterChanged?.Invoke(aliveMonsterCount);
        }

        if (remainingMonsterCount > 0)
        {
            remainingMonsterCount--;

            OnRemainingMonsterChanged?.Invoke(remainingMonsterCount);
        }

        TryClearWave();
    }

    // 생성 완료 후 필드에 몬스터가 없으면 클리어
    private void TryClearWave()
    {
        if (CurrentState == WaveState.Cleared)
        {
            return;
        }

        if (!spawnFinished || aliveMonsterCount > 0)
        {
            return;
        }

        WaveData clearedWave = waves[currentWaveIndex];

        if (GoldManager.instance != null)
        {
            GoldManager.instance.AddGold(clearedWave.ClearGoldReward
            );
        }

        SetState(WaveState.Cleared);

        Debug.Log(
            $"Wave {CurrentWaveNumber} Clear! " +
            $"보상 골드: {clearedWave.ClearGoldReward}"
        );

        bool isLastWave = CurrentWaveNumber >= TotalWaveCount;

        if (isLastWave)
        {
            OnAllWavesCleared?.Invoke();
        }
    }
    //State 설정
    private void SetState(WaveState newState)
    {
        CurrentState = newState;
        OnWaveStateChanged?.Invoke(CurrentState);
    }
}
