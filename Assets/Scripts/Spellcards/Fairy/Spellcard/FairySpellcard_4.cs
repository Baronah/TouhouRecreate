using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

// "Supernova" 
public class FairySpellcard_4 : SpellcardBase
{
    [SerializeField] float intervalStart = 4f, intervalEnd = 1.5f;
    [SerializeField] GameObject StarCollideIndicator;
    [SerializeField] float displacement = 60f;

    BulletData.BulletType[] AllStarsBullets = new BulletData.BulletType[]
    {
        BulletData.BulletType.STAR_RED,
        BulletData.BulletType.STAR_CYAN,
        BulletData.BulletType.STAR_GREEN,
        BulletData.BulletType.STAR_GREY,
        BulletData.BulletType.STAR_MAGENTA,
        BulletData.BulletType.STAR_BLUE,
        BulletData.BulletType.STAR_ORANGE
    };

    protected override IEnumerator AttackPrepare()
    {
        yield return new WaitForSeconds(PrepareTime);
        SoundManager._instance.PlaySound(SfxData.SFXType.CHARGE_2, SoundManager.SfxChannel.EFFECT);
        ChargeEffect._instance.DoChargeEffect(spellOwner.transform.position, Color.yellow, 1.5f);
        yield return new WaitForSeconds(2f);
        OnSpellPrepareFinished();

        yield return InitializeSpellCard();
        StartCoroutine(AttackShoot());
    }

    int indexTrack = 0;
    protected override IEnumerator AttackShoot()
    {
        int length = AllStarsBullets.Length;
        while (true)
        {
            yield return StartCoroutine(MoveEnemy());
            yield return new WaitForSeconds(0.05f);

            float initAngle = Random.Range(0, 360);
            BulletData.BulletType bulletType = AllStarsBullets[indexTrack];
            SoundManager._instance.PlaySound(SfxData.SFXType.CHARGE_4, SoundManager.SfxChannel.EFFECT);
            GameObject ind = CreateIndicatorCirlce(bulletType);
            Destroy(ind, starTravelTime);

            yield return new WaitForSeconds(0.15f);

            Vector3 locationLeft = new Vector3(GameManager._instance.cornerLeftDown.position.x - 80, spellOwner.transform.position.y);
            DefaultProjectile star1 =
                CreateGiantStar(bulletType, locationLeft, Vector3.right, spellOwner.transform.position, StarRotation);

            Vector3 locationRight = new Vector3(GameManager._instance.cornerRightUp.position.x + 80, spellOwner.transform.position.y);
            DefaultProjectile star2 =
                CreateGiantStar(bulletType, locationRight, Vector3.left, spellOwner.transform.position, StarRotation * -1);

            StartCoroutine(WaitUntilTwoStarsMeet(new DefaultProjectile[] { star1, star2 }, starTravelTime, initAngle));

            yield return new WaitForSeconds(intervalStart);
            intervalStart = Mathf.Max(intervalStart - 0.5f, intervalEnd);
            starTravelTime = Mathf.Max(starTravelTime - 0.2f, starTravelMinTime);
            indexTrack = (indexTrack + 1) % length;
        }
    }

    [SerializeField] float OffsetX = 25, OffsetY = 50;
    IEnumerator MoveEnemy()
    {
        Vector3 from = spellOwner.transform.position, to = GetMovableSpot();

        float c = 0, d = 0.5f;
        while (c < d)
        {
            spellOwner.transform.position = Vector3.Lerp(from, to, c * 1.0f / d);
            c += Time.deltaTime;
            yield return null;
        }
    }

    [SerializeField] float MinYMove = 50f;
    Vector3 GetMovableSpot()
    {
        float xBoundLeft = GameManager._instance.cornerLeftDown.position.x + OffsetX;
        float xBoundRight = GameManager._instance.cornerRightUp.position.x - OffsetX;
        float yBoundUp = GameManager._instance.cornerRightUp.position.y + OffsetY;
        float yBoundDown = GameManager._instance.cornerLeftDown.position.y - OffsetY;

        float returnX = PlayerPosition.x;
        if (returnX < xBoundLeft) returnX = xBoundLeft;
        else if (returnX > xBoundRight) returnX = xBoundRight;
        
        float returnY = PlayerPosition.y;
        if (returnY > yBoundUp) returnY = yBoundUp;
        else if (returnY < yBoundDown) returnY = yBoundDown;

        return new Vector3(returnX, returnY);
    }

    [SerializeField] float starTravelTime = 3f, starTravelMinTime = 2f;
    Vector3 StarRotation = new Vector3(0, 0, 360);
    DefaultProjectile CreateGiantStar(BulletData.BulletType bulletType, Vector3 position, Vector3 direction, Vector3 targetPosition, Vector3 rotation)
    {
        float distance = Vector3.Distance(position, targetPosition);
        float speed = distance / starTravelTime;

        DefaultProjectile defaultProjectile = CreateSimpleProjectile(
            bulletType,
            speed,
            position,
            scale: 3.5f
        );

        defaultProjectile.MakeSpawnExplosionOnDisappear(false);
        defaultProjectile.SetRotation(rotation);
        defaultProjectile.SetDirection(direction);
        defaultProjectile.InitializeAndShoot();
        return defaultProjectile;
    }

    IEnumerator WaitUntilTwoStarsMeet(DefaultProjectile[] stars, float duration, float initAngle)
    {
        yield return new WaitForSeconds(duration - 0.15f);
        SoundManager._instance.PlaySound(SfxData.SFXType.BOOMM, SoundManager.SfxChannel.PLAYER);
        yield return new WaitForSeconds(0.15f);

        //ChargeEffect._instance.DoCircleEffect(stars[0].transform.position, GetColorBasedOnStar(stars[0].GetBulletType), 0.4f);
        CreateMochi(stars[0].transform.position, stars[0].GetBulletTypeAsEnum);
        foreach (DefaultProjectile star in stars)
        {
            if (!star.gameObject.activeSelf) continue;
            CreateExplosionFromStar(star, initAngle);
            initAngle += 5f;
        }
    }

    GameObject CreateIndicatorCirlce(BulletData.BulletType bulletType)
    {
        GameObject ind = Instantiate(StarCollideIndicator, spellOwner.transform.position, Quaternion.identity);
        ind.GetComponent<SpriteRenderer>().color = GetColorBasedOnStar(bulletType);
        StartCoroutine(ExpandIndicatorSize(ind));
        return ind;
    }

    IEnumerator ExpandIndicatorSize(GameObject gameObject)
    {
        gameObject.transform.localScale = Vector3.zero;
        float c = 0, d = 0.35f;
        Vector3 final = new(30, 30, 30);
        while (c < d)
        {
            gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, final, c * 1.0f / d);
            c += Time.deltaTime;
            yield return null;
        }
        gameObject.transform.localScale = final;
    }

    [SerializeField] float starExplodeSpeedMin = 50f, starExplodeSpeedMax = 150f;
    [SerializeField] float starExplodeAccelerationMin = 25f, starExplodeAccelerationMax = 100f;
    void CreateExplosionFromStar(DefaultProjectile fromStar, float initAngle)
    {
        BulletData.BulletType bulletType = fromStar.GetBulletTypeAsEnum;
        Vector3 position = fromStar.transform.position;
        fromStar.ReturnToPool();

        StartCoroutine(CreateWaveExplosionFromStar(bulletType, position, initAngle));
        CreateRingExplosionFromStar(bulletType, position, initAngle);
    }

    IEnumerator CreateWaveExplosionFromStar(BulletData.BulletType fr, Vector3 position, float initAngle)
    {
        int starCountPerWave = 5;
        int starJumpPerWave = 360 / starCountPerWave;

        float time = 1.5f;
        float timePerWave = 0.08f;
        int spread = 45;
        int waveCount = Mathf.RoundToInt(time / timePerWave);
        float waveJumpCount = spread / waveCount;
        for (float o = 0; o < spread; o += waveJumpCount)
        {
            SoundManager._instance.PlaySound(SfxData.SFXType.SHOOT_2, SoundManager.SfxChannel.SHOOT);
            for (int i = 0; i < 360; i += starJumpPerWave)
            {
                float angle = (i + o + initAngle) * Mathf.Deg2Rad;
                Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
                CreateSmallStars(fr, position, direction);
            }

            yield return new WaitForSeconds(timePerWave);
        }
    }

    void CreateRingExplosionFromStar(BulletData.BulletType fr, Vector3 position, float initAngle)
    {
        int count = 24;
        int jump = 360 / count;
        for (int i = 0; i < 360; i += jump)
        {
            float angle = (i + initAngle) * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
            CreateSmallStars(fr, position, direction, starExplodeSpeedMax, starExplodeAccelerationMax);
        }
    }

    void CreateMochi(Vector3 position, BulletData.BulletType bulletType)
    {
        DefaultProjectile mochi = CreateSimpleProjectile(
            GetMochiTypeBasedOnStar(bulletType),
            0f,
            position,
            scale: 0f
        );

        mochi.MakeSpawnExplosionOnDisappear(false);
        mochi.SetRotation(StarRotation * 2);
        mochi.SetSpeedExhaustType(DefaultProjectile.SpeedExhaustType.STAY_STILL);
        mochi.SetDirection(Vector3.zero);
        mochi.InitializeAndShoot();

        StartCoroutine(ProjectileShrinkUpAndDown(mochi));
    }

    IEnumerator ProjectileShrinkUpAndDown(DefaultProjectile defaultProjectile)
    {
        float c = 0, d = 0.35f;
        Vector3 finalScale = 3 * ProjectileManager._instance.ProjectileBaseScale;

        while (c < d)
        {
            defaultProjectile.transform.localScale = Vector3.Lerp(defaultProjectile.transform.localScale, finalScale, c * 1.0f / d);
            c += Time.deltaTime;
            yield return null;
        }

        defaultProjectile.transform.localScale = finalScale;
        ProjectileManager._instance.CalculateHitboxSize(defaultProjectile);
        yield return new WaitForSeconds(1.5f);

        c = 0;
        d = 1;
        while (c < d)
        {
            defaultProjectile.transform.localScale = Vector3.Lerp(defaultProjectile.transform.localScale, Vector3.zero, c * 1.0f / d);
            c += Time.deltaTime;
            yield return null;
        }

        defaultProjectile.ReturnToPool();
    }

    BulletData.BulletType GetMochiTypeBasedOnStar(BulletData.BulletType starType)
    {
        return starType switch
        {
            BulletData.BulletType.STAR_RED => BulletData.BulletType.CIRCLE_RED,
            BulletData.BulletType.STAR_CYAN => BulletData.BulletType.CIRCLE_MINT,
            BulletData.BulletType.STAR_GREEN => BulletData.BulletType.CIRCLE_GREEN,
            BulletData.BulletType.STAR_GREY => BulletData.BulletType.CIRCLE_GREY,
            BulletData.BulletType.STAR_MAGENTA => BulletData.BulletType.CIRCLE_PINK,
            BulletData.BulletType.STAR_BLUE => BulletData.BulletType.CIRCLE_BLUE,
            BulletData.BulletType.STAR_ORANGE => BulletData.BulletType.CIRCLE_ORANGE,
            _ => BulletData.BulletType.CIRCLE_YELLOW
        };
    }

    Color GetColorBasedOnStar(BulletData.BulletType starType)
    {
        return starType switch
        {
            BulletData.BulletType.STAR_RED => Color.red,
            BulletData.BulletType.STAR_CYAN => Color.cyan,
            BulletData.BulletType.STAR_GREEN => Color.green,
            BulletData.BulletType.STAR_GREY => Color.gray,
            BulletData.BulletType.STAR_MAGENTA => Color.magenta,
            BulletData.BulletType.STAR_BLUE => Color.blue,
            BulletData.BulletType.STAR_ORANGE => new(1f, 0.52f, 0),
            _ => Color.yellow
        };
    }

    void CreateSmallStars(BulletData.BulletType bulletType, Vector3 position, Vector3 direction, float speed, float acceleration)
    {
        DefaultProjectile defaultProjectile = CreateSimpleProjectile(
            bulletType,
            speed,
            position
        ); 

        defaultProjectile.SetDirection(direction);
        defaultProjectile.SetAcceleration(acceleration);
        defaultProjectile.SetRotation(StarRotation);
        defaultProjectile.InitializeAndShoot();
    }

    void CreateSmallStars(BulletData.BulletType bulletType, Vector3 position, Vector3 direction)
    {
        CreateSmallStars(bulletType, position, direction, Random.Range(starExplodeSpeedMin, starExplodeSpeedMax), Random.Range(starExplodeAccelerationMin, starExplodeAccelerationMax));
    }
}