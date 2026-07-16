using UnityEngine;
using System;
public class BaseHP : MonoBehaviour
{
    public static BaseHP instance;

    public event Action<int, int> OnHpChanged;

    [SerializeField] public int maxHP = 100;

    public int currentHP {  get; private set; }

    public int MaxHP => maxHP;

    private bool isDestroy;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        currentHP = maxHP;

        isDestroy = false;
    }

    public void TakeDamage(int dmg)
    {
        if (isDestroy)
        {
            return;
        }
        if (dmg <= 0)
        {
            Debug.LogWarning(
                $"기지에 잘못된 피해 값이 전달되었습니다: {dmg}"
            );

            return;
        }

        currentHP = Mathf.Max(currentHP - dmg , 0);

        OnHpChanged?.Invoke(currentHP, maxHP);

        Debug.Log(
            $"Base HP: {currentHP}/{maxHP}"
        );

        if (currentHP <= 0)
        {
            DestroyBase();
        }
    }

    private void DestroyBase()
    {
        isDestroy = true;
        Debug.Log("기지가 파괴 되었습니다");

        if(GameManager.instance != null)
        {
            GameManager.instance.GameOver();
        }
    }

    private void OnDestroy()
    {
        if(instance == this)
        {
            instance = null;
        }
    }

}
