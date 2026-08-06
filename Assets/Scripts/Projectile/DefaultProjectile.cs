using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BulletData;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class DefaultProjectile : MonoBehaviour
{
    public enum TargetType
    {
        NONE,
        ENEMY,
        PLAYER
    }

    private TargetType targetType = TargetType.NONE;

    public enum SpeedExhaustType
    {
        STAY_STILL,
        CLEAR,
        CONTINUE,
    }

    // What the projectile should do after its speed reached 0.
    // STAY_STILL: The projectile will stay still at its current position.
    // CLEAR: The projectile will be returned to the pool.
    // CONTINUE: The projectile will continue to move. If acceleration is negative, it will move backward.
    private SpeedExhaustType speedExhaustType = SpeedExhaustType.CONTINUE;

    BulletType bulletType;
    public void SetBulletType(BulletType bulletType) => this.bulletType = bulletType;
    public BulletType GetBulletType => bulletType;

    SpriteRenderer spriteRenderer;
    Rigidbody2D rb2d;

    Vector3 direction;
    float speed;

    Vector3 rotationPerSec;

    float displacementDegreePerSec;

    float damage;
    public float GetDamage => damage;

    // Start is called before the first frame update
    void Start()
    {
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        if (!rb2d) rb2d = GetComponent<Rigidbody2D>();
    }

    float lifeTime = 999f;

    public bool initialized = false;
    float accelerationPerSecond = 0f;
    
    public void SetProperties(BulletType type, TargetType targetType, float damage, float speed)
    {
        Start();

        this.damage = damage;
        bulletType = type;
        this.targetType = targetType;
        SetSpeed(speed);
    }

    public void InitializeAndShoot()
    {
        this.initVelocity = direction.normalized * this.speed;
        initialized = true;
    }

    public void SetDirection(Vector3 direction)
    {
        this.direction = direction;
    }

    public void SetRotation(Vector3 rotationPersec)
    {
        this.rotationPerSec = rotationPersec;
    }

    public void SetDisplacement(float displacement, float spiralDamp = 0.5f)
    {
        this.displacementDegreePerSec = displacement;
        this.spiralDamping = spiralDamp;
    }

    public void SetAcceleration(float acceleration)
    {
        timePassed = 0f;
        this.accelerationPerSecond = acceleration;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public void SetColor(Color color)
    {
        spriteRenderer.color = color;
    }

    public void Stop()
    {
        timePassed = 0;
        speed = 0;
        accelerationPerSecond = 0f;
        rb2d.velocity = initVelocity = Vector2.zero;
    }

    public void SetSpeedExhaustType(SpeedExhaustType speedExhaustType)
    {
        this.speedExhaustType = speedExhaustType;
    }

    public void SetScale(float scale)
    {
        transform.localScale = ProjectileManager._instance.ProjectileBaseScale * scale;
    }

    float spiralDamping = 0.5f;
    void UpdateDirection()
    {
        if (Mathf.Abs(displacementDegreePerSec) <= 0f) return;
        // Calculate how much to turn this frame
        float turnThisFrame = displacementDegreePerSec * Time.fixedDeltaTime;
        Quaternion turnQuaternion = Quaternion.AngleAxis(turnThisFrame, Vector3.forward);
        Vector3 turnedDirection = turnQuaternion * direction;

        // Blend between current direction and turned direction
        // This creates a spiral instead of a loop
        direction = Vector3.Slerp(direction, turnedDirection, spiralDamping).normalized;
    }

    Vector3 initVelocity;
    float timePassed = 0f;
    void UpdateVelocity()
    {
        if (!rb2d) return;
        
        timePassed += Time.fixedDeltaTime;
        // acceleration
        speed = initVelocity.magnitude + accelerationPerSecond * timePassed;

        CheckSpeed();

        rb2d.velocity = direction.normalized * speed;
    }

    void CheckSpeed()
    {
        if (speed > 0) return;
        switch (speedExhaustType)
        {
            case SpeedExhaustType.STAY_STILL:
                speed = 0;
                break;
            case SpeedExhaustType.CLEAR:
                ReturnToPool();
                return;
            case SpeedExhaustType.CONTINUE:
                break;
        }
    }

    private void FixedUpdate()
    {
        if (!initialized) return;

        UpdateDirection();
        UpdateVelocity();
        UpdateRotation();
    }

    void UpdateRotation()
    {
        if (rotationPerSec != Vector3.zero) transform.Rotate(rotationPerSec * Time.fixedDeltaTime);
        else
        {
            float desiredZRotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
            transform.rotation = Quaternion.Euler(0f, 0f, desiredZRotation);
        }
    }

    bool spawnExplosionOnReturn = true;
    public void MakeSpawnExplosionOnDisappear(bool value) => spawnExplosionOnReturn = value;

    public void ReturnToPool()
    {
        if (!gameObject.activeSelf || !initialized) return;
        initialized = false;
        ResetProperties();
        if (spawnExplosionOnReturn) 
            BulletBreakObjectPooling._instance.CreateExplosionAt(transform.position, transform.localScale.x / ProjectileManager._instance.ProjectileBaseScale.x);
        targetType = TargetType.NONE;
        spawnExplosionOnReturn = true;
        ProjectileObjectPooling._instance.ReturnProjectile(this);
    }

    public void ResetProperties()
    {
        speed = accelerationPerSecond = displacementDegreePerSec = spiralDamping = 0f;
        initVelocity = rotationPerSec = Vector3.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (targetType == TargetType.ENEMY && collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyBase>().OnProjectileHit(this);
            spawnExplosionOnReturn = false;
            ReturnToPool();
        }
        else if (targetType == TargetType.PLAYER && collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerBase>().OnProjectileHit(this);
        }
    }
}
