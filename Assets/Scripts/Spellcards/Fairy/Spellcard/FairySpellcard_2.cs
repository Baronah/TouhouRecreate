using System.Collections;
using System.Reflection;
using UnityEngine;

// "Witch's Tears" (魔女の涙)
public class FairySpellcard_2 : SpellcardBase
{
    [SerializeField] int projectileCount = 80;
    [SerializeField] float shootInterval = 0.125f;
    private static WaitForSeconds _waitForSecondsInterval;

    protected override IEnumerator SpellcardShoot()
    {
        _waitForSecondsInterval = new WaitForSeconds(shootInterval);
        yield return StartCoroutine(InitializeSpellCardPreEffect());
        yield return StartCoroutine(CreateCeilShots());
    }

    Vector3 ceilingLeft, ceilingRight;
    IEnumerator CreateCeilShots()
    {
        ceilingLeft = GameManager._instance.cornerLeftUp.position;
        ceilingRight = GameManager._instance.cornerRightUp.position;

        float distanceJump = Vector3.Distance(ceilingLeft, ceilingRight) / projectileCount;

        float timeUntilMiddle = shootInterval * projectileCount / 2;

        StartCoroutine(CreateFallingShoot(projectileCount, distanceJump));
        yield return new WaitForSeconds(timeUntilMiddle);
        StartCoroutine(CreateFallingShoot(projectileCount, distanceJump));
    }

    IEnumerator CreateFallingShoot(int projectileCount, float distanceJump)
    {
        while (true)
        {
            for (int i = 0; i < projectileCount; ++i)
            {
                CreateFallingProjectileAndTrack(ceilingLeft + distanceJump * i * Vector3.right);
                CreateFallingProjectileAndTrack(ceilingRight + distanceJump * i * Vector3.left);

                yield return _waitForSecondsInterval;
            }
        }
    }

    void CreateFallingUpProjectile(Vector3 initPos)
    {
        DefaultProjectile defaultProjectile = CreateSimpleProjectile(
                BulletData.BulletType.KUNAI_CYAN,
                5,
                initPos,
                scale: 1.5f
            );

        defaultProjectile.SetAcceleration(30);
        defaultProjectile.SetDirection(Vector3.up);
        defaultProjectile.InitializeAndShoot();
    }

    void CreateFallingProjectileAndTrack(Vector3 initPos)
    {
        DefaultProjectile defaultProjectile = CreateSimpleProjectile(
                BulletData.BulletType.KUNAI_DARK_BLUE,
                Random.Range(30, 200),
                initPos,
                scale: 1.25f
            );

        defaultProjectile.SetAcceleration(Random.Range(50, 150));
        defaultProjectile.SetDirection(Vector3.down);
        defaultProjectile.InitializeAndShoot();

        StartCoroutine(WaitUntilOverBound(defaultProjectile));
    }

    IEnumerator WaitUntilOverBound(DefaultProjectile toTrack)
    {
        yield return new WaitUntil(() =>
            hasShotOverbound(toTrack.transform.position) || !toTrack.gameObject.activeSelf
        );

        CreateFallingUpProjectile(toTrack.transform.position);
        toTrack.ReturnToPool();
    }
}