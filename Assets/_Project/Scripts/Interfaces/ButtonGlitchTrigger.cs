using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Button))]
public class ButtonGlitchTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Glitch")]
    [SerializeField] private TitleGlitchEffect titleGlitchEffect;

    [Header("Hover Scale")]
    [SerializeField] private RectTransform scaleTarget;
    [SerializeField] private bool useHoverScale = true;
    [SerializeField, Min(1f)] private float hoverScaleMultiplier = 1.1f;

    private Vector3 originalScale;
    private bool hasCachedScale;

    private void Reset()
    {
        AutoAssignReferences();
    }

    private void Awake()
    {
        AutoAssignReferences();
        CacheOriginalScale();
        DisableGlitchEffect();
        RestoreScale();
    }

    private void OnEnable()
    {
        DisableGlitchEffect();
        RestoreScale();
    }

    private void OnDisable()
    {
        DisableGlitchEffect();
        RestoreScale();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EnableGlitchEffect();
        ApplyHoverScale();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DisableGlitchEffect();
        RestoreScale();
    }

    private void AutoAssignReferences()
    {
        if (titleGlitchEffect == null)
        {
            titleGlitchEffect = GetComponent<TitleGlitchEffect>();
        }

        if (titleGlitchEffect == null)
        {
            titleGlitchEffect = GetComponentInChildren<TitleGlitchEffect>(true);
        }

        if (scaleTarget == null)
        {
            scaleTarget = transform as RectTransform;
        }
    }

    private void CacheOriginalScale()
    {
        if (scaleTarget == null)
        {
            return;
        }

        originalScale = scaleTarget.localScale;
        hasCachedScale = true;
    }

    private void EnableGlitchEffect()
    {
        if (titleGlitchEffect != null && !titleGlitchEffect.enabled)
        {
            titleGlitchEffect.enabled = true;
        }
    }

    private void DisableGlitchEffect()
    {
        if (titleGlitchEffect != null && titleGlitchEffect.enabled)
        {
            titleGlitchEffect.enabled = false;
        }
    }

    private void ApplyHoverScale()
    {
        if (!useHoverScale || scaleTarget == null)
        {
            return;
        }

        if (!hasCachedScale)
        {
            CacheOriginalScale();
        }

        scaleTarget.localScale = originalScale * hoverScaleMultiplier;
    }

    private void RestoreScale()
    {
        if (!hasCachedScale || scaleTarget == null)
        {
            return;
        }

        scaleTarget.localScale = originalScale;
    }
}
