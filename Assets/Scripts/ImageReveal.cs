using UnityEngine;
using System.Collections;

public class ImageReveal : MonoBehaviour
{
    public float riseDistance = 3f;
    public float revealDuration = 3f;
    public float overshootScale = 1.08f;
    public float settleSpin = 7f;
    public AnimationCurve revealCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Vector3 startPosition;
    private Vector3 endPosition;
    private Vector3 endScale;
    private Quaternion startRotation;
    private Quaternion endRotation;
    private Renderer imageRenderer;
    private Light revealLight;
    private Material revealMaterial;

    void Awake()
    {
        endPosition = transform.position;
        startPosition = endPosition - Vector3.up * riseDistance;
        endScale = transform.localScale;
        endRotation = transform.rotation;
        startRotation = endRotation * Quaternion.Euler(0f, 0f, -settleSpin);
        imageRenderer = GetComponent<Renderer>();

        if (imageRenderer != null)
        {
            revealMaterial = imageRenderer.material;
            if (revealMaterial.HasProperty("_BaseColor"))
            {
                Color color = revealMaterial.GetColor("_BaseColor");
                color.a = 0f;
                revealMaterial.SetColor("_BaseColor", color);
            }
            else if (revealMaterial.HasProperty("_Color"))
            {
                Color color = revealMaterial.GetColor("_Color");
                color.a = 0f;
                revealMaterial.SetColor("_Color", color);
            }
        }

        revealLight = GetComponentInChildren<Light>(true);
        if (revealLight != null)
        {
            revealLight.intensity = 0f;
        }

        transform.position = startPosition;
        transform.rotation = startRotation;
        transform.localScale = endScale * 0.04f;
    }

    public IEnumerator Reveal()
    {
        float timer = 0f;

        while (timer < revealDuration)
        {
            float rawT = Mathf.Clamp01(timer / revealDuration);
            float t = revealCurve.Evaluate(rawT);
            float scalePunch = 1f + Mathf.Sin(t * Mathf.PI) * (overshootScale - 1f);

            transform.position = Vector3.Lerp(
                startPosition,
                endPosition,
                t
            );

            transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            transform.localScale = Vector3.Lerp(endScale * 0.04f, endScale, t) * scalePunch;

            SetAlpha(t);

            if (revealLight != null)
            {
                revealLight.intensity = Mathf.Sin(t * Mathf.PI) * 2.8f + t * 0.8f;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;
        transform.rotation = endRotation;
        transform.localScale = endScale;
        SetAlpha(1f);

        if (revealLight != null)
        {
            revealLight.intensity = 0.75f;
        }
    }

    private void SetAlpha(float alpha)
    {
        if (revealMaterial == null)
            return;

        if (revealMaterial.HasProperty("_BaseColor"))
        {
            Color color = revealMaterial.GetColor("_BaseColor");
            color.a = alpha;
            revealMaterial.SetColor("_BaseColor", color);
        }
        else if (revealMaterial.HasProperty("_Color"))
        {
            Color color = revealMaterial.GetColor("_Color");
            color.a = alpha;
            revealMaterial.SetColor("_Color", color);
        }
    }
}
