using UnityEngine;

//총알이 몬스터에게 명중했을 때 실행되는 추가 기능의 공통 부모 클래스
public abstract class BulletHitEffect : MonoBehaviour
{
    public abstract void Apply(MonsterHp target);
}
