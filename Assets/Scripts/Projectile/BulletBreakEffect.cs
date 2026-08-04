using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBreakEffect : MonoBehaviour
{
    [SerializeField] Vector3 baseScale;
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void CreateExplosion(float scale = 1.0f)
    {
        Start();
        StartCoroutine(EnableCoroutine(scale));
    }

    IEnumerator EnableCoroutine(float scale)
    {
        StartCoroutine(Expand(scale));
        StartCoroutine(ChangeColor());
        yield return new WaitForSeconds(1.5f);

        BulletBreakObjectPooling._instance.ReturnToPool(this);
    }

    IEnumerator Expand(float scale)
    {
        transform.localScale = Vector3.zero;
        Vector3 finalScale = scale * baseScale;
        float c = 0, d = 0.25f;
        while (c < d)
        {
            float t = c * 1.0f / d;
            transform.localScale = Vector3.Lerp(transform.localScale, finalScale, t);
            c += Time.deltaTime;
            yield return null;
        }
    }

    static Color clear = new(1, 1, 1, 0);
    IEnumerator ChangeColor()
    {
        Color final = Color.white;
        spriteRenderer.color = clear;
        float c = 0, d = 0.25f;
        while (c < d)
        {
            float t = c * 1.0f / d;
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, final, t);
            c += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = final;

        c = 0;
        d = 0.15f;
        while (c < d)
        {
            float t = c * 1.0f / d;
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, clear, t);
            c += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = clear;
    }
}
