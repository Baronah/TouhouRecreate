using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// "Shioiri Nightfall"
public class FairySpellcard_5 : SpellcardBase
{
    [SerializeField] float interval = 2f;
    [SerializeField] float maxMvmtWidth = 300f, maxMvmtHeight = 100f;
    [SerializeField] float speed = 200f;

    Rigidbody2D rb2d;
    protected override IEnumerator AttackShoot()
    {
        rb2d = spellOwner.GetComponent<Rigidbody2D>();
        yield return StartCoroutine(MoveToTarget(spellOwner.InitPos + new Vector3(0, -100)));
        StartCoroutine(CreateEverExpandingStar());
        yield return new WaitForSeconds(interval + 1f);

        StartCoroutine(RandomizeMovement());
        yield return new WaitForSeconds(0.5f);
        while (true)
        {
            yield return new WaitForSeconds(interval);
            StartCoroutine(CreateEverExpandingStar());
        }
    }

    [SerializeField] float initStarSpeed = 400f, initStarAccele = -500f;
    [SerializeField] float smallStarSpeed = 200f, smallStarAccele = -350f;
    float minScale = 0.7f;
    IEnumerator CreateEverExpandingStar()
    {
        int loopCount = 0;
        float initScale = 3f;

        SoundManager._instance.PlaySound(SfxData.SFXType.SHOOT_1);
        DefaultProjectile firstStar = 
            CreateStarShootingDown(bulletsUse[loopCount], spellOwner.transform.position, Vector3.up, initScale, initStarSpeed, initStarAccele);
        yield return new WaitUntil(() => firstStar.speed <= 0);

        List<DefaultProjectile> CreatedStars = new();
        CreatedStars.Add(firstStar);

        initScale -= 0.7f;
        while (loopCount < 3)
        {
            loopCount++;
            List<DefaultProjectile> smallStars = new();
            SoundManager._instance.PlaySound(SfxData.SFXType.ENEMY_VANISH, SoundManager.SfxChannel.TRANSFORM, 0.5f);
            for (int i = 0; i < CreatedStars.Count; i++)
            {
                Vector3 pos = CreatedStars[i].transform.position;
                float InitAngle = Random.Range(0, 360);
                for (int j = 0; j < 360; j += 360 / 5)
                {
                    float angle = (InitAngle + j) * Mathf.Deg2Rad;
                    Vector3 dir = new(Mathf.Cos(angle), Mathf.Sin(angle));
                    DefaultProjectile star = CreateStarShootingDown(
                        bulletsUse[loopCount], 
                        pos, 
                        dir, 
                        initScale, 
                        smallStarSpeed, 
                        smallStarAccele);
                    smallStars.Add(star);
                }

                CreatedStars[i].ReturnToPool();
            }

            initScale -= 0.7f;
            CreatedStars = new(smallStars);
            if (loopCount < 3) yield return new WaitForSeconds(1.1f);
        }

        yield return new WaitForSeconds(0.8f);
        
        // SoundManager._instance.PlaySound(SfxData.SFXType.REVERSE, SoundManager.SfxChannel.TRANSFORM);
        foreach (var star in CreatedStars)
        {
            star.SetDirection(Vector3.down);
            star.SetSpeed(5f);
            star.SetAcceleration(Random.Range(15, 70));
            star.InitializeAndShoot();
        }
    }

    Vector3 rotation = new(0, 0, 200);
    DefaultProjectile CreateStarShootingDown(
        BulletData.BulletType type,
        Vector3 pos, 
        Vector3 direction, 
        float scale, float 
        speed, 
        float acceleration)
    {
        DefaultProjectile star = CreateSimpleProjectile(type, speed, pos, scale);
        star.SetDirection(direction);
        star.SetAcceleration(acceleration);
        star.SetSpeedExhaustType(DefaultProjectile.SpeedExhaustType.STAY_STILL);
        star.SetRotation(rotation);
        star.InitializeAndShoot();

        return star;
    }

    IEnumerator RandomizeMovement()
    {
        Vector2 origin = spellOwner.transform.position; // captured at init

        while (true)
        {
            Vector2 target = GetRandomPointInEllipse(origin, maxMvmtWidth, maxMvmtHeight);

            rb2d.velocity = Vector2.zero;
            yield return StartCoroutine(MoveToTarget(target));
        }
    }

    IEnumerator MoveToTarget(Vector2 target)
    {
        float threshold = 20f;

        while (Vector2.Distance(rb2d.position, target) > threshold)
        {
            Vector2 direction = (target - rb2d.position).normalized;
            rb2d.velocity = direction * speed;
            yield return new WaitForFixedUpdate();
        }

        rb2d.velocity = Vector2.zero;
    }

    Vector2 GetRandomPointInEllipse(Vector2 center, float width, float height)
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float r = Mathf.Sqrt(Random.value); // sqrt for uniform area distribution

        float x = center.x + (width / 2f) * r * Mathf.Cos(angle);
        float y = center.y + (height / 2f) * r * Mathf.Sin(angle);

        return new Vector2(x, y);
    }

    public override void OnAttackFinish()
    {
        base.OnAttackFinish();
        StartCoroutine(MoveToTarget(spellOwner.InitPos));
    }
}