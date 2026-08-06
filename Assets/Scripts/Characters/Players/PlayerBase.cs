using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public abstract class PlayerBase : MonoBehaviour
{
    private static readonly int MLeftHash = Animator.StringToHash("m_left");
    private static readonly int MRightHash = Animator.StringToHash("m_right");
    private static readonly int FocusHash = Animator.StringToHash("focus");

    private float shootInterval = 0.07f;
    private float shootTimer = 0f;
    [SerializeField] protected float moveSpeed = 20f, focusSpeed = 7f;
    
    [SerializeField] [Range(0, mFirePower)] protected float firepower = 1f;
    protected const float mFirePower = 4f;
    protected float GetBulletFirepower(float baseDamage = 1f)
    {
        return Mathf.Lerp(baseDamage, baseDamage * 2f, firepower / mFirePower);
    }

    protected Rigidbody2D rb;
    [SerializeField] GameObject HitboxShow;
    [SerializeField] GameObject ExplosionSfx;
    [SerializeField] Transform ShootPosition;
    [SerializeField] SpriteRenderer Sprite;

    Animator animator;

    float InvunerableTimer = 2f;
    public bool IsInvulnerable => InvunerableTimer > 0f;
    public void SetInvulnerable(float duration)
    {
        InvunerableTimer = Mathf.Max(InvunerableTimer, duration);
    }

    bool isReady = false;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();

        SetInvulnerable(2f);

        PlayerManager._instance.Register(this);

        StartCoroutine(GetReady());
    }

    IEnumerator GetReady()
    {
        Vector3 start = transform.position, final = PlayerManager._instance.GetPlayerReadyPosition();
        float c = 0, d = 0.8f;
        while (c < d)
        {
            transform.position = Vector3.Lerp(start, final, c * 1.0f / d);
            c += Time.deltaTime;
            yield return null;
        }

        transform.position = final;
        isReady = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isReady 
            || IsDead 
            || PauseScreen._instance.IsGamePaused
            || GameManager._instance.IsGameOver) return;

        UpdateCooldowns();
        SetFocus();

        if (Input.GetKey(InputManager.ShootKey))
        {
            Shoot();
        }

        if (Input.GetKeyDown(InputManager.BoomKey))
        {
            Boom();
        }

        Move();
    }

    //void CheckForBulletContact()
    //{
    //    foreach (var bulletPos in BulletRenderer.instance.GetActiveBulletPositions(danmakuTypes))
    //    {
    //        if (Vector3.Distance(transform.position, bulletPos) < 5)
    //        {
    //            OnProjectileHit();
    //            BulletRenderer.instance.Re(bulletPos);
    //        }
    //    }
    //}

    public virtual void UpdateCooldowns()
    {
        if (shootTimer > 0)
        {
            shootTimer -= Time.deltaTime;
        }

        if (InvunerableTimer > 0)
        {
            InvunerableTimer -= Time.deltaTime;
        }
    }

    protected bool CanShoot()
    {
        bool isTrue = shootTimer <= 0;
        if (isTrue)
        {
            shootTimer = shootInterval;
        }
        return isTrue;
    }

    protected bool focusing = false;
    protected virtual void SetFocus()
    {
        focusing = Input.GetKey(InputManager.FocusKey);
        animator.SetBool(FocusHash, focusing);
        HitboxShow.SetActive(focusing);
    }

    protected virtual void Move()
    {
        Vector2 movement = InputManager.GetMovementInput().normalized;
        Vector2 velocity = movement * (focusing ? focusSpeed : moveSpeed);
        rb.velocity = velocity;
        animator.SetBool(MLeftHash, velocity.x < 0);
        animator.SetBool(MRightHash, velocity.x > 0);
    }

    protected abstract void Shoot();
    protected abstract void Boom();

    public virtual void OnProjectileHit(DefaultProjectile projectile)
    {
        if (!isReady || IsDead || IsInvulnerable || !gameObject.activeSelf) return;
        OnDeath();
    }

    bool IsDead = false;
    public void OnDeath()
    {
        rb.velocity = Vector2.zero;
        IsDead = true;
        HitboxShow.SetActive(false);
        PlayerManager._instance.OnPlayerDeath(this);
        SoundManager._instance.PlaySound(SfxData.SFXType.PLAYER_EXPLODE, SoundManager.SfxChannel.PLAYER_DEATH, 0.7f);
        StartCoroutine(Explode());
        StartCoroutine(PlayerFade());
    }

    IEnumerator Explode()
    {
        GameObject explode = Instantiate(ExplosionSfx, transform);
        SpriteRenderer explodeSprite = explode.GetComponent<SpriteRenderer>();
        Color init = explodeSprite.color;
        Vector3 explodeScaleMax = new(7, 7, 7);
        Color endColor = new Color(init.r, init.g, init.b, 0);
        float c = 0, d = 0.5f;
        while (c < d)
        {
            explode.transform.localScale = Vector3.Lerp(explode.transform.localScale, explodeScaleMax, c * 1.0f / d);
            explodeSprite.color = Color.Lerp(init, endColor, c * 1.0f / d);
            c += Time.deltaTime;
            yield return null;
        }

        explode.transform.localScale = explodeScaleMax;
        explodeSprite.color = endColor;
    }

    IEnumerator PlayerFade()
    {
        Color endColor = new Color(1, 1, 1, 0);
        float c = 0, d = 0.7f;
        while (c < d)
        {
            Sprite.color = Color.Lerp(Color.white, endColor, c * 1.0f / d);
            c += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
        Destroy(this.gameObject, 1f);
    }

    public void CreateProjectileAndShoot(BulletData.BulletType bulletType, float baseDamage, float speed, float acceleration, Vector3 direction, Vector3? position = null)
    {
        DefaultProjectile projectile = 
            ProjectileManager._instance.CreateSimpleProjectile(
                bulletType, 
                DefaultProjectile.TargetType.ENEMY, 
                GetBulletFirepower(baseDamage), 
                speed, 
                position ?? ShootPosition.position
            );

        projectile.MakeSpawnExplosionOnDisappear(false);
        projectile.SetColor(new Color(1, 1, 1, 0.5f));
        projectile.SetAcceleration(acceleration);
        projectile.SetDirection(direction);
        projectile.InitializeAndShoot();
    }

}
