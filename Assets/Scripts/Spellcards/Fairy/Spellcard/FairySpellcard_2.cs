using System.Collections;
using UnityEngine;

// "When You Wish upon the Stars"
public class FairySpellcard_2 : SpellcardBase
{
    [SerializeField] int projectileCount = 80;
    [SerializeField] float shootInterval = 0.125f;
    private static WaitForSeconds _waitForSecondsInterval;

    protected override IEnumerator AttackShoot()
    {
        _waitForSecondsInterval = new WaitForSeconds(shootInterval);
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
                SoundManager._instance.PlaySound(SfxData.SFXType.SHOOT_1, SoundManager.SfxChannel.SHOOT);
                CreateFallingProjectileAndTrack(ceilingLeft + distanceJump * i * Vector3.right);
                CreateFallingProjectileAndTrack(ceilingRight + distanceJump * i * Vector3.left);

                yield return _waitForSecondsInterval;
            }
        }
    }

    BulletData.BulletType[] AllStarsBullets = new BulletData.BulletType[]
    {
        BulletData.BulletType.STAR_RED,
        BulletData.BulletType.STAR_CYAN,
        BulletData.BulletType.STAR_GREEN,
        BulletData.BulletType.STAR_MAGENTA,
        BulletData.BulletType.STAR_BLUE,
        BulletData.BulletType.STAR_ORANGE
    };

    Vector3 fallingRotateVector = new(0, 0, 180f);
    Vector3 risingRotateVector = new(0, 0, -60f);

    void CreateFallingUpProjectile(DefaultProjectile projectile)
    {
        SoundManager._instance.PlaySound(SfxData.SFXType.REVERSE, SoundManager.SfxChannel.TRANSFORM);

        ProjectileManager._instance.ChangeBulletTypeOfCurrentProjectile(projectile, BulletData.BulletType.STAR_GREY);
        projectile.SetSpeed(5f);
        projectile.SetRotation(risingRotateVector);
        projectile.SetAcceleration(30);
        projectile.SetDirection(Vector3.up);
        projectile.InitializeAndShoot();
    }

    void CreateFallingProjectileAndTrack(Vector3 initPos)
    {
        DefaultProjectile defaultProjectile = CreateSimpleProjectile(
                AllStarsBullets[Random.Range(0, AllStarsBullets.Length)],
                Random.Range(30, 200),
                initPos,
                scale: 0.85f
            );

        defaultProjectile.SetRotation(fallingRotateVector);
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

        CreateFallingUpProjectile(toTrack);
    }
}