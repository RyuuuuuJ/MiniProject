using UnityEngine;

public class MonsterHp : MonoBehaviour
{
    [SerializeField] int maxHp;

    public int currentHp {  get; private set; }

    public bool isDead { get; private set; }

    private void OnEnable()
    {
        currentHp = maxHp;
        isDead = false;
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

        Debug.Log(
            $"{gameObject.name} HP: {currentHp}/{maxHp}"
        );

        //체력 없을시 사망 처리
        if (currentHp <= 0)
        {
            Die();
        }

    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }
        isDead = true;

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
