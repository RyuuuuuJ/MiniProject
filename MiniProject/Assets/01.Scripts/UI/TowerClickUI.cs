using UnityEngine;

public class TowerClickUI : MonoBehaviour
{
    [SerializeField] private GameObject selectionIndicator;

    [SerializeField] private SpriteRenderer towerRenderer;

    private Transform indicatorTransform;
    private Vector3 originalIndicatorScale;
    private float originalTowerWidth;


    private void Awake()
    {
        if (selectionIndicator == null)
        {
            selectionIndicator.SetActive(false);
        }

        indicatorTransform = selectionIndicator.transform;
        originalIndicatorScale = indicatorTransform.localScale;

        // 1레벨 타워의 가로 크기를 저장
        originalTowerWidth = GetTowerWidth();

        selectionIndicator.SetActive(false);
    }

    public void SetSelected(bool isSelected)
    {
        if (selectionIndicator == null)
        {
           return;
        }

        if (isSelected)
        {
            RefreshIndicatorSize();
        }

        selectionIndicator.SetActive(isSelected);
    }

    // 업그레이드 후 선택 표시 크기를 다시 계산
    public void RefreshIndicatorSize()
    {
        if (indicatorTransform == null || towerRenderer == null || towerRenderer.sprite == null || originalTowerWidth <= 0f)
        {
            return;
        }

        float currentTowerWidth = GetTowerWidth();
        float sizeRatio = currentTowerWidth / originalTowerWidth;

        indicatorTransform.localScale = new Vector3(originalIndicatorScale.x * sizeRatio, originalIndicatorScale.y * sizeRatio, originalIndicatorScale.z);
    }

    private float GetTowerWidth()
    {
        if (towerRenderer == null || towerRenderer.sprite == null)
        {
            return 0f;
        }

        return towerRenderer.sprite.bounds.size.x * Mathf.Abs(towerRenderer.transform.localScale.x);
    }
}
