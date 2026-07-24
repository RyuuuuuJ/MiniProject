using UnityEngine;

[CreateAssetMenu( fileName = "NewTowerData",
    menuName = "Tower Defence / Tower Data" )]
public class TowerData : ScriptableObject
{
    [SerializeField] private string towerName;
    [SerializeField] private Sprite towerIcon;
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private int buildCost = 150;

    //읽기 전용
    public string TowerName => towerName;
    public int BuildCost => buildCost;
    public Sprite TowerIcon => towerIcon;
    public GameObject TowerPrefab => towerPrefab;
}
