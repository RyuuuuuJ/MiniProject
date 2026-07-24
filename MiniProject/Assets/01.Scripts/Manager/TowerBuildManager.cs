using UnityEngine;

public class TowerBuildManager : MonoBehaviour
{
    [SerializeField] private BuildTileSelector tileSelector;

    [SerializeField] private GameObject buildPanel;

    [SerializeField] private Transform towersParent;

    private void Awake()
    {
        if (buildPanel != null)
        {
            buildPanel.SetActive(false);
        }
    }


    private void OnEnable()
    {
        if (tileSelector == null)
        {
            return;
        }

        // 잔디 타일 선택 시 패널 열기
        tileSelector.OnCellSelected += OpenBuildPanel;

        // 타일 선택 취소 시 패널 닫기
        tileSelector.OnSelectionCanceled += CloseBuildPanel;
    }


    private void OnDisable()
    {
        if (tileSelector == null)
        {
            return;
        }

        tileSelector.OnCellSelected -= OpenBuildPanel;
        tileSelector.OnSelectionCanceled -= CloseBuildPanel;
    }


    private void OpenBuildPanel()
    {
        if (buildPanel != null)
        {
            buildPanel.SetActive(true);
        }
    }


    private void CloseBuildPanel()
    {
        if (buildPanel != null)
        {
            buildPanel.SetActive(false);
        }
    }

    public void BuildTower(TowerData towerData)
    {
        if (towerData == null)
        {
            Debug.LogWarning("TowerData가 전달되지 않았습니다.");
            return;
        }

        if (tileSelector == null || !tileSelector.CanBuildOnSelectedCell())
        {
            Debug.LogWarning("건설할 수 있는 타일이 선택되지 않았습니다.");
            return;
        }

        if (towerData.TowerPrefab == null)
        {
            Debug.LogError($"{towerData.TowerName}의 Tower Prefab이 없습니다."
            );

            return;
        }

        if (GoldManager.instance == null)
        {
            Debug.LogError("GoldManager가 존재하지 않습니다.");
            return;
        }


        Vector3Int buildCell = tileSelector.SelectedCell;
        Vector3 buildPosition = tileSelector.SelectedWorldPosition;

        // 골드가 부족시 건설X 패널만 유지
        if (!GoldManager.instance.SpendGold(towerData.BuildCost))
        {
            return;
        }

        GameObject tower = Instantiate(towerData.TowerPrefab,buildPosition,Quaternion.identity,towersParent);

        bool registered = tileSelector.RegisterTower(buildCell, tower);

        //타워 건설 실패시 골드도 반환
        if (!registered)
        {
            Destroy(tower);
            GoldManager.instance.AddGold(towerData.BuildCost);

            Debug.LogWarning("타워 위치 등록에 실패했습니다.");
            return;
        }

        Debug.Log($"{towerData.TowerName} 건설 완료 / 비용: {towerData.BuildCost}"
        );
    }


    //건선 취소
    public void CancelBuild()
    {
        if (tileSelector != null)
        {
            tileSelector.CancelSelection();
        }
    }
}