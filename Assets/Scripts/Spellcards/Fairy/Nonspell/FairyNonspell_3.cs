using System.Collections;
using UnityEngine;
using static BulletData;

public class FairyNonspell_3 : SpellcardBase
{
    [SerializeField] GameObject indicator;
    [SerializeField] private float intervalRed = 1f, intervalBlue = 0.2f;
    [SerializeField] private float Speed = 400f, Acceleration = 0f;

    GameObject ind1, ind2;
    protected override IEnumerator AttackShoot()
    {
        spellOwner.MakeInvulnerable(0.75f);
        SoundManager._instance.PlaySound(SfxData.SFXType.CHARGE_4, SoundManager.SfxChannel.EFFECT);
        ind1 = CreateIndicator(GameManager._instance.CenterLeft + new Vector3(100, 50), Color.cyan, 60);
        ind2 = CreateIndicator(GameManager._instance.CenterRight - new Vector3(100, -50), Color.cyan, -60);
        yield return new WaitForSeconds(1.25f);

        StartCoroutine(CreateConsecutiveStarShootSpreadInCircle(bulletsUse[0]));
        while (true)
        {
            StartCoroutine(Create3ConsecutiveStarShootTowardPlayer(bulletsUse[1]));
            yield return new WaitForSeconds(intervalRed);
        }
    }

    GameObject CreateIndicator(Vector3 pos, Color c, float rotation)
    {
        GameObject ind = Instantiate(indicator, pos, Quaternion.identity);
        StartCoroutine(ShrinkUp(ind, c, rotation));
        return ind;
    }

    IEnumerator ShrinkUp(GameObject o, Color color, float rotation)
    {
        o.GetComponent<SpriteRenderer>().color = color;
        o.GetComponent<ContinuousRotate>().degreePerSecond = new(0, 0, rotation);
        o.transform.localScale = Vector3.zero;

        Vector3 finalScale = new(25, 25, 25);
        float c = 0, d = 0.5f;
        while (c < d)
        {
            o.transform.localScale = Vector3.Lerp(o.transform.localScale, finalScale, c * 1.0f / d);
            c += Time.deltaTime;
            yield return null;
        }

        o.transform.localScale = finalScale;
    }

    IEnumerator Create3ConsecutiveStarShootTowardPlayer(BulletType bulletType)
    {
        float angle = GetAngleToPlayer(spellOwner.transform.position, AngleType.DEGREE);
        int projectilePerWave = 3;
        int projectileCount = 5;
        int jump = 360 / projectileCount;
        for (int i = 0; i < projectilePerWave; ++i)
        {
            SoundManager._instance.PlaySound(SfxData.SFXType.SHOOT_1);
            for (int j = 0; j < 360; j += jump)
            {
                CreateStarProjectile(bulletType, (angle + j) * Mathf.Deg2Rad, spellOwner.transform.position);
            }

            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator CreateConsecutiveStarShootSpreadInCircle(BulletType bulletType)
    {
        int waveCount = 36;
        int projectilePerCircle = 5;
        int projectilePerWave = 3;
        int jumpWave = 360 / waveCount,
            jumpProjectile = 360 / projectilePerCircle;

        Vector3 shootTo = spellOwner.transform.position - new Vector3(0, 320);

        float initAngle_1 = GetAngleToPoint(ind1.transform.position, shootTo, AngleType.DEGREE),
              initAngle_2 = GetAngleToPoint(ind2.transform.position, shootTo, AngleType.DEGREE);
        while (true)
        {
            for (int i = 0; i < 360; i += jumpWave)
            {
                SoundManager._instance.PlaySound(SfxData.SFXType.SHOOT_2);
                for (int c = 0; c < projectilePerWave; c++)
                {
                    for (int j = 0; j < 360; j += jumpProjectile)
                    {
                        CreateStarProjectile(bulletType, (initAngle_1 + i + j) * Mathf.Deg2Rad, ind1.transform.position);
                        CreateStarProjectile(bulletType, (initAngle_2 - i - j) * Mathf.Deg2Rad, ind2.transform.position);
                    }

                    yield return new WaitForSeconds(0.05f);
                }

                yield return new WaitForSeconds(intervalBlue);
            }
        }
    }

    void CreateStarProjectile(BulletType bulletType, float rad, Vector3 from)
    {
        DefaultProjectile defaultProjectile = CreateSimpleProjectile(
                bulletType,
                Speed,
                from,
                1.5f
            );

        defaultProjectile.SetAcceleration(Acceleration);
        defaultProjectile.SetDirection(new Vector3(Mathf.Cos(rad), Mathf.Sin(rad)));
        defaultProjectile.InitializeAndShoot();
    }

    public override void OnAttackFinish()
    {
        Destroy(ind1);
        Destroy(ind2);
        base.OnAttackFinish();
    }
}