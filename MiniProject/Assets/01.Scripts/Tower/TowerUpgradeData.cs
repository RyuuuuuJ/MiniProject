using System;
using UnityEngine;

//타워 업그레이드 데이터
[Serializable] public class TowerUpgradeData 
{
    [SerializeField, Min(0)] private int upgradeCost;

    [SerializeField, Min(0)]private int attackDamage;

    [SerializeField, Min(0.05f)] private float attackInterval = 1f;

    [SerializeField, Min(0.1f)]private float attackRange = 3f;

    [SerializeField] private Sprite towerSprite;

    public int UpgradeCost => upgradeCost;
    public int AttackDamage => attackDamage;
    public float AttackInterval => attackInterval;
    public float AttackRange => attackRange;
    public Sprite TowerSprite => towerSprite;
}
