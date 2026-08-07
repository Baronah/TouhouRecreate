using System;
using UnityEngine;
using static BulletData;

public class DefaultProjectile : MonoBehaviour
{
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
    public SpeedExhaustType speedExhaustType { get; protected set; } = SpeedExhaustType.CONTINUE;

    int bulletType;
    public void SetBulletType(int bulletType) => this.bulletType = bulletType;
    public int GetBulletType => bulletType;
    public BulletType GetBulletTypeAsEnum => (BulletType)bulletType;

    SpriteRenderer spriteRenderer;

    public Vector3 direction { get; protected set; }
    public float speed;

    Vector3 rotationPerSec;

    public float displacementDegreePerSec { get; protected set; }

    float damage;
    public float GetDamage => damage;

    public float hitboxRadius;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public bool initialized = false;
    public float accelerationPerSecond { get; protected set; } = 0f;

    public void SetProperties(int type, float damage, float speed)
    {
        Start();

        this.damage = damage;
        bulletType = type;
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

    public virtual void Stop()
    {
        timePassed = 0;
        speed = 0;
        accelerationPerSecond = 0f;
        initVelocity = Vector3.zero;
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
    protected virtual void UpdateDirection()
    {
        if (Mathf.Abs(displacementDegreePerSec) <= 0f) return;

        float turnThisFrame = displacementDegreePerSec * Time.fixedDeltaTime;
        Quaternion turnQuaternion = Quaternion.AngleAxis(turnThisFrame, Vector3.forward);
        Vector3 turnedDirection = turnQuaternion * direction;

        // Blend between current direction and turned direction
        direction = Vector3.Slerp(direction, turnedDirection, spiralDamping).normalized;
    }

    Vector3 initVelocity;
    float timePassed = 0f;

    protected virtual void UpdateAcceleration()
    {
        timePassed += Time.fixedDeltaTime;
        
        // acceleration
        // v = v0 + at
        speed = initVelocity.magnitude + accelerationPerSecond * timePassed;

        CheckSpeed();
    }

    protected virtual void CheckSpeed()
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

    public virtual void UpdateEverything()
    {
        if (!initialized) return;

        UpdateAcceleration();
        UpdateDirection();
        UpdateRotation();
        UpdatePosition();
        CheckForCollision();
    }

    public virtual void UpdatePosition()
    {
        transform.position += direction * speed * Time.fixedDeltaTime;

        if (GameManager._instance.hasShotOverbound(transform.position))
        {
            spawnExplosionOnReturn = false;
            ProjectileManager._instance.CachedForPoolReturning(this);
        }
    }

    public virtual void CheckForCollision()
    {
        // collision check against player — just distance
        if (Vector3.SqrMagnitude(transform.position - PlayerManager._instance.PlayerPosition) < (hitboxRadius + PlayerManager._instance.GetPlayerHitboxRaidus()))
            PlayerManager._instance.ActivePlayer.OnProjectileHit(this);
    }

    protected virtual void UpdateRotation()
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

    public virtual void ReturnToPool()
    {
        if (!gameObject.activeSelf || !initialized) return;
        initialized = false;
        ResetProperties();
        if (spawnExplosionOnReturn) 
            BulletBreakObjectPooling._instance.CreateExplosionAt(transform.position, transform.localScale.x / ProjectileManager._instance.ProjectileBaseScale.x);
        spawnExplosionOnReturn = true;
        ProjectileObjectPooling._instance.ReturnProjectile(this);
    }

    public virtual void ResetProperties()
    {
        speed = accelerationPerSecond = displacementDegreePerSec = spiralDamping = 0f;
        initVelocity = rotationPerSec = direction = Vector3.zero;
        timePassed = 0f;
    }

    private void OnDrawGizmos()
    {
        if (!initialized) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, hitboxRadius);
    }
}
