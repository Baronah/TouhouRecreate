using UnityEngine;

public class TintedAdditiveLayer : MonoBehaviour
{
    public Color[] colorKeys;
    public float fadeInDuration = 1f;
    Material mat;
    float t;

    void Start() => mat = GetComponent<Renderer>().material;

    void Update()
    {
        t += Time.deltaTime;
        float alpha = Mathf.Clamp01(t / fadeInDuration);
        Color c = Color.Lerp(Color.black, colorKeys[0], alpha);
        mat.color = c;
    }
}