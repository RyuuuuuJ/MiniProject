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
    [SerializeField] private Tilemap buildableTilemap;
    // 두 타워 중심 사이에 확보해야 하는 최소 거리
    [SerializeField] private float minimumTowerDistance = 2.6f;

    //선택 표시
    [SerializeField] private GameObject selectionMarker;

    [SerializeField] private LayerMask buildBlockerLayer;

    [SerializeField]
    private Vector2 blockerCheckSize = new Vector2(1.8f, 1.8f);

    [SerializeField] private float horizontalBuildSpacing = 2.6f;

    [SerializeField]private LayerMask towerLayer;

    private readonly Dictionary<Vector3Int, GameObject> placedTowers = new Dictionary<Vector3Int, GameObject>();

    public bool HasSelectedCell { get; private set; }

    // 현재 선택된 셀 좌표
    public Vector3Int SelectedCell { get; private set; }

    public event Action OnCellSelected;
    public event Action OnSelectionCanceled;

    public Vector3 SelectedWorldPosition { get; private set; }

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

        if (mainCamera != null && towerLayer.value != 0)
        {
            Vector2 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);

            Collider2D clickedTower = Physics2D.OverlapPoint(worldPosition, towerLayer);

            if (clickedTower != null)
            {

                CancelSelection();
                return;
            }
        }

        SelectCell(screenPosition);
    }

    /// 마우스 화면 좌표를 타일 셀 좌표로 변환
    private void SelectCell(Vector2 screenPosition)
    {
        if (mainCamera == null || buildableTilemap == null)
        {
            Debug.LogError( "BuildTileSelector의 Camera 또는 BuildableTilemap이 연결되지 않았습니다." );

            return;
        }

        // 카메라와 Tilemap 사이의 Z 거리 구하기
        float cameraDistance = Mathf.Abs(mainCamera.transform.position.z -buildableTilemap.transform.position.z);

        Vector3 screenPoint = new Vector3(screenPosition.x,screenPosition.y,cameraDistance);

        Vector3 worldPosition =mainCamera.ScreenToWorldPoint(screenPoint);

        worldPosition.z =buildableTilemap.transform.position.z;

        //클릭한곳에 건설 가능한 셀 찾기
        Vector3Int clickedCell = buildableTilemap.WorldToCell(worldPosition);

        if (!buildableTilemap.HasTile(clickedCell))
        {
            CancelSelection();
            return;
        }

        float snappedY = buildableTilemap.GetCellCenterWorld(clickedCell).y;

        float snappedX = Mathf.Round(worldPosition.x / horizontalBuildSpacing) * horizontalBuildSpacing;

        Vector3 buildPosition = new Vector3(snappedX,snappedY,buildableTilemap.transform.position.z);

        Vector3Int buildCell = buildableTilemap.WorldToCell(buildPosition);

        if (!CanBuildAtPosition(buildCell, buildPosition))
        {
            CancelSelection();
            return;
        }

        SelectedCell = buildCell;
        SelectedWorldPosition = buildPosition;
        HasSelectedCell = true;

        if (selectionMarker != null)
        {
            selectionMarker.transform.position =SelectedWorldPosition;

            selectionMarker.SetActive(true);
        }

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
        if (buildableTilemap == null)
        {
            return false;
        }

        // 건설 가능 셀이 아니면 건설 불가능
        if (!buildableTilemap.HasTile(cellPosition))
        {
            return false;
        }

        // 같은 셀에 타워가 있다면 건설 불가능
        if (IsOccupied(cellPosition))
        {
            return false;
        }

        if (buildBlockerLayer.value != 0)
        {
            Collider2D blocker = Physics2D.OverlapBox(buildPosition,blockerCheckSize,0f,buildBlockerLayer);

            if (blocker != null)
            {
                return false;
            }
        }

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
}
