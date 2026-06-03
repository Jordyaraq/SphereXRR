using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MovementSequenceManager : MonoBehaviour
{
    private enum CoreState
    {
        Celestial,
        Nebular,
        Temporal,
        Finished
    }

    [Header("Roots")]
    public GameObject celestialRoot;
    public GameObject nebularRoot;
    public GameObject temporalRoot;

    [Header("Final")]
    public GameObject finalImage;
    public ImageReveal finalReveal;

    [Header("Celestial")]
    public RotateObject celestialRing;
    public Renderer celestialRenderer;
    public PulseAnimation celestialPulse;

    public TMP_Text arrow1;
    public TMP_Text arrow2;
    public TMP_Text arrow3;

    [Header("Nebular")]
    public RotateObject nebularRing;
    public Renderer nebularRenderer;
    public PulseAnimation nebularPulse;

    public TMP_Text nebularArrow1;
    public TMP_Text nebularArrow2;
    public TMP_Text nebularArrow3;

    [Header("Temporal")]
    public RotateObject temporalRing;
    public Renderer temporalRenderer;
    public PulseAnimation temporalPulse;

    public TMP_Text temporalArrow1;
    public TMP_Text temporalArrow2;
    public TMP_Text temporalArrow3;

    [Header("HUD Feedback")]
    public TMP_Text instructionText;
    public TMP_Text inputText;
    public TMP_Text metricsText;
    public TMP_Text feedbackText;
    public TMP_Text restartText;
    public ParticleSystem successParticles;
    public ParticleSystem errorParticles;

    [Header("Audio Feedback")]
    public AudioSource feedbackAudioSource;
    public AudioClip moveSound;
    public AudioClip successSound;
    public AudioClip failSound;
    public AudioClip finalSound;
    public bool useSpatialAudio = true;

    [Header("Soundtrack")]
    public AudioSource soundtrackSource;
    public AudioClip soundtrackClip;
    [Range(0f, 1f)]
    public float soundtrackVolume = 0.32f;
    public bool playSoundtrackOnStart = true;

    private CoreState currentCore = CoreState.Celestial;

    private bool inputLocked = false;
    private Coroutine feedbackRoutine;
    private Coroutine cameraShakeRoutine;
    private Canvas sequenceHudCanvas;
    private string currentInputDisplay = "";
    private float metricsTimer = 0f;
    private int metricsFrameCount = 0;
    private float metricsFrameTime = 0f;

    private int celestialProgress = 0;
    private int nebularProgress = 0;
    private int temporalProgress = 0;

    private readonly string[] celestialPattern =
    {
        "UP",
        "DOWN",
        "UP"
    };

    private readonly string[] nebularPattern =
    {
        "LEFT",
        "RIGHT",
        "LEFT"
    };

    private readonly string[] temporalPattern =
    {
        "UP",
        "RIGHT",
        "DOWN"
    };

    void Start()
    {
        CacheFeedbackReferences();
        ConfigureArrowHud();
        ConfigureAudioSource();
        ConfigureSoundtrackSource();
        SetInstruction("Preparando secuencia");
        ClearInputDisplay();
        HideFeedback();
        HideRestartPrompt();

        celestialRoot.SetActive(false);
        nebularRoot.SetActive(false);
        temporalRoot.SetActive(false);

        finalImage.SetActive(false);

        StartCoroutine(BeginExperience());

        if (playSoundtrackOnStart)
        {
            PlaySoundtrack();
        }
    }

    IEnumerator BeginExperience()
    {
        yield return new WaitForSeconds(1f);

        celestialRoot.SetActive(true);
        ConfigureArrowHud();
        ShowOnlyCelestialArrows();
        SetInstruction("Repite la secuencia celestial");

        ResetCelestialArrows();
        ResetNebularArrows();
        ResetTemporalArrows();
    }

    void Update()
    {
        UpdateMetricsHud();

        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
            return;

        if (currentCore == CoreState.Finished)
        {
            if (keyboard.rKey.wasPressedThisFrame)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

            return;
        }

        if (inputLocked)
            return;

        if (keyboard.wKey.wasPressedThisFrame)
            ProcessInput("UP");

        if (keyboard.sKey.wasPressedThisFrame)
            ProcessInput("DOWN");

        if (keyboard.aKey.wasPressedThisFrame)
            ProcessInput("LEFT");

        if (keyboard.dKey.wasPressedThisFrame)
            ProcessInput("RIGHT");
    }

    public void SubmitExternalInput(string input)
    {
        if (inputLocked || currentCore == CoreState.Finished)
            return;

        string normalized = NormalizeDirectionInput(input);
        if (!string.IsNullOrEmpty(normalized))
        {
            ProcessInput(normalized);
        }
    }

    void ProcessInput(string input)
    {
        switch (currentCore)
        {
            case CoreState.Celestial:
                ProcessCelestialInput(input);
                break;

            case CoreState.Nebular:
                ProcessNebularInput(input);
                break;

            case CoreState.Temporal:
                ProcessTemporalInput(input);
                break;
        }
    }

    // =========================
    // CELESTIAL
    // =========================

    void ProcessCelestialInput(string input)
    {
        string expectedInput = celestialPattern[celestialProgress];

        if (input == expectedInput)
        {
            AppendInputDisplay(input);
            PlaySound(moveSound);
            MarkArrowCorrect(GetArrowByIndex(arrow1, arrow2, arrow3, celestialProgress));

            celestialProgress++;

            if (celestialProgress >= celestialPattern.Length)
            {
                StartCoroutine(ActivateCelestial());
            }
        }
        else
        {
            AppendInputDisplay(input);
            PlaySound(failSound);
            celestialProgress = 0;
            StartCoroutine(ShowCelestialError());
        }
    }

    IEnumerator ActivateCelestial()
    {
        inputLocked = true;

        celestialRing.speed = 120f;
        celestialRenderer.material.color = Color.white;

        yield return StartCoroutine(PlayArrowGroupSuccess(arrow1, arrow2, arrow3));
        PlaySound(successSound);
        PlayFeedback("Muy bien!", new Color(0.45f, 1f, 0.6f, 1f), successParticles);

        yield return StartCoroutine(celestialPulse.Pulse());

        yield return new WaitForSeconds(1f);

        celestialRoot.SetActive(false);

        nebularRoot.SetActive(true);
        ConfigureArrowHud();
        ShowOnlyNebularArrows();
        SetInstruction("Repite la secuencia nebular");

        currentCore = CoreState.Nebular;

        inputLocked = false;
    }

    // =========================
    // NEBULAR
    // =========================

    void ProcessNebularInput(string input)
    {
        string expectedInput = nebularPattern[nebularProgress];

        if (input == expectedInput)
        {
            AppendInputDisplay(input);
            PlaySound(moveSound);
            MarkArrowCorrect(GetArrowByIndex(nebularArrow1, nebularArrow2, nebularArrow3, nebularProgress));

            nebularProgress++;

            if (nebularProgress >= nebularPattern.Length)
            {
                StartCoroutine(ActivateNebular());
            }
        }
        else
        {
            AppendInputDisplay(input);
            PlaySound(failSound);
            nebularProgress = 0;
            StartCoroutine(ShowNebularError());
        }
    }

    IEnumerator ActivateNebular()
    {
        inputLocked = true;

        nebularRing.speed = 120f;
        nebularRenderer.material.color = Color.white;

        yield return StartCoroutine(PlayArrowGroupSuccess(nebularArrow1, nebularArrow2, nebularArrow3));
        PlaySound(successSound);
        PlayFeedback("Muy bien!", new Color(0.45f, 1f, 0.6f, 1f), successParticles);

        yield return StartCoroutine(nebularPulse.Pulse());

        yield return new WaitForSeconds(1f);

        nebularRoot.SetActive(false);

        temporalRoot.SetActive(true);
        ConfigureArrowHud();
        ShowOnlyTemporalArrows();
        SetInstruction("Repite la secuencia temporal");

        currentCore = CoreState.Temporal;

        inputLocked = false;
    }

    // =========================
    // TEMPORAL
    // =========================

    void ProcessTemporalInput(string input)
    {
        string expectedInput = temporalPattern[temporalProgress];

        if (input == expectedInput)
        {
            AppendInputDisplay(input);
            PlaySound(moveSound);
            MarkArrowCorrect(GetArrowByIndex(temporalArrow1, temporalArrow2, temporalArrow3, temporalProgress));

            temporalProgress++;

            if (temporalProgress >= temporalPattern.Length)
            {
                StartCoroutine(ActivateTemporal());
            }
        }
        else
        {
            AppendInputDisplay(input);
            PlaySound(failSound);
            temporalProgress = 0;
            StartCoroutine(ShowTemporalError());
        }
    }

    IEnumerator ActivateTemporal()
    {
        inputLocked = true;

        temporalRing.speed = 120f;
        temporalRenderer.material.color = Color.white;

        yield return StartCoroutine(PlayArrowGroupSuccess(temporalArrow1, temporalArrow2, temporalArrow3));
        PlaySound(successSound);
        PlayFeedback("Muy bien!", new Color(0.45f, 1f, 0.6f, 1f), successParticles);

        yield return StartCoroutine(temporalPulse.Pulse());

        yield return new WaitForSeconds(1f);

        temporalRoot.SetActive(false);

        StartCoroutine(FinalSequence());
    }

    // =========================
    // FINAL
    // =========================

    IEnumerator FinalSequence()
    {
        celestialRoot.SetActive(true);
        nebularRoot.SetActive(true);
        temporalRoot.SetActive(true);
        SetArrowGroupActive(arrow1, arrow2, arrow3, false);
        SetArrowGroupActive(nebularArrow1, nebularArrow2, nebularArrow3, false);
        SetArrowGroupActive(temporalArrow1, temporalArrow2, temporalArrow3, false);

        yield return StartCoroutine(celestialPulse.Pulse());
        yield return StartCoroutine(nebularPulse.Pulse());
        yield return StartCoroutine(temporalPulse.Pulse());

        yield return new WaitForSeconds(2f);

        finalImage.SetActive(true);
        SetInstruction("Experiencia completada");
        ClearInputDisplay();
        PlaySound(finalSound);

        if (finalReveal != null)
        {
            yield return StartCoroutine(finalReveal.Reveal());
        }

        currentCore = CoreState.Finished;
        ShowRestartPrompt();
    }

    // =========================
    // ERRORS
    // =========================

    IEnumerator ShowCelestialError()
    {
        inputLocked = true;

        arrow1.color = Color.red;
        arrow2.color = Color.red;
        arrow3.color = Color.red;

        ShakeCamera();
        PlayFeedback("Vuelve a intentarlo", new Color(1f, 0.32f, 0.22f, 1f), errorParticles);
        yield return StartCoroutine(PlayArrowGroupError(arrow1, arrow2, arrow3));
        yield return new WaitForSeconds(0.25f);

        ResetCelestialArrows();

        inputLocked = false;
    }

    IEnumerator ShowNebularError()
    {
        inputLocked = true;

        nebularArrow1.color = Color.red;
        nebularArrow2.color = Color.red;
        nebularArrow3.color = Color.red;

        ShakeCamera();
        PlayFeedback("Vuelve a intentarlo", new Color(1f, 0.32f, 0.22f, 1f), errorParticles);
        yield return StartCoroutine(PlayArrowGroupError(nebularArrow1, nebularArrow2, nebularArrow3));
        yield return new WaitForSeconds(0.25f);

        ResetNebularArrows();

        inputLocked = false;
    }

    IEnumerator ShowTemporalError()
    {
        inputLocked = true;

        temporalArrow1.color = Color.red;
        temporalArrow2.color = Color.red;
        temporalArrow3.color = Color.red;

        ShakeCamera();
        PlayFeedback("Vuelve a intentarlo", new Color(1f, 0.32f, 0.22f, 1f), errorParticles);
        yield return StartCoroutine(PlayArrowGroupError(temporalArrow1, temporalArrow2, temporalArrow3));
        yield return new WaitForSeconds(0.25f);

        ResetTemporalArrows();

        inputLocked = false;
    }

    // =========================
    // RESETS
    // =========================

    void ResetCelestialArrows()
    {
        ResetArrowVisual(arrow1);
        ResetArrowVisual(arrow2);
        ResetArrowVisual(arrow3);

        celestialProgress = 0;
        ClearInputDisplay();
    }

    void ResetNebularArrows()
    {
        ResetArrowVisual(nebularArrow1);
        ResetArrowVisual(nebularArrow2);
        ResetArrowVisual(nebularArrow3);

        nebularProgress = 0;
        ClearInputDisplay();
    }

    void ResetTemporalArrows()
    {
        ResetArrowVisual(temporalArrow1);
        ResetArrowVisual(temporalArrow2);
        ResetArrowVisual(temporalArrow3);

        temporalProgress = 0;
        ClearInputDisplay();
    }

    void ShowOnlyCelestialArrows()
    {
        ConfigureArrowGroup(arrow1, arrow2, arrow3, "UP", "DOWN", "UP");
        SetArrowGroupActive(arrow1, arrow2, arrow3, true);
        SetArrowGroupActive(nebularArrow1, nebularArrow2, nebularArrow3, false);
        SetArrowGroupActive(temporalArrow1, temporalArrow2, temporalArrow3, false);
    }

    void ShowOnlyNebularArrows()
    {
        ConfigureArrowGroup(nebularArrow1, nebularArrow2, nebularArrow3, "LEFT", "RIGHT", "LEFT");
        SetArrowGroupActive(arrow1, arrow2, arrow3, false);
        SetArrowGroupActive(nebularArrow1, nebularArrow2, nebularArrow3, true);
        SetArrowGroupActive(temporalArrow1, temporalArrow2, temporalArrow3, false);
    }

    void ShowOnlyTemporalArrows()
    {
        ConfigureArrowGroup(temporalArrow1, temporalArrow2, temporalArrow3, "UP", "RIGHT", "DOWN");
        SetArrowGroupActive(arrow1, arrow2, arrow3, false);
        SetArrowGroupActive(nebularArrow1, nebularArrow2, nebularArrow3, false);
        SetArrowGroupActive(temporalArrow1, temporalArrow2, temporalArrow3, true);
    }

    void SetArrowGroupActive(TMP_Text first, TMP_Text second, TMP_Text third, bool active)
    {
        SetArrowActive(first, active);
        SetArrowActive(second, active);
        SetArrowActive(third, active);
    }

    void SetArrowActive(TMP_Text arrow, bool active)
    {
        if (arrow == null)
            return;

        arrow.gameObject.SetActive(active);

        Transform slot = arrow.transform.parent != null ? arrow.transform.parent.Find(arrow.name + "_Slot") : null;
        if (slot != null)
        {
            slot.gameObject.SetActive(active);
        }
    }

    TMP_Text GetArrowByIndex(TMP_Text first, TMP_Text second, TMP_Text third, int index)
    {
        if (index == 0) return first;
        if (index == 1) return second;
        return third;
    }

    void MarkArrowCorrect(TMP_Text arrow)
    {
        if (arrow == null)
            return;

        arrow.color = Color.green;

        HolographicArrowAnimator feedback = arrow.GetComponent<HolographicArrowAnimator>();
        if (feedback != null)
        {
            feedback.PlayPressed();
        }
    }

    void ResetArrowVisual(TMP_Text arrow)
    {
        if (arrow == null)
            return;

        HolographicArrowAnimator feedback = arrow.GetComponent<HolographicArrowAnimator>();
        if (feedback != null)
        {
            feedback.SetIdle();
        }
        else
        {
            arrow.color = Color.cyan;
        }
    }

    IEnumerator PlayArrowGroupSuccess(TMP_Text first, TMP_Text second, TMP_Text third)
    {
        yield return StartCoroutine(PlayArrowSuccess(first));
        yield return new WaitForSeconds(0.04f);
        yield return StartCoroutine(PlayArrowSuccess(second));
        yield return new WaitForSeconds(0.04f);
        yield return StartCoroutine(PlayArrowSuccess(third));
    }

    IEnumerator PlayArrowSuccess(TMP_Text arrow)
    {
        if (arrow == null)
            yield break;

        HolographicArrowAnimator feedback = arrow.GetComponent<HolographicArrowAnimator>();
        if (feedback != null)
        {
            yield return StartCoroutine(feedback.PlaySuccess());
        }
        else
        {
            arrow.color = Color.green;
            yield return new WaitForSeconds(0.16f);
        }
    }

    IEnumerator PlayArrowGroupError(TMP_Text first, TMP_Text second, TMP_Text third)
    {
        StartCoroutine(PlayArrowError(first));
        StartCoroutine(PlayArrowError(second));
        StartCoroutine(PlayArrowError(third));
        yield return new WaitForSeconds(0.36f);
    }

    IEnumerator PlayArrowError(TMP_Text arrow)
    {
        if (arrow == null)
            yield break;

        HolographicArrowAnimator feedback = arrow.GetComponent<HolographicArrowAnimator>();
        if (feedback != null)
        {
            yield return StartCoroutine(feedback.PlayError());
        }
        else
        {
            arrow.color = Color.red;
            yield return new WaitForSeconds(0.34f);
        }
    }

    void ConfigureArrowHud()
    {
        EnsureSequenceHudCanvas();
        AttachArrowToHud(arrow1);
        AttachArrowToHud(arrow2);
        AttachArrowToHud(arrow3);
        AttachArrowToHud(nebularArrow1);
        AttachArrowToHud(nebularArrow2);
        AttachArrowToHud(nebularArrow3);
        AttachArrowToHud(temporalArrow1);
        AttachArrowToHud(temporalArrow2);
        AttachArrowToHud(temporalArrow3);
        EnsureInstructionText();
        EnsureInputText();
        EnsureMetricsText();
        EnsureFeedbackText();
        EnsureRestartText();

        ConfigureArrowGroup(arrow1, arrow2, arrow3, "UP", "DOWN", "UP");
        ConfigureArrowGroup(nebularArrow1, nebularArrow2, nebularArrow3, "LEFT", "RIGHT", "LEFT");
        ConfigureArrowGroup(temporalArrow1, temporalArrow2, temporalArrow3, "UP", "RIGHT", "DOWN");
    }

    void ConfigureArrowGroup(TMP_Text first, TMP_Text second, TMP_Text third, string firstDirection, string secondDirection, string thirdDirection)
    {
        ConfigureArrow(first, firstDirection, new Vector2(-72f, 74f));
        ConfigureArrow(second, secondDirection, new Vector2(0f, 74f));
        ConfigureArrow(third, thirdDirection, new Vector2(72f, 74f));
    }

    void ConfigureArrow(TMP_Text arrow, string direction, Vector2 anchoredPosition)
    {
        if (arrow == null)
            return;

        arrow.text = DirectionToSymbol(direction);
        arrow.alignment = TextAlignmentOptions.Center;
        arrow.fontSize = 34f;
        arrow.textWrappingMode = TextWrappingModes.NoWrap;
        arrow.overflowMode = TextOverflowModes.Overflow;
        arrow.raycastTarget = false;
        arrow.enabled = true;
        arrow.gameObject.layer = 5;

        RectTransform rect = arrow.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(76f, 76f);
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;
        }

        Transform slot = EnsureArrowSlot(arrow);
        RectTransform slotRect = slot != null ? slot.GetComponent<RectTransform>() : null;
        if (slotRect != null)
        {
            slotRect.anchorMin = new Vector2(0.5f, 0f);
            slotRect.anchorMax = new Vector2(0.5f, 0f);
            slotRect.pivot = new Vector2(0.5f, 0.5f);
            slotRect.anchoredPosition = anchoredPosition;
            slotRect.sizeDelta = new Vector2(52f, 50f);
            slotRect.localRotation = Quaternion.identity;
            slotRect.localScale = Vector3.one;
        }

        HolographicArrowAnimator feedback = arrow.GetComponent<HolographicArrowAnimator>();
        if (feedback != null)
        {
            feedback.CaptureBaseState();
        }
    }

    string DirectionToSymbol(string direction)
    {
        if (direction == "UP") return "\u2191";
        if (direction == "DOWN") return "\u2193";
        if (direction == "LEFT") return "\u2190";
        if (direction == "RIGHT") return "\u2192";
        return "?";
    }

    void EnsureSequenceHudCanvas()
    {
        if (sequenceHudCanvas != null)
            return;

        GameObject canvasObject = GameObject.Find("VX_Sequence_HUD_Canvas");
        if (canvasObject == null)
        {
            canvasObject = new GameObject("VX_Sequence_HUD_Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        }

        sequenceHudCanvas = canvasObject.GetComponent<Canvas>();
        sequenceHudCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        sequenceHudCanvas.sortingOrder = 500;
        sequenceHudCanvas.overrideSorting = true;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1024f, 576f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        RectTransform rect = canvasObject.GetComponent<RectTransform>();
        rect.localPosition = Vector3.zero;
        rect.localRotation = Quaternion.identity;
        rect.localScale = Vector3.one;
    }

    void AttachArrowToHud(TMP_Text arrow)
    {
        if (arrow == null || sequenceHudCanvas == null)
            return;

        Transform oldParent = arrow.transform.parent;
        Transform oldSlot = oldParent != null ? oldParent.Find(arrow.name + "_Slot") : null;
        if (oldSlot != null && oldSlot.parent != sequenceHudCanvas.transform)
        {
            oldSlot.SetParent(sequenceHudCanvas.transform, false);
        }

        if (arrow.transform.parent != sequenceHudCanvas.transform)
        {
            arrow.transform.SetParent(sequenceHudCanvas.transform, false);
        }
    }

    Transform EnsureArrowSlot(TMP_Text arrow)
    {
        if (arrow == null || sequenceHudCanvas == null)
            return null;

        string slotName = arrow.name + "_Slot";
        Transform slot = sequenceHudCanvas.transform.Find(slotName);
        if (slot == null)
        {
            GameObject slotObject = new GameObject(slotName, typeof(RectTransform), typeof(Image), typeof(Outline));
            slotObject.transform.SetParent(sequenceHudCanvas.transform, false);
            slot = slotObject.transform;
        }

        Image image = slot.GetComponent<Image>();
        if (image != null)
        {
            image.color = new Color(0.02f, 0.12f, 0.18f, 0.42f);
            image.raycastTarget = false;
        }

        Outline outline = slot.GetComponent<Outline>();
        if (outline != null)
        {
            outline.effectColor = new Color(0.2f, 1f, 1f, 0.42f);
            outline.effectDistance = new Vector2(1.2f, -1.2f);
        }

        slot.SetSiblingIndex(Mathf.Max(0, arrow.transform.GetSiblingIndex()));
        arrow.transform.SetSiblingIndex(slot.GetSiblingIndex() + 1);
        return slot;
    }

    void EnsureFeedbackText()
    {
        if (sequenceHudCanvas == null)
            return;

        if (feedbackText == null)
        {
            Transform existing = sequenceHudCanvas.transform.Find("VX_Sequence_Feedback_Text");
            if (existing != null)
            {
                feedbackText = existing.GetComponent<TMP_Text>();
            }
        }

        if (feedbackText == null)
        {
            GameObject feedbackObject = new GameObject("VX_Sequence_Feedback_Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            feedbackObject.transform.SetParent(sequenceHudCanvas.transform, false);
            feedbackText = feedbackObject.GetComponent<TMP_Text>();
        }

        if (feedbackText.transform.parent != sequenceHudCanvas.transform)
        {
            feedbackText.transform.SetParent(sequenceHudCanvas.transform, false);
        }

        RectTransform rect = feedbackText.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(0f, 150f);
            rect.sizeDelta = new Vector2(420f, 74f);
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;
        }

        feedbackText.alignment = TextAlignmentOptions.Center;
        feedbackText.fontSize = 36f;
        feedbackText.fontStyle = FontStyles.Bold;
        feedbackText.outlineWidth = 0.16f;
        feedbackText.outlineColor = new Color(0f, 0.18f, 0.2f, 1f);
        feedbackText.textWrappingMode = TextWrappingModes.NoWrap;
        feedbackText.raycastTarget = false;
        feedbackText.gameObject.layer = 5;
    }

    void EnsureRestartText()
    {
        if (sequenceHudCanvas == null)
            return;

        if (restartText == null)
        {
            Transform existing = sequenceHudCanvas.transform.Find("VX_Restart_Prompt_Text");
            if (existing != null)
            {
                restartText = existing.GetComponent<TMP_Text>();
            }
        }

        if (restartText == null)
        {
            GameObject restartObject = new GameObject("VX_Restart_Prompt_Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            restartObject.transform.SetParent(sequenceHudCanvas.transform, false);
            restartText = restartObject.GetComponent<TMP_Text>();
        }

        if (restartText.transform.parent != sequenceHudCanvas.transform)
        {
            restartText.transform.SetParent(sequenceHudCanvas.transform, false);
        }

        RectTransform rect = restartText.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(0f, 82f);
            rect.sizeDelta = new Vector2(520f, 62f);
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;
        }

        restartText.text = "Pulsa R para repetir";
        restartText.alignment = TextAlignmentOptions.Center;
        restartText.fontSize = 28f;
        restartText.fontStyle = FontStyles.Bold;
        restartText.color = new Color(0.78f, 1f, 1f, 1f);
        restartText.outlineWidth = 0.13f;
        restartText.outlineColor = new Color(0f, 0.16f, 0.24f, 1f);
        restartText.textWrappingMode = TextWrappingModes.NoWrap;
        restartText.raycastTarget = false;
        restartText.gameObject.layer = 5;
    }

    void EnsureInstructionText()
    {
        if (sequenceHudCanvas == null)
            return;

        if (instructionText == null)
        {
            Transform existing = sequenceHudCanvas.transform.Find("VX_Instruction_Text");
            if (existing != null)
            {
                instructionText = existing.GetComponent<TMP_Text>();
            }
        }

        if (instructionText == null)
        {
            GameObject instructionObject = new GameObject("VX_Instruction_Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            instructionObject.transform.SetParent(sequenceHudCanvas.transform, false);
            instructionText = instructionObject.GetComponent<TMP_Text>();
        }

        if (instructionText.transform.parent != sequenceHudCanvas.transform)
        {
            instructionText.transform.SetParent(sequenceHudCanvas.transform, false);
        }

        RectTransform rect = instructionText.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(0f, 214f);
            rect.sizeDelta = new Vector2(620f, 48f);
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;
        }

        instructionText.alignment = TextAlignmentOptions.Center;
        instructionText.fontSize = 24f;
        instructionText.fontStyle = FontStyles.Bold;
        instructionText.color = new Color(0.82f, 1f, 1f, 0.94f);
        instructionText.outlineWidth = 0.1f;
        instructionText.outlineColor = new Color(0f, 0.12f, 0.18f, 1f);
        instructionText.textWrappingMode = TextWrappingModes.NoWrap;
        instructionText.raycastTarget = false;
        instructionText.gameObject.layer = 5;
    }

    void EnsureInputText()
    {
        if (sequenceHudCanvas == null)
            return;

        if (inputText == null)
        {
            Transform existing = sequenceHudCanvas.transform.Find("VX_Input_Text");
            if (existing != null)
            {
                inputText = existing.GetComponent<TMP_Text>();
            }
        }

        if (inputText == null)
        {
            GameObject inputObject = new GameObject("VX_Input_Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            inputObject.transform.SetParent(sequenceHudCanvas.transform, false);
            inputText = inputObject.GetComponent<TMP_Text>();
        }

        if (inputText.transform.parent != sequenceHudCanvas.transform)
        {
            inputText.transform.SetParent(sequenceHudCanvas.transform, false);
        }

        RectTransform rect = inputText.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(0f, 28f);
            rect.sizeDelta = new Vector2(320f, 38f);
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;
        }

        inputText.alignment = TextAlignmentOptions.Center;
        inputText.fontSize = 20f;
        inputText.fontStyle = FontStyles.Bold;
        inputText.color = new Color(0.55f, 1f, 0.92f, 0.9f);
        inputText.outlineWidth = 0.08f;
        inputText.outlineColor = new Color(0f, 0.1f, 0.14f, 1f);
        inputText.textWrappingMode = TextWrappingModes.NoWrap;
        inputText.raycastTarget = false;
        inputText.gameObject.layer = 5;
    }

    void EnsureMetricsText()
    {
        if (sequenceHudCanvas == null)
            return;

        if (metricsText == null)
        {
            Transform existing = sequenceHudCanvas.transform.Find("VX_Metrics_Text");
            if (existing != null)
            {
                metricsText = existing.GetComponent<TMP_Text>();
            }
        }

        if (metricsText == null)
        {
            GameObject metricsObject = new GameObject("VX_Metrics_Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            metricsObject.transform.SetParent(sequenceHudCanvas.transform, false);
            metricsText = metricsObject.GetComponent<TMP_Text>();
        }

        if (metricsText.transform.parent != sequenceHudCanvas.transform)
        {
            metricsText.transform.SetParent(sequenceHudCanvas.transform, false);
        }

        RectTransform rect = metricsText.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.anchoredPosition = new Vector2(-18f, -14f);
            rect.sizeDelta = new Vector2(260f, 34f);
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;
        }

        metricsText.alignment = TextAlignmentOptions.Right;
        metricsText.fontSize = 14f;
        metricsText.fontStyle = FontStyles.Bold;
        metricsText.color = new Color(0.7f, 1f, 0.86f, 0.62f);
        metricsText.outlineWidth = 0.04f;
        metricsText.outlineColor = new Color(0f, 0.08f, 0.1f, 1f);
        metricsText.textWrappingMode = TextWrappingModes.NoWrap;
        metricsText.raycastTarget = false;
        metricsText.gameObject.layer = 5;
    }

    void CacheFeedbackReferences()
    {
        if (instructionText == null)
        {
            GameObject textObject = FindSceneObject("VX_Instruction_Text");
            if (textObject != null)
            {
                instructionText = textObject.GetComponent<TMP_Text>();
            }
        }

        if (inputText == null)
        {
            GameObject textObject = FindSceneObject("VX_Input_Text");
            if (textObject != null)
            {
                inputText = textObject.GetComponent<TMP_Text>();
            }
        }

        if (metricsText == null)
        {
            GameObject textObject = FindSceneObject("VX_Metrics_Text");
            if (textObject != null)
            {
                metricsText = textObject.GetComponent<TMP_Text>();
            }
        }

        if (feedbackText == null)
        {
            GameObject textObject = FindSceneObject("VX_Sequence_Feedback_Text");
            if (textObject != null)
            {
                feedbackText = textObject.GetComponent<TMP_Text>();
            }
        }

        if (restartText == null)
        {
            GameObject textObject = FindSceneObject("VX_Restart_Prompt_Text");
            if (textObject != null)
            {
                restartText = textObject.GetComponent<TMP_Text>();
            }
        }

        if (successParticles == null)
        {
            GameObject particlesObject = FindSceneObject("VX_Sequence_Success_Particles");
            if (particlesObject != null)
            {
                successParticles = particlesObject.GetComponent<ParticleSystem>();
            }
        }

        if (errorParticles == null)
        {
            GameObject particlesObject = FindSceneObject("VX_Sequence_Error_Particles");
            if (particlesObject != null)
            {
                errorParticles = particlesObject.GetComponent<ParticleSystem>();
            }
        }
    }

    void PlayFeedback(string message, Color color, ParticleSystem particles)
    {
        if (particles != null)
        {
            particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particles.Play();
        }

        if (feedbackText == null)
            return;

        feedbackText.gameObject.SetActive(true);
        feedbackText.text = message;
        feedbackText.color = color;
        RectTransform feedbackRect = feedbackText.GetComponent<RectTransform>();
        if (feedbackRect != null)
        {
            feedbackRect.localScale = Vector3.one;
        }

        if (feedbackRoutine != null)
        {
            StopCoroutine(feedbackRoutine);
        }

        feedbackRoutine = StartCoroutine(FeedbackRoutine(message, color));
    }

    IEnumerator FeedbackRoutine(string message, Color color)
    {
        feedbackText.gameObject.SetActive(true);
        feedbackText.text = message;
        feedbackText.color = color;

        RectTransform rect = feedbackText.GetComponent<RectTransform>();
        Vector3 baseScale = Vector3.one;
        if (rect != null)
        {
            rect.localScale = baseScale * 0.88f;
        }

        float duration = 3.0f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float pop = Mathf.Sin(t * Mathf.PI);
            Color animated = color;
            animated.a = Mathf.Lerp(1f, 0f, Mathf.Clamp01((t - 0.68f) / 0.32f));
            feedbackText.color = animated;

            if (rect != null)
            {
                rect.localScale = baseScale * (1f + pop * 0.08f);
            }

            yield return null;
        }

        HideFeedback();
    }

    void HideFeedback()
    {
        if (feedbackText != null)
        {
            feedbackText.gameObject.SetActive(false);
        }
    }

    void ShowRestartPrompt()
    {
        EnsureRestartText();
        if (restartText != null)
        {
            restartText.gameObject.SetActive(true);
        }
    }

    void HideRestartPrompt()
    {
        if (restartText != null)
        {
            restartText.gameObject.SetActive(false);
        }
    }

    void SetInstruction(string message)
    {
        EnsureInstructionText();
        if (instructionText != null)
        {
            instructionText.text = message;
        }
    }

    void AppendInputDisplay(string input)
    {
        EnsureInputText();
        if (inputText == null)
            return;

        string symbol = DirectionToSymbol(input);
        if (currentInputDisplay.Length > 0)
        {
            currentInputDisplay += "  ";
        }

        currentInputDisplay += symbol;
        inputText.text = currentInputDisplay;
    }

    void ClearInputDisplay()
    {
        currentInputDisplay = "";
        EnsureInputText();
        if (inputText != null)
        {
            inputText.text = "";
        }
    }

    string NormalizeDirectionInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "";

        string normalized = input.Trim().ToUpperInvariant();
        if (normalized == "UP" || normalized == "ARRIBA" || normalized == "W")
            return "UP";

        if (normalized == "DOWN" || normalized == "ABAJO" || normalized == "S")
            return "DOWN";

        if (normalized == "LEFT" || normalized == "IZQUIERDA" || normalized == "A")
            return "LEFT";

        if (normalized == "RIGHT" || normalized == "DERECHA" || normalized == "D")
            return "RIGHT";

        return "";
    }

    void UpdateMetricsHud()
    {
        metricsFrameCount++;
        metricsFrameTime += Time.unscaledDeltaTime;
        metricsTimer += Time.unscaledDeltaTime;

        if (metricsTimer < 0.5f)
            return;

        EnsureMetricsText();
        if (metricsText != null && metricsFrameTime > 0f)
        {
            float fps = metricsFrameCount / metricsFrameTime;
            float frameMs = (metricsFrameTime / metricsFrameCount) * 1000f;
            metricsText.text = Mathf.RoundToInt(fps) + " FPS | " + frameMs.ToString("0.0") + " ms";
        }

        metricsTimer = 0f;
        metricsFrameCount = 0;
        metricsFrameTime = 0f;
    }

    void ConfigureAudioSource()
    {
        if (feedbackAudioSource == null)
        {
            feedbackAudioSource = GetComponent<AudioSource>();
        }

        if (feedbackAudioSource == null)
        {
            feedbackAudioSource = gameObject.AddComponent<AudioSource>();
        }

        feedbackAudioSource.playOnAwake = false;
        feedbackAudioSource.loop = false;
        feedbackAudioSource.volume = 0.55f;
        feedbackAudioSource.spatialBlend = useSpatialAudio ? 0.75f : 0f;
        feedbackAudioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        feedbackAudioSource.minDistance = 1.5f;
        feedbackAudioSource.maxDistance = 18f;
    }

    void ConfigureSoundtrackSource()
    {
        if (soundtrackSource == null)
        {
            AudioSource[] sources = GetComponents<AudioSource>();
            for (int i = 0; i < sources.Length; i++)
            {
                if (sources[i] != feedbackAudioSource)
                {
                    soundtrackSource = sources[i];
                    break;
                }
            }
        }

        if (soundtrackSource == null)
        {
            soundtrackSource = gameObject.AddComponent<AudioSource>();
        }

        soundtrackSource.playOnAwake = false;
        soundtrackSource.loop = true;
        soundtrackSource.volume = soundtrackVolume;
        soundtrackSource.spatialBlend = 0f;
        soundtrackSource.rolloffMode = AudioRolloffMode.Logarithmic;
        soundtrackSource.clip = soundtrackClip;
    }

    public void PlaySoundtrack()
    {
        ConfigureSoundtrackSource();
        if (soundtrackClip == null || soundtrackSource == null)
            return;

        soundtrackSource.clip = soundtrackClip;
        soundtrackSource.volume = soundtrackVolume;

        if (!soundtrackSource.isPlaying)
        {
            soundtrackSource.Play();
        }
    }

    public void StopSoundtrack()
    {
        if (soundtrackSource != null)
        {
            soundtrackSource.Stop();
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (clip == null)
            return;

        ConfigureAudioSource();
        feedbackAudioSource.transform.position = GetActiveAudioPosition();
        feedbackAudioSource.PlayOneShot(clip);
    }

    Vector3 GetActiveAudioPosition()
    {
        if (currentCore == CoreState.Celestial && celestialRoot != null)
            return celestialRoot.transform.position;

        if (currentCore == CoreState.Nebular && nebularRoot != null)
            return nebularRoot.transform.position;

        if (currentCore == CoreState.Temporal && temporalRoot != null)
            return temporalRoot.transform.position;

        Camera camera = Camera.main;
        if (camera != null)
            return camera.transform.position;

        return transform.position;
    }

    void ShakeCamera()
    {
        Camera camera = Camera.main;
        if (camera == null)
            return;

        if (cameraShakeRoutine != null)
        {
            StopCoroutine(cameraShakeRoutine);
        }

        cameraShakeRoutine = StartCoroutine(CameraShakeRoutine(camera.transform));
    }

    IEnumerator CameraShakeRoutine(Transform cameraTransform)
    {
        Vector3 baseLocalPosition = cameraTransform.localPosition;
        float duration = 0.32f;
        float elapsed = 0f;
        float strength = 0.075f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float fade = 1f - Mathf.Clamp01(elapsed / duration);
            float waveX = Mathf.Sin(elapsed * 74f) * strength * fade;
            float waveY = Mathf.Cos(elapsed * 59f) * strength * 0.6f * fade;
            cameraTransform.localPosition = baseLocalPosition + new Vector3(waveX, waveY, 0f);
            yield return null;
        }

        cameraTransform.localPosition = baseLocalPosition;
        cameraShakeRoutine = null;
    }

    GameObject FindSceneObject(string objectName)
    {
        GameObject[] objects = Resources.FindObjectsOfTypeAll<GameObject>();
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] == null || objects[i].name != objectName)
                continue;

            if (!objects[i].scene.IsValid())
                continue;

            return objects[i];
        }

        return null;
    }
}
