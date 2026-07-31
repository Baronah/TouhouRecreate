using UnityEngine;

public class ScrollingBackgroundLayer : MonoBehaviour
{
    public Vector2 scrollSpeed = new Vector2(0f, 0.05f); // UV units/sec
    public float rotateSpeed = 5f; // degrees/sec
    Material mat;

    void Start()
    {
        mat = GetComponent<Renderer>().material; // instance, not sharedMaterial
    }

    void Update()
    {
        mat.mainTextureOffset += scrollSpeed * Time.deltaTime;
        transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
    }
}