using UnityEngine;
using System.Collections;

public class ImageReveal : MonoBehaviour
{
    public float riseDistance = 3f;
    public float revealDuration = 3f;

    private Vector3 startPosition;
    private Vector3 endPosition;

    void Awake()
    {
        endPosition = transform.position;
        startPosition = endPosition - Vector3.up * riseDistance;

        transform.position = startPosition;
        transform.localScale = Vector3.zero;
    }

    public IEnumerator Reveal()
    {
        float timer = 0f;

        while (timer < revealDuration)
        {
            float t = timer / revealDuration;

            transform.position = Vector3.Lerp(
                startPosition,
                endPosition,
                t
            );

            transform.localScale = Vector3.Lerp(
                Vector3.zero,
                Vector3.one,
                t
            );

            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;
        transform.localScale = Vector3.one;
    }
}