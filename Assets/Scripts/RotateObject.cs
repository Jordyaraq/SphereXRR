using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public Vector3 rotationAxis = new Vector3(0, 1, 0);
    public float speed = 30f;

    void Update()
    {
        transform.Rotate(rotationAxis * speed * Time.deltaTime);
    }
}