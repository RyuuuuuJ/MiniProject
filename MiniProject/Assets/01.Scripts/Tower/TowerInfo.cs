using UnityEngine;

//타워 정보
public class TowerInfo : MonoBehaviour
{
    [SerializeField] private TowerAttack towerAttack;
    [SerializeField] private SpriteRenderer towerRenderer;

    private TowerData towerData;
    private BuildTileSelector tileSelector;
    private Vector3Int buildCell;

    private int currentLevel = 1;
    private int totalSpentGold;

    public TowerData TowerData => towerData;
    public TowerAttack TowerAttack => towerAttack;
    public int CurrentLevel => currentLevel;
    public int MaxLevel => towerData == null ? 0 : towerData.MaxLevel;
    public int TotalSpentGold => totalSpentGold;

    public bool CanUpgrade => towerData != null && currentLevel < towerData.MaxLevel;

    public TowerUpgradeData CurrentLevelData => towerData == null ? null : towerData.GetLevelData(currentLevel);

    public TowerUpgradeData NextLevelData => towerData == null ? null : towerData.GetLevelData(currentLevel + 1);

    private void Awake()
    {
        // TowerAttack은 타워 루트에 존재
        if (towerAttack == null)
        {
            towerAttack = GetComponent<TowerAttack>();
        }

        // TowerBody 자식에 있는 SpriteRenderer 검색
        if (towerRenderer == null)
        {
            towerRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    public void Initialize(TowerData data,Vector3Int cell,BuildTileSelector selector)
    {
        towerData = data;
        buildCell = cell;
        tileSelector = selector;

        currentLevel = 1;
        totalSpentGold = data == null ? 0 : data.BuildCost;

        if (!ApplyCurrentLevelData())
        {
            Debug.LogWarning($"{gameObject.name}의 Lv.1 데이터를 적용하지 못했습니다.");
        }
    }

    // 레벨 데이터 적용
    public bool ApplyNextLevel()
    {
        if (!CanUpgrade)
        {
            return false;
        }

        int previousLevel = currentLevel;
        currentLevel++;

        // 적용 실패 시 기존 레벨로 복구
        if (!ApplyCurrentLevelData())
        {
            currentLevel = previousLevel;
            return false;
        }

        return true;
    }

    // 업그레이드 비용 누적
    public void AddSpentGold(int amount)
    {
        if (amount > 0)
        {
            totalSpentGold += amount;
        }
    }

    // 판매시 해당위치에 다시 건설 가능
    public void ReleaseBuildCell()
    {
        if (tileSelector != null)
        {
            tileSelector.UnregisterTower(buildCell);
        }
    }

    private bool ApplyCurrentLevelData()
    {
        TowerUpgradeData data = CurrentLevelData;

        if (data == null)
        {
            return false;
        }

        if (towerAttack == null)
        {
            Debug.LogWarning($"{gameObject.name}에 TowerAttack이 없습니다.");
            return false;
        }

        towerAttack.ApplyUpgradeData(data);

        // 레벨 이미지가 등록된 경우에만 교체
        if (towerRenderer != null && data.TowerSprite != null)
        {
            towerRenderer.sprite = data.TowerSprite;
        }

        return true;
    }
}
