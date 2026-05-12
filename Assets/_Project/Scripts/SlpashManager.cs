using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SlpashManager : MonoBehaviour
{
    // ── Arrastra desde el Inspector ──────────────────
    public Image logoImage;
    public TextMeshProUGUI titleText;

    // ── Tiempos ajustables ───────────────────────────
    public float fadeInDuration = 1.2f;
    public float slideDuration = 0.8f;
    public float textFadeDuration = 1.0f;
    public float waitBeforeLoad = 3.0f;
    public string nextSceneName = "MainMenu";

    // ── Posiciones destino del logo ──────────────────
    public Vector2 logoSlideTarget = new Vector2(-300f, 0f);

    private bool skipRequested = false;

    void Start()
    {

        StartCoroutine(PlaySplash());
    }

    void Update()
    {
        // Cualquier clic o tecla salta la secuencia
        if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
            skipRequested = true;
    }

    IEnumerator PlaySplash()
    {
        yield return new WaitForSeconds(0.2f);
        // Estado inicial
        RectTransform logoRT = logoImage.rectTransform;
        RectTransform textRT = titleText.rectTransform;
        Vector2 logoCenterPos = Vector2.zero;
        Vector2 textStartPos = new Vector2(600f, 0f);
        Vector2 textFinalPos = Vector2.zero;

        logoRT.anchoredPosition = logoCenterPos;
        logoRT.localScale = new Vector3(1.15f, 1.15f, 1f);
        textRT.anchoredPosition = textStartPos;
        textRT.localScale = new Vector3(1.1f, 1.1f, 1f);
        SetAlpha(logoImage, 0f);
        SetTextAlpha(titleText, 0f);

        // ── FASE 1: Fade-in + zoom-in del logo ──────────
        yield return StartCoroutine(
            FadeAndScale(logoImage, logoRT, 0f, 1f,
                         new Vector3(1.15f, 1.15f, 1),
                         new Vector3(3f, 3f, 1f), fadeInDuration));

        // ── FASE 2: Logo se desplaza a la izquierda ─────
        // + simultáneamente empieza el texto
        StartCoroutine(SlideRect(logoRT, logoCenterPos,
                                   logoSlideTarget, slideDuration));
        StartCoroutine(
            FadeTextAndScale(titleText, textRT, 0f, 1f,
                             new Vector3(1.1f, 1.1f, 1),
                             Vector3.one, textFadeDuration,
                             textStartPos, textFinalPos));

        yield return new WaitForSeconds(textFadeDuration);

        // ── ESPERA: 3 segundos o clic ───────────────────
        float elapsed = 0f;
        while (elapsed < waitBeforeLoad && !skipRequested)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        // ── SALIDA: Fade out todo ───────────────────────
        yield return StartCoroutine(FadeOutAll(0.5f));
        SceneManager.LoadScene(nextSceneName);
    }

    // ── Helpers ──────────────────────────────────────

    IEnumerator FadeAndScale(Image img, RectTransform rt,
        float a0, float a1, Vector3 s0, Vector3 s1, float dur)
    {
        for (float t = 0; t < dur; t += Time.deltaTime)
        {
            float n = EaseOut(t / dur);
            SetAlpha(img, Mathf.Lerp(a0, a1, n));
            rt.localScale = Vector3.Lerp(s0, s1, n);
            yield return null;
        }
        SetAlpha(img, a1); rt.localScale = s1;
    }

    IEnumerator FadeTextAndScale(TextMeshProUGUI txt, RectTransform rt,
        float a0, float a1, Vector3 s0, Vector3 s1, float dur,
        Vector2 p0, Vector2 p1)
    {
        for (float t = 0; t < dur; t += Time.deltaTime)
        {
            float n = EaseOut(t / dur);
            SetTextAlpha(txt, Mathf.Lerp(a0, a1, n));
            rt.localScale = Vector3.Lerp(s0, s1, n);
            rt.anchoredPosition = Vector2.Lerp(p0, p1, n);
            yield return null;
        }
        SetTextAlpha(txt, a1); rt.localScale = s1; rt.anchoredPosition = p1;
    }

    IEnumerator SlideRect(RectTransform rt,
        Vector2 from, Vector2 to, float dur)
    {
        for (float t = 0; t < dur; t += Time.deltaTime)
        {
            rt.anchoredPosition = Vector2.Lerp(from, to, EaseInOut(t / dur));
            yield return null;
        }
        rt.anchoredPosition = to;
    }

    IEnumerator FadeOutAll(float dur)
    {
        CanvasGroup cg = GetComponentInParent<CanvasGroup>()
                      ?? gameObject.AddComponent<CanvasGroup>();
        for (float t = 0; t < dur; t += Time.deltaTime)
        {
            cg.alpha = Mathf.Lerp(1f, 0f, t / dur);
            yield return null;
        }
        cg.alpha = 0f;
    }

    void SetAlpha(Image img, float a)
    {
        Color c = img.color; c.a = a; img.color = c;
    }

    void SetTextAlpha(TextMeshProUGUI t, float a)
    {
        Color c = t.color; c.a = a; t.color = c;
    }

    float EaseOut(float t) =>
        1f - Mathf.Pow(1f - t, 3f);

    float EaseInOut(float t) =>
        t < .5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
}