using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BulletData;
using static DefaultProjectile;
using Random = UnityEngine.Random;

// Star sign "The Stars Descend" (星降る夜の星座)
public class FairySpellcard_1 : SpellcardBase
{
    protected override IEnumerator SpellcardShoot()
    {
        yield return StartCoroutine(InitializeSpellCardPreEffect());

        yield return new WaitForSeconds(1f);
        StartCoroutine(BoWaP(1.25f, bulletsUse[0], 250f, -90f, SpeedExhaustType.STAY_STILL));
        StartCoroutine(BoWaP(1.25f, bulletsUse[0], 250f, -115f, SpeedExhaustType.STAY_STILL));
        StartCoroutine(BoWaP(1.25f, bulletsUse[0], 250f, -160f, SpeedExhaustType.STAY_STILL));
        StartCoroutine(BoWaP(1.25f, bulletsUse[0], 250f, -240f, SpeedExhaustType.STAY_STILL));

        while (true)
        {
            yield return new WaitForSeconds(3.5f);

            yield return StartCoroutine(BoWaP(3f, bulletsUse[1], 150f, 25f, SpeedExhaustType.CONTINUE));

            StopAllBullets();

            yield return new WaitForSeconds(1.5f);
            ChangeShots(bulletsUse[1]);

            yield return new WaitForSeconds(10f);
        }
    }

    private static WaitForSeconds _waitForSeconds0_05 = new WaitForSeconds(0.05f);
    IEnumerator BoWaP(float duration, BulletType bulletType, float speed, float acceleration, SpeedExhaustType exhaustType)
    {
        float countUp = 0;
        float initialAngle = Random.Range(0f, 360f);
        int loopcount = 0;
        while (countUp < duration)
        {
            for (int i = 0; i < 360; i += 6)
            {
                CreateWaveShots(i + initialAngle + loopcount * 2, bulletType, speed, acceleration, exhaustType);
                countUp += 0.05f;
                yield return _waitForSeconds0_05;

                if (countUp >= duration) break;
            }

            loopcount++;
        }
    }

    void CreateWaveShots(float degree, BulletData.BulletType bulletType, float speed, float acceleration, SpeedExhaustType exhaustType)
    {
        for (int i = 0; i < 360; i += 40)
        {
            float angle = (degree + i) * Mathf.Deg2Rad;
            CreateWaveShotsAtAngle(angle, bulletType, speed, acceleration, exhaustType);
        }
    }

    Vector3 waveRotation = new(0, 0, -180f);
    void CreateWaveShotsAtAngle(float angle, BulletData.BulletType bulletType, float speed, float acceleration, SpeedExhaustType speedExhaustType)
    {
        Vector3 direction = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0f);
        DefaultProjectile projectile = CreateSimpleProjectile(
            bulletType,
            speed,
            transform.position);

        projectile.SetDirection(direction);
        projectile.SetRotation(waveRotation);
        projectile.SetSpeedExhaustType(speedExhaustType);
        projectile.SetAcceleration(acceleration);
        projectile.InitializeAndShoot();
    }

    List<DefaultProjectile> projectilesCache;
    void StopAllBullets()
    {
        projectilesCache =
            ProjectileManager._instance.GetProjectilesOfType(bulletsUse).Where(b => b.gameObject.activeSelf).ToList();

        foreach (DefaultProjectile shoot in projectilesCache) shoot.Stop();
    }

    void ChangeShots(BulletType type)
    {
        projectilesCache = ProjectileManager._instance.GetProjectilesOfType(type).Where(b => b.gameObject.activeSelf).ToList();
        float downAngle = Mathf.Atan2(Vector3.down.y, Vector3.down.x); 
        
        foreach (DefaultProjectile shoot in projectilesCache)
        {
            ProjectileManager._instance.ChangeBulletTypeOfCurrentProjectile(shoot, BulletType.STAR_RED);

            shoot.SetRotation(waveRotation * -1.5f);
            shoot.SetDirection(new Vector3(Mathf.Cos(downAngle), Mathf.Sin(downAngle)));
            shoot.SetSpeed(5);
            shoot.SetAcceleration(Random.Range(10, 30));
            shoot.InitializeAndShoot();
        }
    }
}
