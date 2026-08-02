using System.Collections;
using UnityEngine;

public class TintedAdditiveLayer : MonoBehaviour
{
    public Color[] colorKeys;
    public float fadeInDuration = 1f, holdTime;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(ColorsFade());
    }

    IEnumerator ColorsFade()
    {
        int loopCount = 0;
        while (true)
        {
            yield return new WaitForSeconds(holdTime);

            float t = 0;
            Color startColor = spriteRenderer.color,
                  targetColor = colorKeys[loopCount % colorKeys.Length];
            while (t < fadeInDuration)
            {
                t += Time.deltaTime;
                float alpha = Mathf.Clamp01(t / fadeInDuration);
                spriteRenderer.color = Color.Lerp(startColor, targetColor, alpha);
                yield return null;
            }
            spriteRenderer.color = targetColor;

            loopCount++;
        }
    }
}