using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBreakEffect : MonoBehaviour
{
    [SerializeField] Vector3 baseScale;
    [SerializeField] float expandTime = 0.25f, colorChangeTime = 0.3f, colorFadeTime = 0.2f;
    [SerializeField] bool PlayOnStart = false;
    [SerializeField] bool ManagedByPool = true;
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        if (PlayOnStart) CreateExplosion();
    }

    public void CreateExplosion(float scale = 1.0f)
    {
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(EnableCoroutine(scale));
    }

    IEnumerator EnableCoroutine(float scale)
    {
        StartCoroutine(Expand(scale));
        StartCoroutine(ChangeColor());
        yield return new WaitForSeconds(expandTime + colorFadeTime + colorFadeTime + 0.15f);

        if (ManagedByPool) BulletBreakObjectPooling._instance.ReturnToPool(this);
    }

    IEnumerator Expand(float scale)
    {
        transform.localScale = Vector3.zero;
        Vector3 finalScale = scale * baseScale;
        float c = 0, d = expandTime;
        while (c < d)
        {
            float t = c * 1.0f / d;
            transform.localScale = Vector3.Lerp(Vector3.zero, finalScale, t);
            c += Time.deltaTime;
            yield return null;
        }
    }

    static Color clear = new(1, 1, 1, 0);
    IEnumerator ChangeColor()
    {
        Color final = Color.white;
        spriteRenderer.color = clear;
        float c = 0, d = colorChangeTime;
        while (c < d)
        {
            float t = c * 1.0f / d;
            spriteRenderer.color = Color.Lerp(clear, final, t);
            c += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = final;

        c = 0;
        d = colorFadeTime;
        Color color = spriteRenderer.color;
        while (c < d)
        {
            float t = c * 1.0f / d;
            spriteRenderer.color = Color.Lerp(color, clear, t);
            c += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = clear;
    }
}
