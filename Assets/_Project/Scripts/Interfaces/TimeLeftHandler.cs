using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimeLeftHandler : MonoBehaviour
{
    public Image cooldownImage;
    private Coroutine cooldownCoroutine;

    public void StartCooldown(float duration)
    {
        if (cooldownCoroutine != null)
            StopCoroutine(cooldownCoroutine);

        cooldownCoroutine = StartCoroutine(CooldownRoutine(duration));
    }

    private IEnumerator CooldownRoutine(float duration)
    {
        float elapsed = 0f;
        cooldownImage.fillAmount = 1f;

        while (elapsed < duration)
        {   
            elapsed += Time.deltaTime;
            cooldownImage.fillAmount = 1 - (elapsed / duration);
            yield return null;
        }

        cooldownImage.fillAmount = 0f;
    }
    public void MakeInvisible()
    {
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
            cooldownCoroutine = null;
        }

        cooldownImage.fillAmount = 0f;

    }
}