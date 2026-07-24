using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class BuildTileSelector : MonoBehaviour
{
    //카메라 , 타일맵
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap roadTilemap;
    // 두 타워 중심 사이에 확보해야 하는 최소 거리
    [SerializeField, Min(0.1f)] private float minimumTowerDistance = 2.6f;

    // 기존 타워와 높이가 비슷하면 같은 줄로 정렬하는 범위
    [SerializeField, Min(0f)] private float rowSnapTolerance = 0.35f;
    //선택 표시
    [SerializeField] private GameObject selectionMarker;

    [SerializeField] private LayerMask buildBlockerLayer;

    [SerializeField]
    private Vector2 blockerCheckSize = new Vector2(1.8f, 1.8f);

    private readonly Dictionary<Vector3Int, GameObject> placedTowers = new Dictionary<Vector3Int, GameObject>();

    public bool HasSelectedCell { get; private set; }

    // 현재 선택된 셀 좌표
    public Vector3Int SelectedCell { get; private set; }

    public event Action OnCellSelected;
    public event Action OnSelectionCanceled;

    public Vector3 SelectedWorldPosition { get; private set; }

    [SerializeField] private float buildSnapSize = 0.25f;

    private void Awake()
    {        
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (selectionMarker != null)
        {
            selectionMarker.SetActive(false);
        }
    }

    private void Update()
    {
        // 게임오버 또는 일시정지 중에는 타일을 선택하지 않습니다.
        if (GameManager.instance != null && !GameManager.instance.IsPlaying)
        {
            return;
        }

        if (Mouse.current == null)
        {
            return;
        }

        // 우클릭으로 선택 취소
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            CancelSelection();
            return;
        }

        // ESC 키로도 선택 취소
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CancelSelection();
            return;
        }

        if (!Mouse.current.leftButton.wasPressedThisFrame)
        {
            return;
        }


         // UI 버튼을 눌렀을 때 UI 뒤에 있는 타일까지 함께 선택되는 것을 방지
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        Vector2 screenPosition = Mouse.current.position.ReadValue();

        SelectCell(screenPosition);
    }

    /// 마우스 화면 좌표를 타일 셀 좌표로 변환
    private void SelectCell(Vector2 screenPosition)
    {
        if (mainCamera == null ||groundTilemap == null || roadTilemap == null)
        {
            Debug.LogError(
                "BuildTileSelector의 Camera 또는 Tilemap이 연결되지 않았습니다."
            );

            return;
        }

        // 카메라부터 Tilemap까지의 Z 거리
        float cameraDistance = Mathf.Abs(mainCamera.transform.position.z -groundTilemap.transform.position.z);

        Vector3 screenPoint = new Vector3(screenPosition.x,screenPosition.y,cameraDistance);

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPoint);

        worldPosition.z = groundTilemap.transform.position.z;

        Vector3 buildPosition = SnapBuildPosition(worldPosition);

        // 옆에 있는 타워와 Y 위치가 비슷하면 같은 줄로 정렬합니다.
        buildPosition = SnapToNearbyTowerRow(buildPosition);

        // 월드 좌표를 Grid 셀 좌표로 변환
        Vector3Int cellPosition = groundTilemap.WorldToCell(buildPosition);

        // 건설할 수 없는 위치를 클릭하면 기존 선택 취소
        if (!CanBuildAtPosition(cellPosition,buildPosition))
        {
            CancelSelection();
            return;
        }

        SelectedCell = cellPosition;
        SelectedWorldPosition = buildPosition;
        HasSelectedCell = true;

        if (selectionMarker != null)
        {
            // 초록색 표시와 실제 타워 위치를 동일하게 맞춥니다.
            selectionMarker.transform.position = SelectedWorldPosition;
            selectionMarker.SetActive(true);
        }

        Debug.Log($"건설 타일 선택: {SelectedCell}");

        OnCellSelected?.Invoke();
    }

    //셀에 건설이 가능한지 확인
    public bool CanBuildOnCell(Vector3Int cellPosition)
    {
        if (groundTilemap == null)
        {
            return false;
        }

        Vector3 cellCenter = groundTilemap.GetCellCenterWorld(cellPosition);

        return CanBuildAtPosition(cellPosition, cellCenter);
    }

    private bool CanBuildAtPosition(Vector3Int cellPosition,Vector3 buildPosition)
    {
        if (groundTilemap == null || roadTilemap == null)
        {
            return false;
        }

        // 맵 바깥에는 건설할 수 없습니다.
        if (!groundTilemap.HasTile(cellPosition))
        {
            return false;
        }

        // 도로 타일에는 건설할 수 없습니다.
        if (roadTilemap.HasTile(cellPosition))
        {
            return false;
        }

        // 같은 타일에 이미 타워가 있다면 건설할 수 없습니다.
        if (IsOccupied(cellPosition))
        {
            return false;
        }

        // Base, EnemySpawn 등의 건설 방해 오브젝트를 검사합니다.
        if (buildBlockerLayer.value != 0)
        {
            Collider2D blocker = Physics2D.OverlapBox( buildPosition,blockerCheckSize, 0f,buildBlockerLayer);

            if (blocker != null)
            {
                return false;
            }
        }

        //기존 타워와 너무 가까우면 건설안댐
        if (IsTooCloseToPlacedTower(buildPosition))
        {
            return false;
        }

        return true;
    }
    //선택된 셀이 건설 가능한지 확인
    public bool CanBuildOnSelectedCell()
    {
        return HasSelectedCell && CanBuildAtPosition(SelectedCell, SelectedWorldPosition);
    }

    //셀에 타워 건설 등록
    public bool RegisterTower(Vector3Int cellPosition,GameObject tower)
    {
        if (tower == null || IsOccupied(cellPosition))
        {
            return false;
        }

        placedTowers.Add(cellPosition, tower);

        if (HasSelectedCell && SelectedCell == cellPosition)
        {
            CancelSelection();
        }

        return true;
    }

    //판매한 타워 셀의 위치를 다시 건설 가능 상태로 전환
    public void UnregisterTower(Vector3Int cellPosition)
    {
        placedTowers.Remove(cellPosition);
    }

    private bool IsOccupied(Vector3Int cellPosition)
    {
        if (!placedTowers.TryGetValue(cellPosition,out GameObject tower))
        {
            return false;
        }
      
        if (tower == null)
        {
            placedTowers.Remove(cellPosition);
            return false;
        }

        return true;
    }

    //현재 선택을 해지/숨김
    public void CancelSelection()
    {
        bool hadSelection = HasSelectedCell;

        HasSelectedCell = false;

        if (selectionMarker != null)
        {
            selectionMarker.SetActive(false);
        }

        if (hadSelection)
        {
            OnSelectionCanceled?.Invoke();
        }
    }

    private Vector3 SnapBuildPosition(Vector3 worldPosition)
    {
        worldPosition.x = Mathf.Round(worldPosition.x / buildSnapSize) * buildSnapSize;

        worldPosition.y = Mathf.Round(worldPosition.y / buildSnapSize) * buildSnapSize;

        worldPosition.z = groundTilemap.transform.position.z;

        return worldPosition;
    }

    // 기존 타워와 너무 가까운 위치인지 검사합니다.
    private bool IsTooCloseToPlacedTower(Vector3 buildPosition)
    {
        float minimumDistanceSqr = minimumTowerDistance * minimumTowerDistance;

        foreach (KeyValuePair<Vector3Int, GameObject> pair in placedTowers)
        {
            GameObject tower = pair.Value;

            if (tower == null)
            {
                continue;
            }

            Vector2 difference = tower.transform.position - buildPosition;

            if (difference.sqrMagnitude < minimumDistanceSqr)
            {
                return true;
            }
        }

        return false;
    }

    // 가까운 기존 타워의 Y 위치에 맞춰 일렬로 정렬합니다.
    private Vector3 SnapToNearbyTowerRow(Vector3 buildPosition)
    {
        float closestDifference = rowSnapTolerance;

        foreach (KeyValuePair<Vector3Int, GameObject> pair in placedTowers)
        {
            GameObject tower = pair.Value;

            if (tower == null)
            {
                continue;
            }

            float yDifference = Mathf.Abs(tower.transform.position.y - buildPosition.y);

            if (yDifference <= closestDifference)
            {
                buildPosition.y = tower.transform.position.y;
                closestDifference = yDifference;
            }
        }

        return buildPosition;
    }
}
