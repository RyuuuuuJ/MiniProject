using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] TMP_Text baseHPText;
    [SerializeField] Image HpBar;

    void Start()
    {
        gameOverPanel.SetActive(false);

        //Base 가 씬에 존재하는지 확인
        if(BaseHP.instance == null)
        {
            return;
        }

        BaseHP.instance.OnHpChanged += updateHpChanged;

        updateHpChanged(BaseHP.instance.currentHP, BaseHP.instance.maxHP);
    }

    //Hp 변경
    private void updateHpChanged(int currentHp, int maxHp)
    {
        baseHPText.text = $"BASE HP  {currentHp} / {maxHp}";

        float hpRatio = (float)currentHp / maxHp;

        // 계산한 비율만큼 HP 바를 표시합니다.
        HpBar.fillAmount = hpRatio;

        // HP가 0이 되면 Game Over 패널을 표시합니다.
        if (currentHp <= 0)
        {
            gameOverPanel.SetActive(true);
        }
    }
    private void OnDestroy()
    {
        if(BaseHP.instance != null)
        {
            BaseHP.instance.OnHpChanged -= updateHpChanged;
        }
    }
}
