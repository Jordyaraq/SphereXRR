using UnityEngine;

public class FloatingHologram : MonoBehaviour
{
    private Vector3 startPos;

    public float amplitude = 0.1f;
    public float speed = 2f;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        transform.localPosition =
            startPos +
            Vector3.up * Mathf.Sin(Time.time * speed) * amplitude;
    }
}