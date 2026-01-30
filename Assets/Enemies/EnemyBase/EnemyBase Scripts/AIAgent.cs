using Pathfinding;
using UnityEngine;

public class AIAgent : MonoBehaviour
{
    private AIPath path;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Transform target;

    private void Start()
    {
        path = GetComponent<AIPath>();
    }

    void Update()
    {
        path.maxSpeed = moveSpeed;
        path.destination = target.position;
    }
}
