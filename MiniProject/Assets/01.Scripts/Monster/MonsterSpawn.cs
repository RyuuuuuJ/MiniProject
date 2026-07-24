using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using System;

//몬스터 스폰 관리
public class MonsterSpawn : MonoBehaviour
{
    private Coroutine spawnRoutine;

    public bool IsSpawning => spawnRoutine != null;

    // 몬스터 한 마리가 생성될 때 발생
    public event Action<GameObject> OnMonsterSpawned;

    // 현재 웨이브의 모든 생성이 끝났을 때 발생
    public event Action OnSpawnFinished;


    public bool StartWave(WaveData waveData)
    {
        if (waveData == null)
        {
            Debug.LogError("생성할 WaveData가 없습니다.");
            return false;
        }

        if (spawnRoutine != null)
        {
            Debug.LogWarning("이미 몬스터를 생성하고 있습니다.");
            return false;
        }

        if (ObjectPoolManager.instance == null)
        {
            Debug.LogError("ObjectPoolManager가 없습니다.");
            return false;
        }

        spawnRoutine = StartCoroutine(SpawnWaveRoutine(waveData));

        return true;
    }

    private IEnumerator SpawnWaveRoutine(WaveData waveData)
    {
        WaveSpwanStart[] entries = waveData.SpawnEntries;

        if (entries != null)
        {
            for (int entryIndex = 0;entryIndex < entries.Length; entryIndex++)
            {
                WaveSpwanStart entry = entries[entryIndex];

                if (entry == null || entry.MonsterPrefab == null)
                {
                    Debug.LogWarning(
                        $"WaveData의 {entryIndex}번 몬스터 설정이 비어 있습니다."
                    );

                    continue;
                }

                WaitForSeconds spawnDelay = new WaitForSeconds(entry.SpawnInterval);

                for (int count = 0; count < entry.SpawnCount; count++)
                {
                    GameObject monster =ObjectPoolManager.instance.GetObject(entry.PoolKey,transform.position,Quaternion.identity);

                    if (monster != null)
                    {
                        OnMonsterSpawned?.Invoke(monster);
                    }
             
                    if (count < entry.SpawnCount - 1)
                    {
                        yield return spawnDelay;
                    }
                }
            }
        }

        spawnRoutine = null;

        OnSpawnFinished?.Invoke();
    }

    private void OnDisable()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }
}
