using UnityEngine;

public class FireTower : TowerAttack
{

    [SerializeField] string bulletkey = "FireBullet";
    [SerializeField] float bulletspeed = 5f;

    [SerializeField] Transform firePoint;

    protected override void Attack(MonsterHp target)
    {
        // 타깃이 사라졌거나 이미 죽었다면 공격 X
        if (target == null || target.isDead)
        {
            return;
        }

        if (ObjectPoolManager.instance == null)
        {           
            return;
        }

        //발사 위치가 지정 되어있지 않으면 현재 위치에서 발사, 지정시 지정된 곳에서 발사
        Vector3 spawnPosition = firePoint != null? firePoint.position: transform.position;

        //pool에서 bullet 가져오기
        GameObject bullet = ObjectPoolManager.instance.GetObject(bulletkey,spawnPosition,Quaternion.identity);

        TowerBullet towerBullet = bullet.GetComponent<TowerBullet>();

        if (towerBullet == null)
        {
            ObjectPoolManager.instance.ReturnObject(bullet);
            return;
        }

        towerBullet.Initialize(target,attackDamage,bulletspeed);

        Debug.Log(
            $"{gameObject.name}이 {target.gameObject.name}을 공격했습니다. " +$"Damage: {attackDamage}"
        );
    }
}
