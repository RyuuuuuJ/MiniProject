using UnityEngine;


//ObjectPool에서 총알 가져오기
//FirePoint에서 총알 생성
//총알에게 타깃, 피해량, 속도 전달
//공용 기본 공격
public class CreateBullet : TowerAttack
{
    [SerializeField]
    private string bulletKey;

    [SerializeField]
    private float bulletSpeed = 5f;

    [SerializeField]
    private Transform firePoint;

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
        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;

        //pool에서 bullet 가져오기
        GameObject bullet = ObjectPoolManager.instance.GetObject(bulletKey, spawnPosition, Quaternion.identity);


        if (!bullet.TryGetComponent(out TowerBullet towerBullet))
        {
            ObjectPoolManager.instance.ReturnObject(bullet);

            return;
        }

        towerBullet.Initialize(target, attackDamage, bulletSpeed);

        Debug.Log(
             $"{gameObject.name}이 {target.gameObject.name}을 공격했습니다. " + $"Damage: {attackDamage}"
         );
    }
}
