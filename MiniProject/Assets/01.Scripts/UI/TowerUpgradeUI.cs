using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 선택된 타워의 정보를 표시하고 업그레이드를 요청
public class TowerUpgradeUI : MonoBehaviour
{
    public static TowerUpgradeUI instance { get; private set; }

    [SerializeField] private GameObject upgradePanel;
    
    [SerializeField] private Image towerImage;
    [SerializeField] private TMP_Text towerNameText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text statText;
    [SerializeField] private TMP_Text upgradeCostText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button sellButton;
    [SerializeField] private TMP_Text sellPriceText;
    [SerializeField] private BuildTileSelector tileSelector;

    private TowerInfo selectedTower;
    private TowerUpgrade selectedUpgrade;
    private TowerClickUI towerClicked;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (tileSelector != null)
        {
            tileSelector.OnCellSelected += ClosePanel;
        }
    }

    private void OnDisable()
    {
        if (tileSelector != null)
        {
            tileSelector.OnCellSelected -= ClosePanel;
        }
    }

    // TowerSelection에서 호출
    public void OpenPanel( TowerInfo towerInfo, TowerUpgrade towerUpgrade)
    {

        if (towerInfo == null || towerUpgrade == null)
        {
            return;
        }

        // 전에 선택한거 해제
        if (tileSelector != null)
        {
            tileSelector.CancelSelection();
        }

        if (towerClicked != null)
        {
            towerClicked.SetSelected(false);
        }

        selectedTower = towerInfo;
        selectedUpgrade = towerUpgrade;

        towerClicked = towerInfo.GetComponent<TowerClickUI>();

        // 새로 선택한 타워의 표시를 켭니다.
        if (towerClicked != null)
        {
            towerClicked.SetSelected(true);
        }

        upgradePanel.SetActive(true);
        RefreshUI();
    }

    // Upgrade 버튼에서 호출
    public void UpgradeSelectedTower()
    {
        if (selectedUpgrade == null)
        {
            return;
        }

        bool upgradeSucceeded = selectedUpgrade.TryUpgrade();

        if (upgradeSucceeded && towerClicked != null)
        {
            towerClicked.RefreshIndicatorSize();
        }

        RefreshUI();
    }

    // Close 버튼에서 호출
    public void ClosePanel()
    {
        // 선택된 타워의 표시를 끔
        if (towerClicked != null)
        {
            towerClicked.SetSelected(false);
        }

        towerClicked = null;
        selectedTower = null;
        selectedUpgrade = null;

        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }
    }

    private void RefreshUI()
    {
        if (selectedTower == null || selectedUpgrade == null)
        {
            ClosePanel();
            return;
        }

        TowerData towerData = selectedTower.TowerData;
        TowerUpgradeData levelData = selectedTower.CurrentLevelData;

        if (towerNameText != null)
        {
            towerNameText.text = towerData == null ? "TOWER" : towerData.TowerName;
        }

        if (levelText != null)
        {
            levelText.text =$"LV.{selectedTower.CurrentLevel} / " + $"{selectedTower.MaxLevel}";
        }

        // 현재 레벨의 실제 타워 이미지 표시
        if (towerImage != null)
        {
            Sprite sprite = levelData == null ? null : levelData.TowerSprite;

            towerImage.sprite = sprite;
            towerImage.enabled = sprite != null;
        }

        TowerAttack towerAttack = selectedTower.TowerAttack;

        if (statText != null && towerAttack != null)
        {
            // 초당 공격 횟수
            float attacksPerSecond = 1f / towerAttack.AttackInterval;

            statText.text =
                $"DMG  {towerAttack.AttackDamage}\n" +
                $"SPD  {attacksPerSecond:0.0}/s\n" +
                $"RANGE  {towerAttack.AttackRange:0.0}";
        }

        bool canUpgrade = selectedUpgrade.CanUpgrade;

        if (upgradeButton != null)
        {
            upgradeButton.interactable = canUpgrade;
        }

        if (upgradeCostText != null)
        {
            upgradeCostText.text = canUpgrade ? $"UPGRADE  {selectedUpgrade.NextUpgradeCost} G": "MAX LEVEL";
        }

        if (sellButton != null)
        {
            sellButton.interactable = true;
        }

        if (sellPriceText != null)
        {
            sellPriceText.text = $"SELL  {selectedUpgrade.SellPrice} G";
        }
    }

    public void SellSelectedTower()
    {
        if (selectedUpgrade == null)
        {
            return;
        }

        TowerUpgrade towerToSell = selectedUpgrade;

        ClosePanel();

        towerToSell.SellTower();
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}