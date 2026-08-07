using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static BulletData;

[Singleton]
public class PlayerProjectileObjectPooling : MonoBehaviour
{
    public static PlayerProjectileObjectPooling _instance;
    [SerializeField] private int initSize = 50;
    [SerializeField] private int expandSize = 50;
    [SerializeField] GameObject ProjectilePrefab;

    // Start is called before the first frame update
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            for (int i = 0; i < initSize; i++)
            {
                GameObject projectile = Instantiate(ProjectilePrefab, transform);
                projectile.SetActive(false);
                pools.Enqueue(projectile);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    readonly Queue<GameObject> pools = new Queue<GameObject>();

    public Vector3 ProjectileBaseScale => ProjectilePrefab.transform.localScale;
    public GameObject GetProjectile(BulletData.PlayerBulletType bulletType, Vector3 position, float scale = 1.0f)
    {
        GameObject projectile;

        if (pools.Count > 0)
        {
            projectile = pools.Dequeue();
        }
        else
        {
            projectile = Instantiate(ProjectilePrefab, transform);
            for (int i = 0; i < expandSize; i++)
            {
                GameObject projectileEx = Instantiate(ProjectilePrefab, transform);
                projectileEx.SetActive(false);
                pools.Enqueue(projectileEx);
            }
        }

        PlayerProjectile playerProjectile = projectile.GetComponent<PlayerProjectile>();
        PlayerProjectileManager._instance.SetSamplePropertiesToTargetProjectile(bulletType, playerProjectile);

        projectile.transform.localScale = ProjectileBaseScale * scale;
        projectile.transform.position = position;

        projectile.SetActive(true);

        PlayerProjectileManager._instance.Register(playerProjectile);

        return projectile;
    }

    public void ReturnProjectile(DefaultProjectile projectile)
    {
        if (!projectile.gameObject.activeSelf) return;
        projectile.gameObject.SetActive(false);
        pools.Enqueue(projectile.gameObject);
    }
}
