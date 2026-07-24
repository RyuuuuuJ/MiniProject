using UnityEngine;

[CreateAssetMenu(fileName = "Wave_01",menuName = "Tower Defence/Wave Data")]
public class WaveData : ScriptableObject
{
    [SerializeField]
    private WaveSpwanStart[] spawnEntries;

    [SerializeField, Min(0)]
    private int clearGoldReward = 50;

    public WaveSpwanStart[] SpawnEntries => spawnEntries;
    public int ClearGoldReward => clearGoldReward;

    // 웨이브에서 생성할 전체 몬스터 수를 계산
    public int TotalMonsterCount
    {
        get
        {
            if (spawnEntries == null)
            {
                return 0;
            }

            int totalCount = 0;

            for (int i = 0; i < spawnEntries.Length; i++)
            {
                WaveSpwanStart entry = spawnEntries[i];

                if (entry != null && entry.MonsterPrefab != null)
                {
                    totalCount += entry.SpawnCount;
                }
            }

            return totalCount;
        }
    }
}