using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BulletData;
using static DefaultProjectile;
using static SpellcardBase;

public class FairyTest : EnemyBase
{
    [SerializeField] GameObject SpellcardEffect;

    protected override void Start()
    {
        base.Start();
        StartShooting();
    }

    BulletType[] GetBulletTypesOfSpell(int index)
    {
        return getSpellDatas[index].BulletTypes.Distinct().ToArray();
    }

    BulletType[] GetAllBulletTypes()
    {
        return spellDatas[currentLifeIndex].SelectMany(s => s.BulletTypes).Distinct().ToArray();
    }

    private void StartShooting()
    {
        StartCoroutine(ShootCoroutine());
    }

    IEnumerator ShootCoroutine()
    {
        for (int i = 0; i < spellcards.Length; ++i)
        {
            currentLifeIndex = i;
            BossHealthBar._instance.AttachHUDToBoss(this);

            for (int j = 0; j < spellcards[i].Length; ++j)
            {
                currentSpellcardIndex = j;
                SpellcardEffect.SetActive(currentActiveSpellcard.spellType == SpellType.SPELLCARD);
                currentActiveSpellcard.Shoot();
                yield return new WaitUntil(() => !currentActiveSpellcard.IsShooting);
            }
        }

        OnDeath();
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        ProjectileManager._instance.ClearShootsOfType(GetAllBulletTypes());
        BossHealthBar._instance.SetHUDEmpty();
    }
}
