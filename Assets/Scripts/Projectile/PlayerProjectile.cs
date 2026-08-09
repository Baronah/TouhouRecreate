using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerProjectile : DefaultProjectile
{
    Rigidbody2D rb2d;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        if (!rb2d) rb2d = GetComponent<Rigidbody2D>();
    }

    public override void Stop()
    {
        base.Stop();
        rb2d.velocity = Vector2.zero;
    }

    public override void UpdateEverything()
    {
        if (!initialized) return;
        base.UpdateEverything();
        UpdateVelocity();
    }

    public override void CheckForCollision()
    {
        
    }

    protected void UpdateVelocity()
    {
        if (!rb2d) rb2d.velocity = direction.normalized * speed;
    }

    public override void ReturnToPool()
    {
        if (!gameObject.activeSelf || !initialized) return;
        initialized = false;
        ResetProperties();
        PlayerProjectileObjectPooling._instance.ReturnProjectile(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyBase>().OnProjectileHit(this);
            ReturnToPool();
        }
    }

    private void OnDrawGizmos()
    {
        if (!initialized) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, hitboxRadius);
    }
}
