using UnityEngine;
using System.Collections;

public class HitStopManager : MonoBehaviour
{
    public static HitStopManager Instance { get; private set; }

    private Coroutine activeRoutine;

    private void Awake()
    {
        // Singleton pattern (prefab-based)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Stops time temporarily. Duration is in real-time seconds.
    /// </summary>
    public void StartHitStopTime(float duration = 0.1f, float timeScale = 0f)
    {
        if (activeRoutine != null)
            StopCoroutine(activeRoutine);

        activeRoutine = StartCoroutine(HitStopRoutine(duration, timeScale));
    }

    private IEnumerator HitStopRoutine(float duration, float timeScale)
    {
        float originalScale = Time.timeScale;
        Time.timeScale = timeScale;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = originalScale;
        activeRoutine = null;
    }
}
