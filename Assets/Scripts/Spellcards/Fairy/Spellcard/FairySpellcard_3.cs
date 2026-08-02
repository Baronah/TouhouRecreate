using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// "A Starry Night, the City of Light" (星の夜、ひかりの街)
public class FairySpellcard_3 : SpellcardBase
{
    private static WaitForSeconds _waitForSeconds1 = new WaitForSeconds(1f);
    [SerializeField] float ShootInterval = 2f;
    private static WaitForSeconds _waitForSecondsInterval;


    protected override void Start()
    {
        _waitForSecondsInterval = new WaitForSeconds(ShootInterval);

        base.Start();
    }

    protected override IEnumerator SpellcardShoot()
    {
        yield return StartCoroutine(InitializeSpellCardPreEffect());
        yield return new WaitForSeconds(0.25f);
        yield return StartCoroutine(MoveToCenter());
        yield return new WaitForSeconds(1f);

        float offset = 0;
        int count = 0;
        while (true)
        {
            if (count >= 4)
            {
                yield return new WaitForSeconds(0.5f);
                for (int i = 0; i < 360; i += 45)
                {
                    CreateBigStars(bulletsUse[Random.Range(0, 4)], i);
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
            yield return _waitForSecondsInterval;
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
        float[] shootDatas = GetSpeedAccelerationAndStarCountByType(type);
        float speed = shootDatas[0],
              acceleration = shootDatas[1],
              count = shootDatas[2];

        float initAngle = Random.Range(0, 360);
        int jump = Mathf.RoundToInt(360 / count);
        for (int i = 0; i < 360; i += jump)
        {
            CreateSmallStarProjectile(spawnPos, type, (i + initAngle) * Mathf.Deg2Rad, speed, acceleration);
        }
    }

    float[] GetSpeedAccelerationAndStarCountByType(BulletData.BulletType type)
    {
        float[] result = type switch
        {
            BulletData.BulletType.STAR_RED => new float[] { 150, 20, 24 },
            BulletData.BulletType.STAR_CYAN => new float[] { 10, 50, 40 },
            BulletData.BulletType.STAR_GREEN => new float[] { 50, 30, 30 },
            _ => new float[] { 10, 7, 50 },
        };

        return result;
    }

    DefaultProjectile CreateSmallStarProjectile(Vector3 spawnPos, BulletData.BulletType type, float angle, float speed, float acceleration)
    {
        DefaultProjectile star = CreateSimpleProjectile(type, speed, spawnPos);

        star.SetAcceleration(acceleration);
        star.SetDirection(new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)));
        star.SetRotation(starRotation);
        star.InitializeAndShoot();
        
        return star;
    }
}
