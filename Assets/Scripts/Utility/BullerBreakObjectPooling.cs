using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Singleton]
public class BulletBreakObjectPooling : MonoBehaviour
{
    public static BulletBreakObjectPooling _instance;

    [SerializeField] GameObject bulletBreakPref;
    [SerializeField] private int poolSize = 500;

    Queue<GameObject> poolQueue = new Queue<GameObject>();

    private void Awake()
    {
        if (!_instance)
        {
            _instance = this;
            for (int i = 0; i < poolSize; i++)
            {
                GameObject o = Instantiate(bulletBreakPref, transform);
                o.SetActive(false);
                poolQueue.Enqueue(o);
            }
        }
    }

    public GameObject GetEffect(Vector3 position)
    {
        GameObject effect;
        if (poolQueue.Count > 0)
        {
            effect = poolQueue.Dequeue();
        }
        else
        {
            effect = Instantiate(bulletBreakPref, transform);
        }

        effect.transform.position = position;
        effect.SetActive(true);
        return effect;
    }

    public void CreateExplosionAt(Vector3 position, float scale = 1.0f)
    {
        GameObject effect = GetEffect(position);
        effect.GetComponent<BulletBreakEffect>().CreateExplosion(scale);
    }

    public void ReturnToPool(BulletBreakEffect bulletBreakEffect)
    {
        if (!bulletBreakEffect.gameObject.activeSelf) return;
        bulletBreakEffect.gameObject.SetActive(false);
        poolQueue.Enqueue(bulletBreakEffect.gameObject);
    }
}