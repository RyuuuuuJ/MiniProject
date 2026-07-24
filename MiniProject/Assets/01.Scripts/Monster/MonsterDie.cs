using UnityEngine;

public class MonsterDie : MonoBehaviour
{
    [SerializeField] private int getGold = 10;

    private MonsterHp monsterHp;

    private void Awake()
    {
        monsterHp = GetComponent<MonsterHp>();
    }

    private void OnEnable()
    {
        monsterHp.onDie += GiveGold;
    }
    private void OnDisable()
    {
        monsterHp.onDie -= GiveGold;
    }
    private void GiveGold()
    {
        if (GoldManager.instance == null)
        {
            return;
        }

        GoldManager.instance.AddGold(getGold);
    }
}
