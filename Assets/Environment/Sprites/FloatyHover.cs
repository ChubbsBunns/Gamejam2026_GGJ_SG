using UnityEngine;

public class FloatyHover : MonoBehaviour
{
    [Header("Float Motion")]
    public float amplitude = 0.2f;
    public float frequency = 1.2f;
    public float rotationAmount = 5f;

    [Header("Hold Effects (Shimmer + Jitter)")]
    public float holdShakeAmount = 0.05f;
    public float holdShakeSpeed = 40f;
    public float pulseSpeed = 8f;
    public float pulseAmount = 0.3f;

    public bool floating = true;  

    private Vector3 startPos;

    private Vector3 jitterOffset;
    private SpriteRenderer sr;


    void Start()
    {
        startPos = transform.localPosition;
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (floating)
        {
            float offset = Mathf.Sin(Time.time * frequency) * amplitude;
            transform.localPosition = startPos + new Vector3(0, offset, 0);

            // Slight rotation for life
            transform.localRotation = Quaternion.Euler(
                0,
                0,
                Mathf.Sin(Time.time * frequency) * rotationAmount
            );
            jitterOffset = Vector3.zero;
            sr.color = Color.white;
        }
        else
        {
            jitterOffset = new Vector3(
                Mathf.Sin(Time.time * holdShakeSpeed) * holdShakeAmount,
                Mathf.Cos(Time.time * holdShakeSpeed) * holdShakeAmount,
                0
            );

            transform.localPosition += jitterOffset;
            float pulse = (Mathf.Sin(Time.time * pulseSpeed) * 0.5f + 0.5f) * pulseAmount;

            sr.color = new Color(
                1f + pulse,
                1f + pulse,
                1f + pulse,
                1f
            );
        }

    }
}
