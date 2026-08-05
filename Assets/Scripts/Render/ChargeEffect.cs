using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[Singleton]
public class ChargeEffect : MonoBehaviour
{
    public static ChargeEffect _instance;
    [SerializeField] SpriteRenderer circle;
    SpriteRenderer[] renderers;

    private void Awake()
    {
        if (!_instance)
        {
            _instance = this;
            renderers = GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer renderer in renderers) renderer.gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] float MaxScale = 50f, MinScale = 20f;
    [SerializeField] float MaxOffset = 1000f, MinOffset = 550f;
    Color initColor;
    public void DoChargeEffect(Vector3 position, Color color, float time = 1.25f)
    {
        initColor = color;
        transform.position = position;
        foreach (var renderer in renderers)
        {
            renderer.color = initColor;

            if (renderer == circle) continue;

            renderer.gameObject.SetActive(true);
            float scale = Random.Range(MinScale, MaxScale);
            renderer.transform.localScale = new(scale, scale);

            Vector3 pos = new(
                Random.Range(MinOffset, MaxOffset) * Mathf.Pow(-1, Random.Range(0, 10) % 2 + 1), 
                Random.Range(MinOffset, MaxOffset) * Mathf.Pow(-1, Random.Range(0, 10) % 2 + 1));
            renderer.transform.localPosition = pos;
            renderer.transform.Rotate(0, 0, Random.Range(-360, 360));
        }

        StartCoroutine(PlayChargeEffect(time));
    }

    IEnumerator PlayChargeEffect(float time)
    {
        Vector3[] inits = new Vector3[renderers.Length];
        for (int i = 0; i < inits.Length; i++)
        {
            inits[i] = renderers[i].transform.localPosition;
        }

        Color clear = new(initColor.r, initColor.g, initColor.b, 0);
        float c = 0, d = time;
        while (c < d)
        {
            float t = c * 1.0f / d;
            for (int i = 0; i < inits.Length; ++i)
            {
                SpriteRenderer renderer = renderers[i];
                if (renderer == circle) continue;
                renderer.transform.localPosition = Vector3.Lerp(inits[i], Vector3.zero, t);
                renderer.color = Color.Lerp(initColor, clear, t);
            }

            c += Time.deltaTime;
            yield return null;
        }

        foreach (var renderer in renderers)
        {
            if (renderer == circle) continue;
            renderer.transform.localPosition = Vector3.zero;
            renderer.color = clear;
            renderer.gameObject.SetActive(false);
        }
    }

    public void DoCircleEffect(Vector3 position, Color color, float time = 1.25f)
    {
        initColor = color;
        transform.position = position;
        circle.color = initColor;

        circle.gameObject.SetActive(true);
        StartCoroutine(PlayCircleEffect(time));
    }

    IEnumerator PlayCircleEffect(float time)
    {
        Color clear = new(initColor.r, initColor.g, initColor.b, 0);
        circle.transform.localScale = Vector3.zero;
        circle.gameObject.SetActive(true);
        float c = 0;
        float d = time;
        Vector3 circleMaxScale = new(400, 400);
        while (c < d)
        {
            float t = c * 1.0f / d;
            circle.transform.localScale = Vector3.Lerp(Vector3.zero, circleMaxScale, t);
            circle.color = Color.Lerp(initColor, clear, t);
            c += Time.deltaTime;
            yield return null;
        }

        circle.gameObject.SetActive(false);
    }
}