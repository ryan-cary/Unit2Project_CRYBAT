using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro; // Use UnityEngine.UI if using legacy Text

public class ButtonHoverBlink : MonoBehaviour, IPointerEnterHandler
{
    public TMP_Text buttonText; // Change to 'Text' if using legacy UI Text
    private Coroutine blinkRoutine;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
        }
        blinkRoutine = StartCoroutine(BlinkEffect());
    }

    private IEnumerator BlinkEffect()
    {
        // 3 quick blinks over ~0.38 seconds
        for (int i = 0; i < 3; i++)
        {
            buttonText.enabled = false;
            yield return new WaitForSeconds(0.05f);
            buttonText.enabled = true;
            yield return new WaitForSeconds(0.05f);
        }
        blinkRoutine = null;
    }
}