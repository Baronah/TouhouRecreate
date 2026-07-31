using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairySpellcard_3 : SpellcardBase
{
    private static WaitForSeconds _waitForSeconds1 = new WaitForSeconds(1f);
    [SerializeField] float ShootInterval = 2f;
    private static WaitForSeconds _waitForSecondsInterval;

    float[] ScreenCorners;

    protected override void Start()
    {
        ScreenCorners = new float[]
        {
            GameManager._instance.cornerLeftDown.position.x,
            GameManager._instance.cornerLeftDown.position.y,
            GameManager._instance.cornerRightUp.position.x,
            GameManager._instance.cornerRightUp.position.y,
        };

        _waitForSecondsInterval = new WaitForSeconds(ShootInterval);

        base.Start();
    }

    protected override IEnumerator SpellcardShoot()
    {
        yield return StartCoroutine(InitializeSpellCardPreEffect());
        yield return new WaitForSeconds(0.25f);
        yield return StartCoroutine(MoveToCenter());

        float offset = 0;
        int count = 0;
        while (true)
        {
            yield return _waitForSecondsInterval;

            if (count >= 4)
            {
                yield return new WaitForSeconds(0.5f);
                for (int i = 0; i < 360; i += 45)
                {
                    CreateBigStars(bulletsUse[Random.Range(0, 4)], 45 * i);
                }

                count = 0;

                yield return _waitForSeconds1;
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    CreateBigStars(bulletsUse[i], 90 * i + offset);
                }

                count++;
            }

            offset = (offset + 90) % 360;
        }
    }

    IEnumerator MoveToCenter()
    {
        Vector3 center = GameManager._instance.CenterScreen;

        float c = 0, duration = 2f;
        spellOwner.MakeInvulnerable(duration);
        while (c < duration)
        {
            spellOwner.transform.position = Vector3.Lerp(spellOwner.transform.position, center, c * 1.0f / duration);
            c += Time.deltaTime;
            yield return null;
        }

        spellOwner.transform.position = center;
    }

    void CreateBigStars(BulletData.BulletType bulletType, float angle)
    {
        var shoot = CreateBigStarProjectile(bulletType, angle * Mathf.Deg2Rad);
        StartCoroutine(WaitUntilShootOverbound(shoot));
    }

    Vector3 starRotation = new(0, 0, 360);
    DefaultProjectile CreateBigStarProjectile(BulletData.BulletType bulletType, float angle)
    {
        DefaultProjectile projectileStar = CreateSimpleProjectile(
                bulletType,
                50f,
                transform.position,
                4f
            );

        projectileStar.SetRotation(starRotation);
        projectileStar.SetDirection(new(Mathf.Cos(angle), Mathf.Sin(angle)));
        projectileStar.SetAcceleration(30f);
        projectileStar.InitializeAndShoot();
        return projectileStar;
    }


    bool hasShotOverbound(Vector3 shootPosition)
    {
        return
            shootPosition.x < ScreenCorners[0]
            ||
            shootPosition.y < ScreenCorners[1]
            ||
            shootPosition.x > ScreenCorners[2]
            ||
            shootPosition.y > ScreenCorners[3];
    }

    IEnumerator WaitUntilShootOverbound(DefaultProjectile toTrack)
    {
        yield return new WaitUntil(() => 
            hasShotOverbound(toTrack.transform.position) || !toTrack.gameObject.activeSelf
        );
        TransformShoot(toTrack);
    }

    void TransformShoot(DefaultProjectile toShoot)
    {
        if (!toShoot.gameObject.activeSelf) return;

        Vector3 spawnPos = toShoot.transform.position;
        BulletData.BulletType type = toShoot.GetBulletType;

        toShoot.ReturnToPool();

        CreateSmallStars(spawnPos, type);
    }

    void CreateSmallStars(Vector3 spawnPos, BulletData.BulletType type)
    {
        float initAngle = Random.Range(0, 360);
        for (int i = 0; i < 360; i += 12)
        {
            CreateSmallStarProjectile(spawnPos, type, (i + initAngle) * Mathf.Deg2Rad);
        }
    }

    float[] GetSpeedAndAccelerationByType(BulletData.BulletType type)
    {
        float[] result = type switch
        {
            BulletData.BulletType.STAR_RED => new float[] { 120, 10 },
            BulletData.BulletType.STAR_CYAN => new float[] { 10, 50 },
            BulletData.BulletType.STAR_GREEN => new float[] { 50, 30 },
            _ => new float[] { 20, 70 },
        };

        return result;
    }

    DefaultProjectile CreateSmallStarProjectile(Vector3 spawnPos, BulletData.BulletType type, float angle)
    {
        float[] speedAndAcceleration = GetSpeedAndAccelerationByType(type);

        DefaultProjectile star = CreateSimpleProjectile(type, speedAndAcceleration[0], spawnPos);

        star.SetAcceleration(speedAndAcceleration[1]);
        star.SetDirection(new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)));
        star.SetRotation(starRotation);
        star.InitializeAndShoot();
        
        return star;
    }
}
