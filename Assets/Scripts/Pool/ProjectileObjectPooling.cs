using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static BulletData;

[Singleton]
public class ProjectileObjectPooling : MonoBehaviour
{
    public static ProjectileObjectPooling _instance;
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
    public GameObject GetProjectile(BulletData.BulletType bulletType, Vector3 position, float scale = 1.0f)
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

        projectile.transform.localScale = ProjectileBaseScale * scale;
        projectile.transform.position = position;

        DefaultProjectile projectileScript = projectile.GetComponent<DefaultProjectile>();
        ProjectileManager._instance.SetSamplePropertiesToTargetProjectile(bulletType, projectileScript);
        
        projectile.SetActive(true);

        ProjectileManager._instance.Register(projectileScript);

        return projectile;
    }

    public void ReturnProjectile(DefaultProjectile projectile)
    {
        if (!projectile.gameObject.activeSelf) return;
        projectile.gameObject.SetActive(false);
        pools.Enqueue(projectile.gameObject);
        ProjectileManager._instance.Remove(projectile);
    }
}
