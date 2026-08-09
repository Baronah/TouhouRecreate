using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static BulletData;
using static DefaultProjectile;

public class ProjectileManager : MonoBehaviour
{
    [SerializeField] BulletData bulletScriptable;

    public static ProjectileManager _instance;
    private void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(gameObject);
    }

    HashSet<DefaultProjectile> ActiveProjectiles = new();

    [SerializeField] TMP_Text BulletCountTxt;
    private void Update()
    {
        if (BulletCountTxt) BulletCountTxt.text = string.Format("{0:N0}", ActiveProjectiles.Count);
    }

    List<DefaultProjectile> ClearCaches = new();
    private void FixedUpdate()
    {
        ClearCaches.Clear();
        foreach (DefaultProjectile projectile in ActiveProjectiles)
        {
            projectile.UpdateEverything();
        }

        for (int i = 0; i < ClearCaches.Count; i++) ClearCaches[i].ReturnToPool();
    }

    public void CachedForPoolReturning(DefaultProjectile projectile) => ClearCaches.Add(projectile);

    public List<DefaultProjectile> GetProjectilesOfType(BulletType bulletType)
    {
        return ActiveProjectiles.Where(p => p.gameObject.activeSelf && p.GetBulletTypeAsEnum == bulletType).ToList();
    }

    public List<DefaultProjectile> GetProjectilesOfType(BulletType[] bulletTypes)
    {
        return ActiveProjectiles.Where(p => p.gameObject.activeSelf && bulletTypes.Contains(p.GetBulletTypeAsEnum)).ToList();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Projectile"))
        {
            collision.GetComponent<DefaultProjectile>().ReturnToPool();
        }
    }

    public void Register(DefaultProjectile projectile)
    {
        if (ActiveProjectiles.Contains(projectile)) return;
        ActiveProjectiles.Add(projectile);
    }

    public void Remove(DefaultProjectile projectile)
    {
        if (!ActiveProjectiles.Contains(projectile)) return;
        ActiveProjectiles.Remove(projectile);
    }

    public void ClearShootsOfType(BulletType bulletType)
    {
        ClearShootsOfType(new[] { bulletType });
    }

    public void ClearShootsOfType(BulletType[] bulletTypes)
    {
        if (bulletTypes.Length <= 0) return;

        DefaultProjectile[] ToClear =
            ActiveProjectiles.Where(p => bulletTypes.Contains(p.GetBulletTypeAsEnum)).ToArray();

        int count = ToClear.Length;
        for (int i = 0; i < count; ++i)
        {
            DefaultProjectile projectile = ToClear[i];
            if (!projectile.gameObject.activeSelf) continue;
            projectile.ReturnToPool();
        }
    }

    public List<DefaultProjectile> ChangeShootsOfType(BulletType fromType, BulletType toType)
    {
        return ChangeShootsOfType(new[] { fromType }, toType);
    }

    public List<DefaultProjectile> ChangeShootsOfType(BulletType[] fromType, BulletType toType)
    {
        List<DefaultProjectile> bullets = GetProjectilesOfType(fromType);
        foreach (DefaultProjectile bullet in bullets) ChangeBulletTypeOfCurrentProjectile(bullet, toType);
        return bullets;
    }

    short ProjectileSortingOder = short.MinValue;
    public void SetSamplePropertiesToTargetProjectile(BulletData.BulletType bulletType, DefaultProjectile projectile)
    {
        SetSpriteAndHitboxSizeOfProjectileByBulletType(bulletType, projectile, ProjectileSortingOder);
        ProjectileSortingOder++;
    }

    public void ChangeBulletTypeOfCurrentProjectile(DefaultProjectile currentProjectile, BulletData.BulletType newBulletType)
    {
        currentProjectile.SetBulletType((int) newBulletType);
        SetSpriteAndHitboxSizeOfProjectileByBulletType(newBulletType, currentProjectile, 0, false);
    }

    void SetSpriteAndHitboxSizeOfProjectileByBulletType(BulletData.BulletType bulletType, DefaultProjectile projectile, short SortingOrder, bool changeSortingOrder = true)
    {
        SpriteRenderer renderer = projectile.GetSpriteRenderer();
        renderer.color = Color.white;
        renderer.sprite = bulletScriptable.Bullets[(int)bulletType];
        if (changeSortingOrder) renderer.sortingOrder = SortingOrder;

        CalculateHitboxSize(projectile);
    }

    public void CalculateHitboxSize(DefaultProjectile projectile)
    {
        projectile.hitboxRadius = projectile.GetSpriteRenderer().sprite.bounds.extents.magnitude * projectile.transform.localScale.x / 3.8f;
    }

    public Vector3 ProjectileBaseScale => ProjectileObjectPooling._instance.ProjectileBaseScale;
    public DefaultProjectile CreateSimpleProjectile(BulletData.BulletType bulletType, float strength, float speed, Vector3 position, float scale = 1.0f)
    {
        GameObject projectile = ProjectileObjectPooling._instance.GetProjectile(bulletType, position, scale);

        DefaultProjectile defaultProjectile = projectile.GetComponent<DefaultProjectile>();
        defaultProjectile.SetProperties((int) bulletType, strength, speed);

        return defaultProjectile;
    }

    public DefaultProjectile CreateHomingProjectile(BulletData.BulletType bulletType, float strength, float speed, Transform target, Vector3 position)
    {
        if (!target) return null;
        Vector3 direction = (target.position - position).normalized;
        DefaultProjectile projectile = CreateSimpleProjectile(bulletType, strength, speed, position);
        projectile.SetDirection(direction);
        return projectile;
    }
}
