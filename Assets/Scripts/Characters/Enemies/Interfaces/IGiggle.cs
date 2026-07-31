using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeSpriteGiggle : MonoBehaviour
{
    [SerializeField] Vector3 MaxOffset;
    Vector3 initPos;

    [SerializeField] float duration = 1f;
    [SerializeField] Transform SpriteTransform;

    private void Awake()
    {
        if (!SpriteTransform) SpriteTransform = transform.Find("Sprite");
        initPos = SpriteTransform.localPosition;
        MakeGiggle();
    }

    protected void MakeGiggle()
    {
        StartCoroutine(Giggle());
    }

    IEnumerator Giggle()
    {
        float c = 0;
        Vector3 currentOffset;
        bool isMovingUp = true;

        while (true)
        {
            currentOffset = Vector3.Lerp(Vector3.zero, MaxOffset, c * 1.0f / duration);
            SpriteTransform.localPosition = initPos + currentOffset;

            if (isMovingUp) c += Time.deltaTime;
            else c -= Time.deltaTime;

            if (isMovingUp && c >= duration) isMovingUp = false;
            else if (!isMovingUp && c <= 0) isMovingUp = true;

            yield return null;
        }
    }
}
