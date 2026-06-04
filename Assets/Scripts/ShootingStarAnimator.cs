using UnityEngine;

public class ShootingStarAnimator : MonoBehaviour
{
    public Vector3 startPoint = new Vector3(-8f, 5.8f, 6.5f);
    public Vector3 endPoint = new Vector3(8f, 3.8f, 6.5f);
    public float minDelay = 7f;
    public float maxDelay = 14f;
    public float travelDuration = 1.35f;
    public AudioSource whooshSource;
    public AudioClip whooshClip;
    [Range(0f, 1f)]
    public float whooshVolume = 0.38f;

    private float timer;
    private float progress;
    private bool active;
    private LineRenderer line;
    private ParticleSystem particles;
    private Light sparkleLight;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        particles = GetComponent<ParticleSystem>();
        sparkleLight = GetComponent<Light>();
        ConfigureAudioSource();
        ScheduleNext();
        SetVisible(false);
    }

    void Update()
    {
        if (!active)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                active = true;
                progress = 0f;
                SetVisible(true);
                if (particles != null)
                {
                    particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    particles.Play();
                }

                PlayWhoosh();
            }
            return;
        }

        progress += Time.deltaTime / Mathf.Max(0.01f, travelDuration);
        float t = Mathf.Clamp01(progress);
        Vector3 position = Vector3.Lerp(startPoint, endPoint, t);
        Vector3 direction = (endPoint - startPoint).normalized;
        transform.position = position;

        if (line != null)
        {
            float fade = Mathf.Sin(t * Mathf.PI);
            line.enabled = true;
            line.widthMultiplier = Mathf.Lerp(0.03f, 0.11f, fade);
            line.SetPosition(0, position);
            line.SetPosition(1, position - direction * Mathf.Lerp(0.9f, 2.2f, fade));
        }

        if (sparkleLight != null)
        {
            sparkleLight.intensity = Mathf.Sin(t * Mathf.PI) * 1.4f;
        }

        if (t >= 1f)
        {
            active = false;
            SetVisible(false);
            ScheduleNext();
        }
    }

    private void SetVisible(bool visible)
    {
        if (line != null)
        {
            line.enabled = visible;
        }

        if (sparkleLight != null)
        {
            sparkleLight.enabled = visible;
        }
    }

    private void ScheduleNext()
    {
        timer = Random.Range(minDelay, maxDelay);
    }

    private void ConfigureAudioSource()
    {
        if (whooshSource == null)
        {
            whooshSource = GetComponent<AudioSource>();
        }

        if (whooshSource == null)
        {
            whooshSource = gameObject.AddComponent<AudioSource>();
        }

        whooshSource.playOnAwake = false;
        whooshSource.loop = false;
        whooshSource.volume = whooshVolume;
        whooshSource.spatialBlend = 0.85f;
        whooshSource.rolloffMode = AudioRolloffMode.Logarithmic;
        whooshSource.minDistance = 2f;
        whooshSource.maxDistance = 28f;
    }

    private void PlayWhoosh()
    {
        if (whooshClip == null)
            return;

        ConfigureAudioSource();
        whooshSource.volume = whooshVolume;
        whooshSource.PlayOneShot(whooshClip);
    }
}
