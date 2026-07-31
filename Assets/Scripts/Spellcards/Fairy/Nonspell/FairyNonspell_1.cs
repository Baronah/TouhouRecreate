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

    private void Start()
    {
        _waitForSecondsCircularHoming = new WaitForSeconds(shotInterval);
        _waitForSecondsColorChange = new WaitForSeconds(waitUntilShotChangeColor);
    }

    protected override IEnumerator SpellcardShoot()
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

            homingShotCache.Clear();
            int jump = 200 / circularShotProjecitleCount;
            for (int i = -100; i < 100; i += jump)
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

    Vector3 rotation = new(0, 0, 180);
    DefaultProjectile CreateHomingProjectile(float angle)
    {
        DefaultProjectile projectile = CreateSimpleProjectile(
            bulletsUse[0],
            StarShotVelocity,
            transform.position
        );

        projectile.SetRotation(rotation);
        projectile.SetSpeedExhaustType(DefaultProjectile.SpeedExhaustType.STAY_STILL);
        projectile.SetDirection(new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)));
        projectile.SetAcceleration(StarShotAcceleration);
        projectile.InitializeAndShoot();

        return projectile;
    }

    void ChangeShotAndVelocity(List<DefaultProjectile> defaultProjectiles)
    {
        foreach (DefaultProjectile projectile in defaultProjectiles)
        {
            if (!projectile.gameObject.activeSelf) continue;

            ProjectileManager._instance.ChangeBulletTypeOfCurrentProjectile(
                projectile,
                bulletsUse[1]
                );

            projectile.SetRotation(Vector3.zero);
            projectile.SetDirection(Vector3.down);
            projectile.SetSpeed(RedShotVelocity);
            projectile.SetAcceleration(RedShotAcceleration);
            projectile.InitializeAndShoot();
        }
    }
}
