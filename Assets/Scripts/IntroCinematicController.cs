using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

public class IntroCinematicController : MonoBehaviour
{
    private static bool introPlayedThisSession = false;

    public VideoClip introClip;
    public VideoPlayer videoPlayer;
    public AudioSource audioSource;
    public RawImage videoImage;
    public Canvas introCanvas;
    public MovementSequenceManager sequenceManager;
    public Color backgroundColor = Color.black;
    public bool startGameSoundtrackAfterIntro = true;
    public float prepareTimeout = 5f;
    [Range(0f, 1f)]
    public float videoVolume = 1f;
    public AudioClip externalIntroAudio;
    public float externalAudioDelay = 0.12f;
    public bool finishIntroIfVideoStalls = true;
    public float stallTimeout = 3f;

    private RenderTexture runtimeTexture;
    private bool isFinishing;
    private Coroutine externalAudioRoutine;
    private double lastVideoTime;
    private float stallTimer;

    void Awake()
    {
        if (introPlayedThisSession)
        {
            HideIntro();
            return;
        }

        if (sequenceManager == null)
        {
            sequenceManager = FindAnyObjectByType<MovementSequenceManager>(FindObjectsInactive.Include);
        }

        if (sequenceManager != null)
        {
            sequenceManager.StopSoundtrack();
            sequenceManager.enabled = false;
        }

        ConfigureVideo();
    }

    void Start()
    {
        if (introPlayedThisSession)
            return;

        if (introClip == null || videoPlayer == null)
        {
            FinishIntro();
            return;
        }

        StartCoroutine(PlayIntroWhenReady());
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= HandleVideoFinished;
        }

        if (runtimeTexture != null)
        {
            runtimeTexture.Release();
            Destroy(runtimeTexture);
        }
    }

    private void ConfigureVideo()
    {
        if (introCanvas != null)
        {
            introCanvas.gameObject.SetActive(true);
            introCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            introCanvas.sortingOrder = 2000;
            introCanvas.overrideSorting = true;
        }

        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        if (videoPlayer == null)
        {
            videoPlayer = gameObject.AddComponent<VideoPlayer>();
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        runtimeTexture = new RenderTexture(1920, 1080, 0, RenderTextureFormat.ARGB32);
        runtimeTexture.name = "Intro_Cinematic_Runtime_Texture";
        runtimeTexture.Create();

        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;
        videoPlayer.waitForFirstFrame = true;
        videoPlayer.skipOnDrop = false;
        videoPlayer.timeReference = VideoTimeReference.InternalTime;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = runtimeTexture;
        bool useExternalAudio = externalIntroAudio != null;
        videoPlayer.audioOutputMode = useExternalAudio ? VideoAudioOutputMode.None : VideoAudioOutputMode.Direct;
        videoPlayer.controlledAudioTrackCount = (ushort)(useExternalAudio ? 0 : 1);
        if (!useExternalAudio)
        {
            videoPlayer.EnableAudioTrack(0, true);
            videoPlayer.SetDirectAudioVolume(0, videoVolume);
        }
        videoPlayer.clip = introClip;
        videoPlayer.loopPointReached -= HandleVideoFinished;
        videoPlayer.loopPointReached += HandleVideoFinished;

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
        audioSource.volume = externalIntroAudio == null ? 0f : videoVolume;
        audioSource.clip = externalIntroAudio;

        if (videoImage != null)
        {
            videoImage.texture = runtimeTexture;
            videoImage.color = Color.white;
            videoImage.raycastTarget = true;
        }
    }

    void Update()
    {
        if (introPlayedThisSession || isFinishing || videoPlayer == null || !videoPlayer.isPlaying || !finishIntroIfVideoStalls)
            return;

        if (Mathf.Abs((float)(videoPlayer.time - lastVideoTime)) < 0.01f)
        {
            stallTimer += Time.unscaledDeltaTime;
            if (stallTimer >= stallTimeout)
            {
                FinishIntro();
            }
        }
        else
        {
            lastVideoTime = videoPlayer.time;
            stallTimer = 0f;
        }
    }

    private IEnumerator PlayIntroWhenReady()
    {
        ShowIntro();

        videoPlayer.Prepare();
        float elapsed = 0f;
        while (!videoPlayer.isPrepared && elapsed < prepareTimeout)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        if (!videoPlayer.isPrepared)
        {
            FinishIntro();
            yield break;
        }

        videoPlayer.Play();
        lastVideoTime = videoPlayer.time;
        stallTimer = 0f;

        if (externalIntroAudio != null)
        {
            if (externalAudioRoutine != null)
            {
                StopCoroutine(externalAudioRoutine);
            }

            externalAudioRoutine = StartCoroutine(PlayExternalIntroAudio());
        }
    }

    private IEnumerator PlayExternalIntroAudio()
    {
        if (audioSource == null || externalIntroAudio == null)
            yield break;

        audioSource.Stop();
        audioSource.clip = externalIntroAudio;
        audioSource.volume = videoVolume;

        if (externalAudioDelay > 0f)
        {
            yield return new WaitForSecondsRealtime(externalAudioDelay);
        }

        if (!isFinishing)
        {
            audioSource.Play();
        }
    }

    private void HandleVideoFinished(VideoPlayer player)
    {
        FinishIntro();
    }

    private void FinishIntro()
    {
        if (isFinishing)
            return;

        isFinishing = true;
        introPlayedThisSession = true;

        if (videoPlayer != null)
        {
            videoPlayer.Stop();
        }

        if (externalAudioRoutine != null)
        {
            StopCoroutine(externalAudioRoutine);
            externalAudioRoutine = null;
        }

        if (audioSource != null)
        {
            audioSource.Stop();
        }

        HideIntro();

        if (sequenceManager != null)
        {
            sequenceManager.enabled = true;

            if (startGameSoundtrackAfterIntro)
            {
                sequenceManager.PlaySoundtrack();
            }
        }
    }

    private void ShowIntro()
    {
        if (introCanvas != null)
        {
            introCanvas.gameObject.SetActive(true);
        }

        if (videoImage != null)
        {
            videoImage.gameObject.SetActive(true);
        }
    }

    private void HideIntro()
    {
        if (introCanvas != null)
        {
            introCanvas.gameObject.SetActive(false);
        }
    }
}
