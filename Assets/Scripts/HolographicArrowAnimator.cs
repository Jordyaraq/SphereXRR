using TMPro;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class HolographicArrowAnimator : MonoBehaviour
{
    public float pressScale = 1.14f;
    public float successScale = 1.2f;
    public float errorShake = 12f;
    public Color idleGlow = new Color(0.15f, 1f, 1f, 1f);
    public Color idleColor = Color.cyan;
    public Color pressColor = new Color(0.45f, 1f, 0.8f, 1f);
    public Color successColor = new Color(0.35f, 1f, 0.35f, 1f);
    public Color errorColor = new Color(1f, 0.18f, 0.12f, 1f);

    private TMP_Text text;
    private RectTransform rectTransform;
    private Vector3 baseScale;
    private Vector3 basePosition;
    private Vector2 baseAnchoredPosition;
    private Material material;
    private Coroutine feedbackRoutine;

    void Awake()
    {
        text = GetComponent<TMP_Text>();
        rectTransform = GetComponent<RectTransform>();
        CaptureBaseState();
        material = text.fontMaterial;
        ApplyMaterialStyle();
    }

    void OnEnable()
    {
        if (text == null)
            text = GetComponent<TMP_Text>();

        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        CaptureBaseState();
    }

    void Update()
    {
        // Keep the original arrow placement and gameplay behavior intact.
        // This component only applies the visual TMP material polish.
    }

    public void SetIdle()
    {
        StopFeedback();
        transform.localScale = baseScale;
        RestoreBasePosition();
        SetVisualColor(idleColor, idleGlow, 0.55f);
    }

    public void CaptureBaseState()
    {
        baseScale = transform.localScale;
        basePosition = transform.localPosition;
        if (rectTransform != null)
        {
            baseAnchoredPosition = rectTransform.anchoredPosition;
        }
    }

    public void PlayPressed()
    {
        StartFeedback(PressedRoutine());
    }

    public IEnumerator PlaySuccess()
    {
        yield return RunFeedback(SuccessRoutine());
    }

    public IEnumerator PlayError()
    {
        yield return RunFeedback(ErrorRoutine());
    }

    private IEnumerator PressedRoutine()
    {
        yield return AnimateScaleAndColor(baseScale * pressScale, pressColor, 0.08f);
        yield return AnimateScaleAndColor(baseScale, successColor, 0.12f);
    }

    private IEnumerator SuccessRoutine()
    {
        yield return AnimateScaleAndColor(baseScale * successScale, successColor, 0.12f);
        yield return AnimateScaleAndColor(baseScale, successColor, 0.16f);
    }

    private IEnumerator ErrorRoutine()
    {
        SetVisualColor(errorColor, errorColor, 0.75f);

        float duration = 0.34f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float wave = Mathf.Sin(elapsed * 58f);
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = baseAnchoredPosition + Vector2.right * (wave * errorShake);
            }
            else
            {
                transform.localPosition = basePosition + Vector3.right * (wave * errorShake);
            }
            transform.localScale = Vector3.Lerp(baseScale * 1.05f, baseScale, elapsed / duration);
            yield return null;
        }

        RestoreBasePosition();
        transform.localScale = baseScale;
    }

    private void RestoreBasePosition()
    {
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = baseAnchoredPosition;
        }
        else
        {
            transform.localPosition = basePosition;
        }
    }

    private IEnumerator AnimateScaleAndColor(Vector3 targetScale, Color targetColor, float duration)
    {
        Vector3 startScale = transform.localScale;
        Color startColor = text != null ? text.color : targetColor;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            transform.localScale = Vector3.LerpUnclamped(startScale, targetScale, eased);
            SetVisualColor(Color.Lerp(startColor, targetColor, eased), targetColor, Mathf.Lerp(0.55f, 0.85f, eased));
            yield return null;
        }

        transform.localScale = targetScale;
        SetVisualColor(targetColor, targetColor, 0.7f);
    }

    private IEnumerator RunFeedback(IEnumerator routine)
    {
        StopFeedback();
        feedbackRoutine = StartCoroutine(routine);
        yield return feedbackRoutine;
        feedbackRoutine = null;
    }

    private void StartFeedback(IEnumerator routine)
    {
        StopFeedback();
        feedbackRoutine = StartCoroutine(routine);
    }

    private void StopFeedback()
    {
        if (feedbackRoutine != null)
        {
            StopCoroutine(feedbackRoutine);
            feedbackRoutine = null;
        }
    }

    private void ApplyMaterialStyle()
    {
        if (text == null)
            return;

        text.fontStyle |= FontStyles.Bold;
        text.outlineWidth = 0.22f;
        text.outlineColor = new Color(0f, 0.34f, 0.42f, 1f);

        material = text.fontMaterial;
        if (material == null)
            return;

        if (material.HasProperty(ShaderUtilities.ID_GlowColor))
        {
            material.EnableKeyword(ShaderUtilities.Keyword_Glow);
            material.SetColor(ShaderUtilities.ID_GlowColor, idleGlow);
            material.SetFloat(ShaderUtilities.ID_GlowPower, 0.55f);
            material.SetFloat(ShaderUtilities.ID_GlowOuter, 0.32f);
        }

        if (material.HasProperty(ShaderUtilities.ID_UnderlayColor))
        {
            material.EnableKeyword(ShaderUtilities.Keyword_Underlay);
            material.SetColor(ShaderUtilities.ID_UnderlayColor, new Color(0f, 0.5f, 0.7f, 0.65f));
            material.SetFloat(ShaderUtilities.ID_UnderlayDilate, 0.35f);
            material.SetFloat(ShaderUtilities.ID_UnderlaySoftness, 0.35f);
        }
    }

    private void SetVisualColor(Color color, Color glowColor, float glowPower)
    {
        if (text != null)
        {
            text.color = color;
        }

        if (material == null && text != null)
        {
            material = text.fontMaterial;
        }

        if (material != null && material.HasProperty(ShaderUtilities.ID_GlowColor))
        {
            material.EnableKeyword(ShaderUtilities.Keyword_Glow);
            material.SetColor(ShaderUtilities.ID_GlowColor, glowColor);
            material.SetFloat(ShaderUtilities.ID_GlowPower, glowPower);
        }
    }
}
