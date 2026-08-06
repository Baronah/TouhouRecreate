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
    protected override IEnumerator AttackShoot()
    {
        SoundManager._instance.PlaySound(SfxData.SFXType.CHARGE_2, SoundManager.SfxChannel.EFFECT);
        ChargeEffect._instance.DoChargeEffect(spellOwner.transform.position, Color.white, 1.25f);
        yield return new WaitForSeconds(1.4f);
        StartCoroutine(BoWaP(2f, bulletsUse[0], 250f, -90f, SpeedExhaustType.STAY_STILL, false));
        StartCoroutine(BoWaP(2f, bulletsUse[0], 250f, -115f, SpeedExhaustType.STAY_STILL, false));
        StartCoroutine(BoWaP(2f, bulletsUse[0], 250f, -160f, SpeedExhaustType.STAY_STILL, false));
        StartCoroutine(BoWaP(2f, bulletsUse[0], 250f, -240f, SpeedExhaustType.STAY_STILL, false));
        yield return new WaitForSeconds(3.5f);

        bool reversed = false;
        while (true)
        {
            yield return StartCoroutine(BoWaP(3.6f, bulletsUse[1], 150f, 25f, SpeedExhaustType.STAY_STILL, reversed));
            SoundManager._instance.PlaySound(SfxData.SFXType.SHATTER_1, SoundManager.SfxChannel.EFFECT);
            yield return new WaitForSeconds(0.07f);

            ChargeEffect._instance.DoCircleEffect(spellOwner.transform.position, Color.blue, 0.6f);
            StopAllBullets();
            yield return new WaitForSeconds(1.4f);

            SoundManager._instance.PlaySound(SfxData.SFXType.CHARGE_2, SoundManager.SfxChannel.EFFECT);
            ChargeEffect._instance.DoChargeEffect(spellOwner.transform.position, new Color(1, 0.62f, 0.62f), 1.25f);
            yield return new WaitForSeconds(1.5f);
            ChargeEffect._instance.DoCircleEffect(spellOwner.transform.position, Color.red, 0.5f);
            yield return new WaitForSeconds(0.1f);
            ChangeShotsVelocityOfCachedProjectile();

            reversed = !reversed;
            yield return new WaitForSeconds(8f);
        }
    }

    private static WaitForSeconds _waitForSeconds0_05 = new WaitForSeconds(0.05f);
    IEnumerator BoWaP(float duration, BulletType bulletType, float speed, float acceleration, SpeedExhaustType exhaustType, bool reversed)
    {
        float countUp = 0;
        float initialAngle = Random.Range(0f, 360f);
        int loopcount = 0;
        while (countUp < duration)
        {
            for (int i = 0; i < 360; i += 6)
            {
                SoundManager._instance.PlaySound(SfxData.SFXType.SHOOT_1);
                CreateWaveShots(i + initialAngle + loopcount * 2, bulletType, speed, acceleration, exhaustType, reversed);
                countUp += 0.05f;
                yield return _waitForSeconds0_05;

                if (countUp >= duration) yield break;
            }

            loopcount++;
        }
    }

    void CreateWaveShots(float degree, BulletData.BulletType bulletType, float speed, float acceleration, SpeedExhaustType exhaustType, bool reversed)
    {
        for (int i = 0; i < 360; i += 40)
        {
            float angle = (degree + i) * Mathf.Deg2Rad;
            CreateWaveShotsAtAngle(angle, bulletType, speed, acceleration, exhaustType, reversed);
        }
    }

    Vector3 waveRotation = new(0, 0, -180f);
    void CreateWaveShotsAtAngle(float angle, BulletData.BulletType bulletType, float speed, float acceleration, SpeedExhaustType speedExhaustType, bool reversed)
    {
        Vector3 direction = reversed ? new Vector3(Mathf.Sin(angle), Mathf.Cos(angle))  : new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
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
                ProjectileManager._instance.GetProjectilesOfType(bulletsUse[1]).ToList();

        foreach (DefaultProjectile shoot in projectilesCache)
        {
            shoot.SetAcceleration(-350f);
            shoot.InitializeAndShoot();
        }
    }

    void ChangeShotsVelocityOfCachedProjectile()
    {
        projectilesCache =
                ProjectileManager._instance.GetProjectilesOfType(bulletsUse[1]).ToList();
        SoundManager._instance.PlaySound(SfxData.SFXType.REVERSE, SoundManager.SfxChannel.EFFECT);
        float downAngle = Mathf.Atan2(Vector3.down.y, Vector3.down.x); 
        
        foreach (DefaultProjectile shoot in projectilesCache)
        {
            ProjectileManager._instance.ChangeBulletTypeOfCurrentProjectile(shoot, bulletsUse[2]);

            shoot.SetRotation(waveRotation * -1.5f);
            shoot.SetDirection(new Vector3(Mathf.Cos(downAngle), Mathf.Sin(downAngle)));
            shoot.SetSpeed(5);
            shoot.SetAcceleration(Random.Range(10, 30));
            shoot.InitializeAndShoot();
        }
    }
}
