using UnityEngine;
using System;

public class GoldManager : MonoBehaviour
{
    public static GoldManager instance {  get; private set; }

    // 게임 시작 시 지급되는 골드
    [SerializeField] private int startingGold;

    public int CurrentGold { get; private set; }

    // 골드가 변경될 때 UI에 알려주는 이벤트
    public event Action<int> OnGoldChanged;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        CurrentGold = Mathf.Max(startingGold, 0);
    }

    //골드 획득
    public void AddGold(int amount)
    {
        if(amount <=0)
        {
            return;
        }

        CurrentGold += amount;
        OnGoldChanged?.Invoke(CurrentGold);

    }

    public bool SpendGold(int amount)
    {
        if (amount <=0)
        {
            return false;
        }

        if(CurrentGold < amount)
        {

            return false;
        }

        CurrentGold -= amount;
        OnGoldChanged?.Invoke(CurrentGold);

        return true;
    }

    private void OnDestroy()
    {
        if(instance == this)
        {
            instance = null;
        }
    }


}
