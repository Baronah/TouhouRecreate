using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BulletData;

public class FairyNonspell_1 : SpellcardBase
{
    [SerializeField] [Range(0, 200)] int circularShotProjecitleCount = 50;
    [SerializeField] float StarShotVelocity = 480f, StarShotAcceleration = -400;
    [SerializeField] float RedShotVelocity = 20, RedShotAcceleration = 20f;

    [SerializeField] float shotInterval = 0.6f;
    [SerializeField] float waitUntilShotChangeColor = 1.8f;

    private static WaitForSeconds _waitForSecondsCircularHoming = new WaitForSeconds(0.6f);
    private static WaitForSeconds _waitForSecondsColorChange = new WaitForSeconds(1.8f);

    protected override void Start()
    {
        base.Start();
        _waitForSecondsCircularHoming = new WaitForSeconds(shotInterval);
        _waitForSecondsColorChange = new WaitForSeconds(waitUntilShotChangeColor);
    }

    protected override IEnumerator AttackShoot()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(HomingCircularShoot());
    }

    List<DefaultProjectile> homingShotCache = new();
    IEnumerator HomingCircularShoot()
    {
        while (true)
        {
            yield return _waitForSecondsCircularHoming;
            Vector3 targetDirection = (PlayerPosition - transform.position).normalized;
            float angleHoming = Mathf.Atan2(targetDirection.y, targetDirection.x);

            SoundManager._instance.PlaySound(SfxData.SFXType.SHOOT_1);
            homingShotCache.Clear();
            int jump = 180 / circularShotProjecitleCount;
            for (int i = -90; i < 90; i += jump)
            {
                float radianOffset = i * Mathf.Deg2Rad;
                homingShotCache.Add(CreateHomingProjectile(angleHoming + radianOffset));
            }

            StartCoroutine(WaitThenChangeShot(new(homingShotCache)));
        }
    }

    IEnumerator WaitThenChangeShot(List<DefaultProjectile> defaultProjectiles)
    {
        yield return _waitForSecondsColorChange;
        ChangeShotAndVelocity(defaultProjectiles);
    }

    DefaultProjectile CreateHomingProjectile(float angle)
    {
        DefaultProjectile projectile = CreateSimpleProjectile(
            bulletsUse[0],
            StarShotVelocity,
            transform.position
        );

        projectile.SetSpeedExhaustType(DefaultProjectile.SpeedExhaustType.STAY_STILL);
        projectile.SetDirection(new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)));
        projectile.SetAcceleration(StarShotAcceleration);
        projectile.InitializeAndShoot();

        return projectile;
    }

    void ChangeShotAndVelocity(List<DefaultProjectile> defaultProjectiles)
    {
        SoundManager._instance.PlaySound(SfxData.SFXType.REVERSE, SoundManager.SfxChannel.TRANSFORM);
        foreach (DefaultProjectile projectile in defaultProjectiles)
        {
            if (!projectile.gameObject.activeSelf) continue;

            DefaultProjectile transformProjectile = CreateSimpleProjectile(
                    bulletsUse[1],
                    0f,
                    projectile.transform.position
                );

            transformProjectile.SetRotation(Vector3.zero);
            transformProjectile.SetDirection(Vector3.down);
            transformProjectile.SetSpeed(RedShotVelocity);
            transformProjectile.SetAcceleration(RedShotAcceleration);
            transformProjectile.InitializeAndShoot();

            projectile.ReturnToPool();
        }
    }
}
