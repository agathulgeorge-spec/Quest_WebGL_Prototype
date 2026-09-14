using UnityEngine;
using TMPro;
using System.Collections;

public class NarrativeObject : MonoBehaviour
{
    [Header("Narrative")]
    [TextArea(2, 6)]
    public string[] narrativeLines;

    [Header("UI")]
    public GameObject narrativePanel;
    public TMP_Text narrativeText;

    [Header("Typing")]
    public float typingSpeed = 0.03f;

    [Header("Fade")]
    public float fadeSpeed = 2f;

    [Header("Trigger")]
    public Collider2D triggerCollider;

    private int currentLine = 0;

    private bool playerInside = false;
    private bool isShowing = false;
    private bool isTyping = false;

    private bool previousMobileE = false;

    private Coroutine typingCoroutine;
    private Coroutine fadeCoroutine;

    private CanvasGroup canvasGroup;


    private void Awake()
    {
        if (triggerCollider == null)
            triggerCollider = GetComponent<Collider2D>();


        if (narrativePanel != null)
        {
            canvasGroup =
                narrativePanel.GetComponent<CanvasGroup>();

            if (canvasGroup == null)
                canvasGroup =
                    narrativePanel.AddComponent<CanvasGroup>();

            canvasGroup.alpha = 0f;

            narrativePanel.SetActive(false);
        }
    }


    private void Update()
    {
        if (!playerInside || !isShowing)
            return;


        // =========================================
        // PC E
        // =========================================

        bool pcEPressed =
            Input.GetKeyDown(KeyCode.E);


        // =========================================
        // MOBILE E
        // Detect only the moment the button
        // becomes pressed.
        // =========================================

        bool mobileEPressed =
            MobileInput.eHeld &&
            !previousMobileE;


        previousMobileE =
            MobileInput.eHeld;


        // =========================================
        // E FROM EITHER PC OR MOBILE
        // =========================================

        if (pcEPressed || mobileEPressed)
        {
            ContinueNarrative();
        }
    }


    // =========================================================
    // CONTINUE NARRATIVE
    // =========================================================

    public void ContinueNarrative()
    {
        if (!playerInside || !isShowing)
            return;


        // =========================================
        // IF CURRENT LINE IS STILL TYPING
        // FINISH IT IMMEDIATELY
        // =========================================

        if (isTyping)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            narrativeText.text =
                narrativeLines[currentLine];

            isTyping = false;

            return;
        }


        // =========================================
        // CURRENT LINE FINISHED
        // MOVE TO NEXT LINE
        // =========================================

        currentLine++;


        if (currentLine < narrativeLines.Length)
        {
            typingCoroutine =
                StartCoroutine(
                    TypeLine(
                        narrativeLines[currentLine]
                    )
                );
        }
        else
        {
            // =====================================
            // FINAL E
            // CLOSE NARRATIVE
            // =====================================

            HideNarrative();
        }
    }


    // =========================================================
    // TRIGGER ENTER
    // =========================================================

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<HeroController>() == null)
            return;


        playerInside = true;

        ShowNarrative();
    }


    // =========================================================
    // TRIGGER EXIT
    // =========================================================

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<HeroController>() == null)
            return;


        playerInside = false;

        HideNarrative();
    }


    // =========================================================
    // SHOW
    // =========================================================

    private void ShowNarrative()
    {
        if (narrativeLines == null ||
            narrativeLines.Length == 0)
            return;


        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);


        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);


        currentLine = 0;

        isShowing = true;

        isTyping = false;


        // Reset mobile E state tracking
        previousMobileE =
            MobileInput.eHeld;


        narrativeText.text = "";

        narrativePanel.SetActive(true);


        fadeCoroutine =
            StartCoroutine(FadeIn());


        typingCoroutine =
            StartCoroutine(
                TypeLine(
                    narrativeLines[currentLine]
                )
            );
    }


    // =========================================================
    // TYPEWRITER
    // =========================================================

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;

        narrativeText.text = "";


        foreach (char letter in line)
        {
            narrativeText.text += letter;

            yield return new WaitForSeconds(
                typingSpeed
            );
        }


        isTyping = false;
    }


    // =========================================================
    // HIDE
    // =========================================================

    private void HideNarrative()
    {
        if (!isShowing)
            return;


        isShowing = false;

        isTyping = false;


        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);


        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);


        fadeCoroutine =
            StartCoroutine(FadeOut());
    }


    // =========================================================
    // FADE IN
    // =========================================================

    private IEnumerator FadeIn()
    {
        float startAlpha =
            canvasGroup.alpha;


        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha +=
                fadeSpeed * Time.deltaTime;

            yield return null;
        }


        canvasGroup.alpha = 1f;
    }


    // =========================================================
    // FADE OUT
    // =========================================================

    private IEnumerator FadeOut()
    {
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -=
                fadeSpeed * Time.deltaTime;

            yield return null;
        }


        canvasGroup.alpha = 0f;

        narrativePanel.SetActive(false);

        narrativeText.text = "";
    }
}