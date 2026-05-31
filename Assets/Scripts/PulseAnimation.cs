using UnityEngine;
using System.Collections;

public class PulseAnimation : MonoBehaviour
{
    public IEnumerator Pulse()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 1.3f;

        float duration = 0.3f;
        float timer = 0;

        while (timer < duration)
        {
            transform.localScale =
                Vector3.Lerp(originalScale, targetScale, timer / duration);

            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0;

        while (timer < duration)
        {
            transform.localScale =
                Vector3.Lerp(targetScale, originalScale, timer / duration);

            timer += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
    }
}