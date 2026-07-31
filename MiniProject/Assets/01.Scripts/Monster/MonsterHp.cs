using System;
using UnityEngine;


//몬스터 HP
public class MonsterHp : MonoBehaviour
{
    [SerializeField] int maxHp;

    public int currentHp {  get; private set; }

    public bool isDead { get; private set; }

    public MonsterStatus statusEffect { get; private set; }

    public event Action onDie;

    public event Action<int, int> OnHpChanged;
    public int MaxHp => maxHp;

    private void Awake()
    {
        TryGetComponent(out MonsterStatus status);

        statusEffect = status;
    }

    private void OnEnable()
    {
        currentHp = maxHp;
        isDead = false;

        OnHpChanged?.Invoke(currentHp, maxHp);
    }

    public void TakeDamage(int dmg)
    {
        if(isDead)
        {
            return;
        }
        if (dmg <= 0)
        {
            Debug.LogWarning(
                $"{gameObject.name} 잘못된 피해 값이 전달되었습니다: {dmg}"
            );

            return;
        }

        currentHp = Mathf.Max(currentHp - dmg, 0);

        OnHpChanged?.Invoke(currentHp, maxHp);

        //체력 없을시 사망 처리
        if (currentHp <= 0)
        {

            if (PlaySound.instance != null)
            {
                PlaySound.instance.PlayMonsterDeath();
            }

            Die();
        }

        else
        {
            if (PlaySound.instance != null)
            {
                PlaySound.instance.PlayMonsterHit();
            }
        }

    }

    //사망처리
    private void Die()
    {
        if (isDead)
        {
            return;
        }
        isDead = true;

        onDie?.Invoke();

        if(ObjectPoolManager.instance != null)
        {
            ObjectPoolManager.instance.ReturnObject(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }



    }

}
