using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    //BaseHp UI
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] TMP_Text baseHPText;
    [SerializeField] Image HpBar;

    //Gold UI
    [SerializeField] TMP_Text goldText;

    void Start()
    {
        gameOverPanel.SetActive(false);

        
        if(BaseHP.instance != null)
        {
            BaseHP.instance.OnHpChanged += updateHpChanged;

            updateHpChanged(BaseHP.instance.currentHP, BaseHP.instance.maxHP);

        }

      

        if(GoldManager.instance != null)
        {
            GoldManager.instance.OnGoldChanged += UpdateGoldText;

            UpdateGoldText(GoldManager.instance.CurrentGold);
        }
    }

    //Hp 변경
    private void updateHpChanged(int currentHp, int maxHp)
    {
        baseHPText.text = $"{currentHp} / {maxHp}";

        float hpRatio = maxHp > 0? (float)currentHp / maxHp : 0f;

        // 계산한 비율만큼 HP 바를 표시합니다.
        HpBar.fillAmount = hpRatio;

        // HP가 0이 되면 Game Over 패널을 표시합니다.
        if (currentHp <= 0)
        {
            gameOverPanel.SetActive(true);
        }
    }

    private void UpdateGoldText(int currentGold)
    {
        goldText.text = currentGold.ToString();
    }
    private void OnDestroy()
    {
        if(BaseHP.instance != null)
        {
            BaseHP.instance.OnHpChanged -= updateHpChanged;
        }

        if(GoldManager.instance != null)
        {
            GoldManager.instance.OnGoldChanged -= UpdateGoldText;
        }
    }
}
