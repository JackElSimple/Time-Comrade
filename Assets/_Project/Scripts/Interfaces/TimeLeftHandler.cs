using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimeLeftHandler : MonoBehaviour
{
    public Image cooldownImage;
    private bool invisible;

    public void StartCooldown(float duration)
    {
        invisible = false;
        StartCoroutine(CooldownRoutine(duration));
    }

    private IEnumerator CooldownRoutine(float duration)
    {
        float elapsed = 0f;
        cooldownImage.fillAmount = 1f;

        while (elapsed < duration)
        {   
            if(invisible) {
                elapsed += Time.deltaTime;
                cooldownImage.fillAmount = 0f;
                yield return null;
            }
            else { 
                elapsed += Time.deltaTime;
                cooldownImage.fillAmount = 1 - (elapsed / duration);
                yield return null;
            }
        }

        cooldownImage.fillAmount = 0f;
    }
    public void MakeInvisible()
    {
        invisible = true;
      
    }
}