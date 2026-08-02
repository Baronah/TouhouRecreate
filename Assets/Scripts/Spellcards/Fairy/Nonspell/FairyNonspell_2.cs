using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BulletData;

public class FairyNonspell_2 : SpellcardBase
{
    private static WaitForSeconds _waitForSeconds0_25 = new WaitForSeconds(0.2f);
    private static WaitForSeconds _waitForSecondsForStillProjecileTurnDisplace = new WaitForSeconds(1.9f);
    private static WaitForSeconds _waitForSecondsDisplacement = new WaitForSeconds(2.5f);

    protected override IEnumerator SpellcardShoot()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(CircularDisplacementShoot());
    }

    IEnumerator CircularDisplacementShoot()
    {
        yield return new WaitForSeconds(0.5f);
        while (true)
        {
            float offset = Random.Range(0, 360);
            float addedSpeed = 0;
            for (int i = 0; i < 6; ++i)
            {
                float displacement = 60 * Mathf.Pow(-1, i);
                List<DefaultProjectile> initialProjectiles 
                    = CreateInitialDisplacementProjectiles(
                        40,
                        offset,
                        addedSpeed);

                StartCoroutine(
                    WaitSecondsThenMakeProjecileDisplace(
                        initialProjectiles,
                        displacement
                    )
                );

                offset += 15;
                addedSpeed -= 10;
                yield return _waitForSeconds0_25;
            }

            yield return _waitForSecondsDisplacement;
        }
    }

    IEnumerator WaitSecondsThenMakeProjecileDisplace(List<DefaultProjectile> projectiles, float displacement)
    {
        yield return _waitForSecondsForStillProjecileTurnDisplace;
        MakeDisplacementProjectile(projectiles, displacement);
    }

    List<DefaultProjectile> defaultProjectiles;
    List<DefaultProjectile> CreateInitialDisplacementProjectiles(int count, float offset, float addedSpeed)
    {
        defaultProjectiles = new();
        int jump = 360 / count;

        for (int i = 0; i < 360; i += jump)
        {
            float radianOffset = (offset + i) * Mathf.Deg2Rad;
            DefaultProjectile projectileOut = CreateInitialDisplacementProjectile(radianOffset, addedSpeed);
            defaultProjectiles.Add(projectileOut);
        }

        return defaultProjectiles;
    }

    DefaultProjectile CreateInitialDisplacementProjectile(float angle, float addedSpeed)
    {
        DefaultProjectile projectile = CreateSimpleProjectile(
            BulletType.RICE_CYAN,
            200 + addedSpeed,
            transform.position
        );

        projectile.SetSpeedExhaustType(DefaultProjectile.SpeedExhaustType.STAY_STILL);
        projectile.SetDirection(new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)));
        projectile.SetAcceleration(-250f);
        projectile.InitializeAndShoot();

        return projectile;
    }

    void MakeDisplacementProjectile(List<DefaultProjectile> projectiles, float displacement)
    {
        foreach (var projectile in projectiles)
        {
            ProjectileManager._instance.ChangeBulletTypeOfCurrentProjectile(
                projectile,
                BulletType.RICE_RED
            );

            projectile.SetSpeed(120f);
            projectile.SetAcceleration(50f);
            projectile.SetDisplacement(displacement);
            projectile.InitializeAndShoot();
        }
    }
}
