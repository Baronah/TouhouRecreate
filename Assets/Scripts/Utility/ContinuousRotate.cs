using UnityEngine;

public class ContinuousRotate : MonoBehaviour
{
    [SerializeField] Vector3 degreePerSecond = new(0, 0, 90f);

    private void Update()
    {
        transform.Rotate(degreePerSecond * Time.deltaTime);
    }
}