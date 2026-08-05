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
        GameManager._instance.TrackEnemy(this);
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
        ProjectileManager._instance.ClearShootsOfType(GetAllBulletTypes());
        BossHealthBar._instance.SetHUDEmpty();
        StartCoroutine(WaitForDestruction());
    }

    [SerializeField] GameObject ExplodeEffect;
    IEnumerator WaitForDestruction()
    {
        SoundManager._instance.PlaySound(SfxData.SFXType.BOSS_DEFEAT, SoundManager.SfxChannel.PLAYER_DEATH);
        GameObject o = Instantiate(ExplodeEffect, transform.position, Quaternion.identity);
        Destroy(o, 3);

        float c = 0, d = 1f;
        Color c1 = Color.white, c2 = Color.clear;   
        while (c < d)
        {
            spriteRenderer.color = Color.Lerp(c1, c2, c * 1.0f / d);
            c += Time.deltaTime;
            yield return null;
        }
        spriteRenderer.color = c2;

        yield return new WaitForSeconds(1f);
        GameManager._instance.OnGameOver();
        gameObject.SetActive(false);
    }
}
