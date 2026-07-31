using System.Collections;
using System.Reflection;
using UnityEngine;

public class FairySpellcard_2 : SpellcardBase
{
    private static WaitForSeconds _waitForSeconds0_2 = new WaitForSeconds(0.15f);

    protected override IEnumerator SpellcardShoot()
    {
        yield return StartCoroutine(InitializeSpellCardPreEffect());
        yield return StartCoroutine(CreateCeilShots());
    }

    Vector3 ceilingLeft, ceilingRight;
    IEnumerator CreateCeilShots()
    {
        ceilingLeft = GameManager._instance.cornerLeftUp.position;
        ceilingRight = GameManager._instance.cornerRightUp.position;

        int projectileCount = 80;
        float distanceJump = Vector3.Distance(ceilingLeft, ceilingRight) / projectileCount;

        float timeUntilMiddle = 0.125f * projectileCount / 2;

        StartCoroutine(CreateSlowFallingShoot(projectileCount, distanceJump));
        yield return new WaitForSeconds(timeUntilMiddle);
        StartCoroutine(CreateSlowFallingShoot(projectileCount, distanceJump));
        yield return new WaitForSeconds(timeUntilMiddle / 2);
        StartCoroutine(CreateFallingShoot(projectileCount, distanceJump));
        yield return new WaitForSeconds(timeUntilMiddle / 2);
        StartCoroutine(CreateFallingShoot(projectileCount, distanceJump));
        yield return new WaitForSeconds(timeUntilMiddle / 2);
        StartCoroutine(CreateFallingShoot(projectileCount, distanceJump));
    }

    IEnumerator CreateFallingShoot(int projectileCount, float distanceJump)
    {
        while (true)
        {
            for (int i = 0; i < projectileCount; ++i)
            {
                CreateFallingProjectile(ceilingLeft + distanceJump * i * Vector3.right);
                CreateFallingProjectile(ceilingRight + distanceJump * i * Vector3.left);

                yield return _waitForSeconds0_2;
            }
        }
    }

    IEnumerator CreateSlowFallingShoot(int projectileCount, float distanceJump)
    {
        while (true)
        {
            for (int i = 0; i < projectileCount; ++i)
            {
                CreateSlowFallingProjectile(ceilingLeft + distanceJump * i * Vector3.right);
                CreateSlowFallingProjectile(ceilingRight + distanceJump * i * Vector3.left);

                yield return _waitForSeconds0_2;
            }
        }
    }

    void CreateSlowFallingProjectile(Vector3 initPos)
    {
        DefaultProjectile defaultProjectile = CreateSimpleProjectile(
                BulletData.BulletType.KUNAI_CYAN,
                70,
                initPos,
                scale: 1.3f
            );

        defaultProjectile.SetAcceleration(30);
        defaultProjectile.SetDirection(Vector3.down);
        defaultProjectile.InitializeAndShoot();
    }

    void CreateFallingProjectile(Vector3 initPos)
    {
        DefaultProjectile defaultProjectile = CreateSimpleProjectile(
                BulletData.BulletType.KUNAI_DARK_BLUE,
                Random.Range(30, 200),
                initPos
            );

        defaultProjectile.SetAcceleration(Random.Range(50, 150));
        defaultProjectile.SetDirection(Vector3.down);
        defaultProjectile.InitializeAndShoot();
    }
}