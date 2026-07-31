using System;
using UnityEngine;

[Serializable] public class WaveSpwanStart 
{
    [SerializeField]private GameObject monsterPrefab;

    [SerializeField]private int spawnCount = 5;

    [SerializeField]private float spawnInterval = 1f;

    public GameObject MonsterPrefab => monsterPrefab;

    // ObjectPoolManager에서 프리팹 이름을 키로 사용
    public string PoolKey => monsterPrefab != null ? monsterPrefab.name : string.Empty;

    public int SpawnCount => spawnCount;
    public float SpawnInterval => spawnInterval;
}
