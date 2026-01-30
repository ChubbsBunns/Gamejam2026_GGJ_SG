using System.Collections;
using UnityEngine;

public class DialogueUIScreenShakeManager : MonoBehaviour
{
    public RectTransform defaultNPCSubtitleText;
    public float rectTransformScreenShakeIntensity;
    public float defaultScreenShakeDuration;
    
    public void ShakeDefaultNPCSubtitleText()
    {
        ShakeUI(defaultNPCSubtitleText);
    }

    public void ShakeUI(RectTransform rect, float duration = 0.4f)
    {
        StartCoroutine(ShakeRectTransform(rect, rectTransformScreenShakeIntensity, duration));
    }

    public IEnumerator ShakeRectTransform(
        RectTransform rectTransform,
        float intensity,
        float duration=0.4f,
        float frequency = 60f
    )
    {
        if (rectTransform == null)
            yield break;

        Vector2 originalPos = rectTransform.anchoredPosition;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float damper = 1f - Mathf.Clamp01(elapsed / duration);
            Vector2 offset = UnityEngine.Random.insideUnitCircle * intensity * damper;

            rectTransform.anchoredPosition = originalPos + offset;

            // Optional: controls how "jittery" it feels
            yield return new WaitForSecondsRealtime(1f / frequency);
        }

        rectTransform.anchoredPosition = originalPos;
    }
    
}
