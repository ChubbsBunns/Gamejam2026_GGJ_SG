using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneFaderManager : MonoBehaviour
{
    [Header("Defaults")]
    public float defaultFadeInTime = 1f;
    public float defaultFadeOutTime = 1f;
    public Color fadeColor = Color.black;   // default fade color

    private Image img;
    private Coroutine currentFade;

    void Awake()
    {
        img = GetComponent<Image>();
        img.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f); // start transparent
    }

    // -------------------------------
    // PUBLIC FUNCTIONS
    // -------------------------------

    public void FadeIn(float? duration = null, Color? colorOverride = null)
    {
        float time = duration ?? defaultFadeInTime;
        Color color = colorOverride ?? fadeColor;

        StartFade(0f, 1f, time, color);
    }

    public void FadeOut(float? duration = null, Color? colorOverride = null)
    {
        float time = duration ?? defaultFadeOutTime;
        Color color = colorOverride ?? fadeColor;

        StartFade(1f, 0f, time, color);
    }

    // -------------------------------
    // INTERNAL FADE LOGIC
    // -------------------------------

    private void StartFade(float startAlpha, float endAlpha, float duration, Color color)
    {
        // stop old fades
        if (currentFade != null)
            StopCoroutine(currentFade);

        currentFade = StartCoroutine(FadeRoutine(startAlpha, endAlpha, duration, color));
    }

    private IEnumerator FadeRoutine(float startA, float endA, float duration, Color color)
    {
        // Set initial color baseline
        img.color = new Color(color.r, color.g, color.b, startA);

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            float a = Mathf.Lerp(startA, endA, t);

            img.color = new Color(
                color.r,
                color.g,
                color.b,
                a
            );

            yield return null;
        }

        // Force final value
        img.color = new Color(color.r, color.g, color.b, endA);

        currentFade = null;
    }
}
