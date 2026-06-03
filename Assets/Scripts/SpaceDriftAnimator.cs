using UnityEngine;

public class SpaceDriftAnimator : MonoBehaviour
{
    public Vector3 orbitCenter = Vector3.zero;
    public Vector3 orbitAxis = Vector3.up;
    public float orbitRadius = 6f;
    public float verticalAmplitude = 0.5f;
    public float orbitSpeed = 8f;
    public float phase;
    public Vector3 spinAxis = Vector3.one;
    public float spinSpeed = 25f;

    private Vector3 radial;
    private Vector3 tangent;

    void Awake()
    {
        RecalculateBasis();
    }

    void OnValidate()
    {
        RecalculateBasis();
    }

    void Update()
    {
        if (radial == Vector3.zero || tangent == Vector3.zero)
        {
            RecalculateBasis();
        }

        float angle = phase + Time.time * orbitSpeed * Mathf.Deg2Rad;
        float vertical = Mathf.Sin(angle * 1.7f + phase) * verticalAmplitude;
        Vector3 position = orbitCenter;
        position += radial * Mathf.Cos(angle) * orbitRadius;
        position += tangent * Mathf.Sin(angle) * orbitRadius;
        position += orbitAxis.normalized * vertical;

        transform.position = position;
        transform.Rotate(spinAxis.normalized, spinSpeed * Time.deltaTime, Space.Self);
    }

    private void RecalculateBasis()
    {
        Vector3 axis = orbitAxis == Vector3.zero ? Vector3.up : orbitAxis.normalized;
        Vector3 seed = Vector3.Cross(axis, Vector3.forward);
        if (seed.sqrMagnitude < 0.001f)
        {
            seed = Vector3.Cross(axis, Vector3.right);
        }

        radial = seed.normalized;
        tangent = Vector3.Cross(axis, radial).normalized;
    }
}
