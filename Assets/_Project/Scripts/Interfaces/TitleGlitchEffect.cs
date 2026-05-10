using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TitleGlitchEffect : MonoBehaviour
{
    private const string FallbackTitle = "TIME COMRADE";
    private const string GlitchSymbols = "!@#01&";
    private const float MinIntervalMultiplier = 1.5f;
    private const float MaxIntervalMultiplier = 6f;

    [Range(0f, 1f)]
    public float glitchChance = 0.35f;
    public float jitterIntensity = 12f;
    public float glitchDuration = 0.08f;

    private TextMeshProUGUI titleText;
    private RectTransform rectTransform;
    private Vector2 originalAnchoredPosition;
    private Color32 baseVertexColor;
    private char[] originalChars;
    private char[] workingChars;
    private Coroutine glitchRoutine;
    private readonly WaitForSecondsRealtime loopWaitInstruction = new WaitForSecondsRealtime(0f);
    private readonly WaitForSecondsRealtime glitchHoldInstruction = new WaitForSecondsRealtime(0f);

    private void Awake()
    {
        titleText = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();

        string originalTitle = string.IsNullOrWhiteSpace(titleText.text) ? FallbackTitle : titleText.text;
        titleText.text = originalTitle;

        originalChars = originalTitle.ToCharArray();
        workingChars = new char[originalChars.Length];
        originalAnchoredPosition = rectTransform.anchoredPosition;
        baseVertexColor = titleText.color;

        RestoreTitleState();
    }

    private void OnEnable()
    {
        if (glitchRoutine == null)
        {
            glitchRoutine = StartCoroutine(GlitchLoop());
        }
    }

    private void OnDisable()
    {
        if (glitchRoutine != null)
        {
            StopCoroutine(glitchRoutine);
            glitchRoutine = null;
        }

        RestoreTitleState();
    }

    private IEnumerator GlitchLoop()
    {
        while (true)
        {
            float waitTime = Random.Range(
                Mathf.Max(0.05f, glitchDuration * MinIntervalMultiplier),
                Mathf.Max(0.1f, glitchDuration * MaxIntervalMultiplier));

            loopWaitInstruction.waitTime = waitTime;
            yield return loopWaitInstruction;

            if (Random.value <= glitchChance)
            {
                yield return RunSingleGlitch();
            }
        }
    }

    private IEnumerator RunSingleGlitch()
    {
        ApplyGlitchedText();
        ApplyVertexColor(GetRandomNeonColor());

        Vector2 offset = Random.insideUnitCircle * jitterIntensity;
        rectTransform.anchoredPosition = originalAnchoredPosition + offset;

        float holdDuration = glitchDuration * 0.65f;
        if (holdDuration > 0f)
        {
            glitchHoldInstruction.waitTime = holdDuration;
            yield return glitchHoldInstruction;
        }

        float returnDuration = Mathf.Max(0.01f, glitchDuration - holdDuration);
        Vector2 startPosition = rectTransform.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < returnDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / returnDuration);
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, originalAnchoredPosition, t);
            yield return null;
        }

        RestoreTitleState();
    }

    private void ApplyGlitchedText()
    {
        bool changedAnyCharacter = false;

        for (int i = 0; i < originalChars.Length; i++)
        {
            char currentChar = originalChars[i];
            workingChars[i] = currentChar;

            if (char.IsWhiteSpace(currentChar))
            {
                continue;
            }

            if (Random.value <= 0.25f)
            {
                workingChars[i] = GlitchSymbols[Random.Range(0, GlitchSymbols.Length)];
                changedAnyCharacter = true;
            }
        }

        if (!changedAnyCharacter)
        {
            for (int i = 0; i < workingChars.Length; i++)
            {
                if (char.IsWhiteSpace(workingChars[i]))
                {
                    continue;
                }

                workingChars[i] = GlitchSymbols[Random.Range(0, GlitchSymbols.Length)];
                break;
            }
        }

        titleText.SetCharArray(workingChars);
    }

    private void RestoreTitleState()
    {
        rectTransform.anchoredPosition = originalAnchoredPosition;
        titleText.SetCharArray(originalChars);
        ApplyVertexColor(baseVertexColor);
    }

    private void ApplyVertexColor(Color32 color)
    {
        titleText.ForceMeshUpdate();
        TMP_TextInfo textInfo = titleText.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo characterInfo = textInfo.characterInfo[i];
            if (!characterInfo.isVisible)
            {
                continue;
            }

            int materialIndex = characterInfo.materialReferenceIndex;
            int vertexIndex = characterInfo.vertexIndex;
            Color32[] colors = textInfo.meshInfo[materialIndex].colors32;

            colors[vertexIndex] = color;
            colors[vertexIndex + 1] = color;
            colors[vertexIndex + 2] = color;
            colors[vertexIndex + 3] = color;
        }

        titleText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    private static Color32 GetRandomNeonColor()
    {
        switch (Random.Range(0, 3))
        {
            case 0:
                return new Color32(255, 48, 48, 255);
            case 1:
                return new Color32(0, 255, 255, 255);
            default:
                return new Color32(255, 0, 255, 255);
        }
    }
}
