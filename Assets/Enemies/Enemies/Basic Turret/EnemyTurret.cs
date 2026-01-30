using UnityEngine;

public class EnemyTurret : EnemyBase
{
    [Header("Turret Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 8f;
    public float fireRate = 2f;
    public float detectionRange = 12f;
    public bool rotateTowardsTarget = true;

    private float fireTimer;

    public PlayerCharacter playerCharacter;

    protected override void Awake()
    {
        base.Awake();
        playerCharacter = FindAnyObjectByType<PlayerCharacter>();
    }

    private void Update()
    {
        Vector3 playerPos = playerCharacter.GetActivePosition().position;
        Vector2 toPlayer = playerPos - transform.position;
        float dist = toPlayer.magnitude;

        if (dist <= detectionRange)
        {
            // Optionally rotate turret
            if (rotateTowardsTarget && toPlayer != Vector2.zero)
            {
                float angle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg + 180;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }

            // Shoot if timer allows
            fireTimer -= Time.deltaTime;
            if (fireTimer <= 0f)
            {
                Shoot(toPlayer.normalized);
                fireTimer = fireRate;
            }
        }
    }

    private void Shoot(Vector2 direction)
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = direction * projectileSpeed;

        EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();
        if (ep != null)
            ep.damage = 1; // or expose this in inspector

        Destroy(proj, 5f); // cleanup
    }
}
