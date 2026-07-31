using UnityEngine;

public class MonsterDeathEffectSpawner : MonoBehaviour
{
    [SerializeField] private string effectPoolKey = "MonsterDieEffect";

    [SerializeField] private Vector3 spawnOffset;

    private MonsterHp monsterHp;

    private void Awake()
    {
        monsterHp = GetComponent<MonsterHp>();
    }

    private void OnEnable()
    {
        if (monsterHp != null)
        {
            monsterHp.onDie += SpawnDeathEffect;
        }
    }

    private void OnDisable()
    {
        if (monsterHp != null)
        {
            monsterHp.onDie -= SpawnDeathEffect;
        }
    }

    private void SpawnDeathEffect()
    {
        if (ObjectPoolManager.instance == null)
        {
            return;
        }

        Vector3 spawnPosition = transform.position + spawnOffset;

        ObjectPoolManager.instance.GetObject(effectPoolKey, spawnPosition, Quaternion.identity
        );
    }
}
