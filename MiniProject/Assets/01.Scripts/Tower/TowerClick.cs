using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class TowerClick : MonoBehaviour, IPointerClickHandler
{
    private TowerInfo towerInfo;
    private TowerUpgrade towerUpgrade;

    private void Awake()
    {
        towerInfo = GetComponent<TowerInfo>();
        towerUpgrade = GetComponent<TowerUpgrade>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 좌클릭만 사용
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        // 게임 오버, 승리, 일시정지 상태에서는 선택하지 않음
        if (GameManager.instance != null && !GameManager.instance.IsPlaying)
        {
            return;
        }

        if (towerInfo == null || towerUpgrade == null)
        {
            Debug.LogWarning( $"{gameObject.name}의 TowerInfo 또는 TowerUpgrade가 없습니다." );

            return;
        }

        if (TowerUpgradeUI.instance == null)
        {
            Debug.LogWarning("TowerUpgradeUI가 존재하지 않습니다.");
            return;
        }

        TowerUpgradeUI.instance.OpenPanel(towerInfo, towerUpgrade);
    }
}
