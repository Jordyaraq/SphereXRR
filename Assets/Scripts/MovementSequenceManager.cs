using UnityEngine;
using UnityEngine.InputSystem;
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

    private CoreState currentCore = CoreState.Celestial;

    private bool inputLocked = false;

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
        celestialRoot.SetActive(false);
        nebularRoot.SetActive(false);
        temporalRoot.SetActive(false);

        finalImage.SetActive(false);

        StartCoroutine(BeginExperience());
    }

    IEnumerator BeginExperience()
    {
        yield return new WaitForSeconds(1f);

        celestialRoot.SetActive(true);

        ResetCelestialArrows();
        ResetNebularArrows();
        ResetTemporalArrows();
    }

    void Update()
    {
        if (inputLocked || currentCore == CoreState.Finished)
            return;

        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
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
            if (celestialProgress == 0) arrow1.color = Color.green;
            if (celestialProgress == 1) arrow2.color = Color.green;
            if (celestialProgress == 2) arrow3.color = Color.green;

            celestialProgress++;

            if (celestialProgress >= celestialPattern.Length)
            {
                StartCoroutine(ActivateCelestial());
            }
        }
        else
        {
            celestialProgress = 0;
            StartCoroutine(ShowCelestialError());
        }
    }

    IEnumerator ActivateCelestial()
    {
        inputLocked = true;

        celestialRing.speed = 120f;
        celestialRenderer.material.color = Color.white;

        yield return StartCoroutine(celestialPulse.Pulse());

        yield return new WaitForSeconds(1f);

        celestialRoot.SetActive(false);

        nebularRoot.SetActive(true);

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
            if (nebularProgress == 0) nebularArrow1.color = Color.green;
            if (nebularProgress == 1) nebularArrow2.color = Color.green;
            if (nebularProgress == 2) nebularArrow3.color = Color.green;

            nebularProgress++;

            if (nebularProgress >= nebularPattern.Length)
            {
                StartCoroutine(ActivateNebular());
            }
        }
        else
        {
            nebularProgress = 0;
            StartCoroutine(ShowNebularError());
        }
    }

    IEnumerator ActivateNebular()
    {
        inputLocked = true;

        nebularRing.speed = 120f;
        nebularRenderer.material.color = Color.white;

        yield return StartCoroutine(nebularPulse.Pulse());

        yield return new WaitForSeconds(1f);

        nebularRoot.SetActive(false);

        temporalRoot.SetActive(true);

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
            if (temporalProgress == 0) temporalArrow1.color = Color.green;
            if (temporalProgress == 1) temporalArrow2.color = Color.green;
            if (temporalProgress == 2) temporalArrow3.color = Color.green;

            temporalProgress++;

            if (temporalProgress >= temporalPattern.Length)
            {
                StartCoroutine(ActivateTemporal());
            }
        }
        else
        {
            temporalProgress = 0;
            StartCoroutine(ShowTemporalError());
        }
    }

    IEnumerator ActivateTemporal()
    {
        inputLocked = true;

        temporalRing.speed = 120f;
        temporalRenderer.material.color = Color.white;

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

        yield return StartCoroutine(celestialPulse.Pulse());
        yield return StartCoroutine(nebularPulse.Pulse());
        yield return StartCoroutine(temporalPulse.Pulse());

        yield return new WaitForSeconds(2f);

        finalImage.SetActive(true);

        if (finalReveal != null)
        {
            yield return StartCoroutine(finalReveal.Reveal());
        }

        currentCore = CoreState.Finished;
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

        yield return new WaitForSeconds(1f);

        ResetCelestialArrows();

        inputLocked = false;
    }

    IEnumerator ShowNebularError()
    {
        inputLocked = true;

        nebularArrow1.color = Color.red;
        nebularArrow2.color = Color.red;
        nebularArrow3.color = Color.red;

        yield return new WaitForSeconds(1f);

        ResetNebularArrows();

        inputLocked = false;
    }

    IEnumerator ShowTemporalError()
    {
        inputLocked = true;

        temporalArrow1.color = Color.red;
        temporalArrow2.color = Color.red;
        temporalArrow3.color = Color.red;

        yield return new WaitForSeconds(1f);

        ResetTemporalArrows();

        inputLocked = false;
    }

    // =========================
    // RESETS
    // =========================

    void ResetCelestialArrows()
    {
        arrow1.color = Color.cyan;
        arrow2.color = Color.cyan;
        arrow3.color = Color.cyan;

        celestialProgress = 0;
    }

    void ResetNebularArrows()
    {
        nebularArrow1.color = Color.cyan;
        nebularArrow2.color = Color.cyan;
        nebularArrow3.color = Color.cyan;

        nebularProgress = 0;
    }

    void ResetTemporalArrows()
    {
        temporalArrow1.color = Color.cyan;
        temporalArrow2.color = Color.cyan;
        temporalArrow3.color = Color.cyan;

        temporalProgress = 0;
    }
}