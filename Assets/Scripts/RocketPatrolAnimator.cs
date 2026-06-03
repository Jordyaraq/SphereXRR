using UnityEngine;

public class RocketPatrolAnimator : MonoBehaviour
{
    public Vector3 center = Vector3.zero;
    public Vector3 radius = new Vector3(5.5f, 1.1f, 3.8f);
    public Vector3 finalImageSafeCenter = new Vector3(0f, 0f, -2.9f);
    public Vector3 finalImageSafeRadius = new Vector3(6.6f, 1.2f, 1.35f);
    public Transform avoidWhenActive;
    public float speed = 0.18f;
    public float bankAmount = 22f;
    public float routeBlendSpeed = 1.5f;
    public Vector3 modelForwardAxis = Vector3.forward;
    public Vector3 modelUpAxis = Vector3.up;

    private Vector3 previousPosition;
    private Vector3 currentCenter;
    private Vector3 currentRadius;

    void Start()
    {
        currentCenter = center;
        currentRadius = radius;
        previousPosition = transform.position;
    }

    void Update()
    {
        bool shouldAvoid = avoidWhenActive != null && avoidWhenActive.gameObject.activeInHierarchy;
        Vector3 targetCenter = shouldAvoid ? finalImageSafeCenter : center;
        Vector3 targetRadius = shouldAvoid ? finalImageSafeRadius : radius;
        float blend = 1f - Mathf.Exp(-routeBlendSpeed * Time.deltaTime);
        currentCenter = Vector3.Lerp(currentCenter, targetCenter, blend);
        currentRadius = Vector3.Lerp(currentRadius, targetRadius, blend);

        float angle = Time.time * speed * Mathf.PI * 2f;
        Vector3 nextPosition = currentCenter + new Vector3(
            Mathf.Cos(angle) * currentRadius.x,
            Mathf.Sin(angle * 1.7f) * currentRadius.y + 1.4f,
            Mathf.Sin(angle) * currentRadius.z
        );

        Vector3 velocity = (nextPosition - previousPosition) / Mathf.Max(Time.deltaTime, 0.0001f);
        transform.position = nextPosition;

        if (velocity.sqrMagnitude > 0.0001f)
        {
            Quaternion pathRotation = Quaternion.LookRotation(velocity.normalized, Vector3.up);
            Quaternion modelCorrection = Quaternion.Inverse(Quaternion.LookRotation(modelForwardAxis.normalized, modelUpAxis.normalized));
            float bank = -Vector3.Dot(velocity.normalized, Vector3.right) * bankAmount;
            Quaternion bankRotation = Quaternion.AngleAxis(bank, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, pathRotation * bankRotation * modelCorrection, Time.deltaTime * 4f);
        }

        previousPosition = nextPosition;
    }
}
