using UnityEngine;
using UnityEngine.UI;

public class MonsterHpBar : MonoBehaviour
{
    [SerializeField] private MonsterHp monsterHp;
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject barVisual;
    [SerializeField] private bool hideWhenFull = true;

    private void Awake()
    {
        if (monsterHp == null)
        {
            monsterHp = GetComponentInParent<MonsterHp>();
        }
    }

    private void OnEnable()
    {
        if (monsterHp == null)
        {
            return;
        }

        monsterHp.OnHpChanged += RefreshHpBar;

        RefreshHpBar(monsterHp.currentHp, monsterHp.MaxHp);
    }

    private void OnDisable()
    {
        if (monsterHp != null)
        {
            monsterHp.OnHpChanged -= RefreshHpBar;
        }
    }

    private void RefreshHpBar(int currentHp, int maxHp)
    {
        if (fillImage == null)
        {
            return;
        }

        float hpRatio = maxHp > 0? Mathf.Clamp01((float)currentHp / maxHp): 0f;

        fillImage.fillAmount = hpRatio;

        if (barVisual != null)
        {
            bool shouldShow =!hideWhenFull || (currentHp > 0 && hpRatio < 1f);

            barVisual.SetActive(shouldShow);
        }
    }
}
