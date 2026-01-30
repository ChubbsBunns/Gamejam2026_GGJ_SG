using System.Collections.Generic;
using UnityEngine;

public class SlashDetector : MonoBehaviour
{
    public List<Transform> pathPoints;    // set these in inspector
    public float toleranceRadius = 0.4f;

    private int currentIndex = 0;
    private bool tracking = false;

    public System.Action OnSlashComplete;

    public void StartTracking()
    {
        tracking = true;
        currentIndex = 0;
    }

    public void StopTracking()
    {
        tracking = false;
    }

    public void TrackPoint(Vector3 worldPos)
    {
        if (!tracking) return;

        if (currentIndex >= pathPoints.Count) return;

        float dist = Vector3.Distance(worldPos, pathPoints[currentIndex].position);
        if (dist < toleranceRadius)
        {
            PortalRipple ripple = pathPoints[currentIndex].GetComponent<PortalRipple>();
            if (ripple != null)
            {
                ripple.OpenRipple();
            }
            currentIndex++;

            if (currentIndex >= pathPoints.Count)
            {
                tracking = false;
                OnSlashComplete?.Invoke();
            }
        }
    }
}
