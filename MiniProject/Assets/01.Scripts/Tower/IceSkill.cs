using UnityEngine;

public class IceSkill : BulletHitEffect
{
    [SerializeField] [Range(0.1f, 1f)] private float slowMultiplier = 0.5f;

    [SerializeField] private float slowDuration = 2.5f;

    public override void Apply(MonsterHp target)
    {
        if (target == null ||target.isDead || !target.gameObject.activeInHierarchy)
        {
            return;
        }

        MonsterStatus statusEffects = target.statusEffect;

        if (statusEffects == null)
        {
            return;
        }

        statusEffects.ApplySlow(slowMultiplier,slowDuration);
    }
}
