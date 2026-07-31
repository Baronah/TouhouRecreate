using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static BulletData;

[Singleton]
public class ObjectPooling : MonoBehaviour
{
    public static ObjectPooling _instance;
    [SerializeField] private int initSize = 50;
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

    Vector3 baseScale = new(70f, 70f);
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
        }

        projectile.transform.localScale = baseScale * scale;
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
