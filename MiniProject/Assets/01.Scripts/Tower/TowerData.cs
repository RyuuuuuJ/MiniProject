using UnityEngine;

[CreateAssetMenu( fileName = "NewTowerData", menuName = "Tower Defence / Tower Data" )]
public class TowerData : ScriptableObject
{
    [SerializeField] private string towerName;
    [SerializeField] private Sprite towerIcon;
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private int buildCost = 150;

    [SerializeField] private TowerUpgradeData[] levelData;

    //읽기 전용
    public string TowerName => towerName;
    public int BuildCost => buildCost;
    public Sprite TowerIcon => towerIcon;
    public GameObject TowerPrefab => towerPrefab;

    public int MaxLevel => levelData == null ? 0 : levelData.Length;

    public TowerUpgradeData GetLevelData(int level)
    {
        int index = level - 1;

        if (levelData == null || index < 0 || index >= levelData.Length)
        {
            return null;
        }

        return levelData[index];
    }
}
