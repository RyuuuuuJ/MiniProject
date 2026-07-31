using UnityEngine;

// 골드 확인, 차감, 레벨 상승을 담당
public class TowerUpgrade : MonoBehaviour
{
    [SerializeField] private TowerInfo towerInfo;
    [SerializeField] private float sellRefundRate = 0.7f;
    public TowerInfo TowerInfo => towerInfo;

    public bool CanUpgrade => towerInfo != null && towerInfo.CanUpgrade;

    private void Awake()
    {
        // TowerInfo 자동 검색
        if (towerInfo == null)
        {
            towerInfo = GetComponent<TowerInfo>();
        }
    }

    public int NextUpgradeCost
    {
        get
        {
            if (towerInfo == null || towerInfo.NextLevelData == null)
            {
                return 0;
            }

            return towerInfo.NextLevelData.UpgradeCost;
        }
    }

    // 이후 업그레이드 UI 버튼에서 호출할 함수
    public bool TryUpgrade()
    {
        if (towerInfo == null)
        {
            Debug.LogWarning($"{gameObject.name}에 TowerInfo가 없습니다.");
            return false;
        }

        if (!towerInfo.CanUpgrade)
        {

            return false;
        }

        TowerUpgradeData nextLevelData = towerInfo.NextLevelData;

        if (nextLevelData == null)
        {
            Debug.LogWarning("다음 레벨 데이터가 없습니다.");
            return false;
        }

        if (GoldManager.instance == null)
        {
            Debug.LogWarning("GoldManager가 존재하지 않습니다.");
            return false;
        }

        int upgradeCost = nextLevelData.UpgradeCost;

        // 골드가 부족하면 강화 실패
        if (!GoldManager.instance.SpendGold(upgradeCost))
        {
            return false;
        }

        // 업그레이ㅣ드 적용 실패 시 골드를 돌려줌
        if (!towerInfo.ApplyNextLevel())
        {
            GoldManager.instance.AddGold(upgradeCost);
            return false;
        }

        towerInfo.AddSpentGold(upgradeCost);

        PlaySound.instance?.PlayTowerUpgrade();

        return true;
    }

    public int SellPrice
    {
        get
        {
            if (towerInfo == null)
            {
                return 0;
            }

            return Mathf.FloorToInt(
                towerInfo.TotalSpentGold * sellRefundRate
            );
        }
    }

    // 선택된 타워 판매
    public bool SellTower()
    {
        if (towerInfo == null)
        {
            Debug.LogWarning($"{gameObject.name}에 TowerInfo가 없습니다.");
            return false;
        }

        if (GoldManager.instance == null)
        {
            Debug.LogWarning("GoldManager가 존재하지 않습니다.");
            return false;
        }

        int refundGold = SellPrice;

        // 등록된 buildtile 초기화
        towerInfo.ReleaseBuildCell();

        // 판매 골드 반환
        GoldManager.instance.AddGold(refundGold);


        // 타워와 자식 오브젝트 제거
        Destroy(gameObject);

        return true;
    }

}