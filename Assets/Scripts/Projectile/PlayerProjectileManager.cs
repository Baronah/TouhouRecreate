using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileManager : MonoBehaviour
{
    [SerializeField] BulletData bulletScriptable;

    public static PlayerProjectileManager _instance;
    private void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(gameObject);
    }

    HashSet<PlayerProjectile> ActiveProjectiles = new();

    List<PlayerProjectile> ClearCaches = new();
    private void FixedUpdate()
    {
        ClearCaches.Clear();
        foreach (PlayerProjectile projectile in ActiveProjectiles)
        {
            projectile.UpdateEverything();
        }

        for (int i = 0; i < ClearCaches.Count; i++) ClearCaches[i].ReturnToPool();
    }

    public void CachedForPoolReturning(PlayerProjectile projectile) => ClearCaches.Add(projectile);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Projectile"))
        {
            collision.GetComponent<PlayerProjectile>().ReturnToPool();
        }
    }

    public void Register(PlayerProjectile projectile)
    {
        if (ActiveProjectiles.Contains(projectile)) return;
        ActiveProjectiles.Add(projectile);
    }

    public void Remove(PlayerProjectile projectile)
    {
        if (!ActiveProjectiles.Contains(projectile)) return;
        ActiveProjectiles.Remove(projectile);
    }

    short ProjectileSortingOder = short.MinValue;
    public void SetSamplePropertiesToTargetProjectile(BulletData.PlayerBulletType bulletType, PlayerProjectile projectile)
    {
        SetSpriteAndHitboxSizeOfProjectileByBulletType(bulletType, projectile, ProjectileSortingOder);
        ProjectileSortingOder++;
    }

    void SetSpriteAndHitboxSizeOfProjectileByBulletType(BulletData.PlayerBulletType bulletType, PlayerProjectile projectile, short SortingOrder, bool changeSortingOrder = true)
    {
        SpriteRenderer renderer = projectile.GetComponent<SpriteRenderer>();
        renderer.color = Color.white;
        renderer.sprite = bulletScriptable.Bullets[(int)bulletType];
        if (changeSortingOrder) renderer.sortingOrder = SortingOrder;

        CircleCollider2D collider = projectile.GetComponent<CircleCollider2D>();
        collider.radius = renderer.sprite.bounds.extents.magnitude / 5;
    }

    public Vector3 ProjectileBaseScale => PlayerProjectileObjectPooling._instance.ProjectileBaseScale;
    public PlayerProjectile CreateSimpleProjectile(BulletData.PlayerBulletType bulletType, float strength, float speed, Vector3 position, float scale = 1.0f)
    {
        GameObject projectile = PlayerProjectileObjectPooling._instance.GetProjectile(bulletType, position, scale);

        PlayerProjectile defaultProjectile = projectile.GetComponent<PlayerProjectile>();
        defaultProjectile.SetProperties((int) bulletType, strength, speed);

        return defaultProjectile;
    }

    public PlayerProjectile CreateHomingProjectile(BulletData.PlayerBulletType bulletType, float strength, float speed, Transform target, Vector3 position)
    {
        if (!target) return null;
        Vector3 direction = (target.position - position).normalized;
        PlayerProjectile projectile = CreateSimpleProjectile(bulletType, strength, speed, position);
        projectile.SetDirection(direction);
        return projectile;
    }
}
