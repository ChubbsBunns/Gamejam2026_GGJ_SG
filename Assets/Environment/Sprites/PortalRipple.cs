using System.Collections;
using UnityEngine;

public class PortalRipple : MonoBehaviour
{
    private SpriteRenderer sr;
    public float growDuration = 0.4f;   // time to grow to full size
    [SerializeField] private Sprite rippedSprite;
    private bool opened = false;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = true;        
    }

    public void OpenRipple()
    {
        if (!opened)
        {
            opened = true;
            sr.sprite = rippedSprite;
            StopAllCoroutines();
            StartCoroutine(GrowRipple());            
        }

    }

    private IEnumerator GrowRipple()
    {
        float t = 0f;
        Vector3 start = Vector3.zero;
        Vector3 end = Vector3.one;

        while (t < 1f)
        {
            t += Time.deltaTime / growDuration;

            // Smooth easing (optional but looks nicer)
            float eased = Mathf.SmoothStep(0f, 1f, t);

            transform.localScale = Vector3.Lerp(start, end, eased);
            yield return null;
        }

        transform.localScale = end; // ensure exactly 1,1,1
    }
}
