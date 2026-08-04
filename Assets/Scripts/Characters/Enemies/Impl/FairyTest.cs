using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BulletData;
using static DefaultProjectile;
using static SpellcardBase;

public class FairyTest : EnemyBase
{
    [SerializeField] GameObject MagicCircle;
    [SerializeField] GameObject SpellcardEffect;

    protected override void Start()
    {
        base.Start();
        StartShooting();
    }

    public override void Update()
    {
        base.Update();
        SpellcardEffect.SetActive(IsUsingSpellcard);
    }

    public override void DisableHitbox()
    {
        base.DisableHitbox();
        MagicCircle.SetActive(false);
    }

    public override void EnableHitbox()
    {
        base.EnableHitbox();
        MagicCircle.SetActive(true);
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
                currentActiveSpellcard.Shoot();
                yield return new WaitUntil(() => !currentActiveSpellcard.IsShooting);
            }
        }

        OnDeath();
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        BossHealthBar._instance.SetHUDEmpty();
    }
}
