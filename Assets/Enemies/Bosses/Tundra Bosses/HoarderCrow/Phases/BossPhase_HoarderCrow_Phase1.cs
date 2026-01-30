using System.Collections;
using Pathfinding;
using UnityEngine;

public class BossPhase_HoarderCrow_Phase1 : BossPhaseBase
{
    private float attackTimer = 0f;
    private float attackInterval = 2f;
    private int attackIndex = 0;

    // Pathfinding Variables
    public AIPath path;
    [SerializeField] private float moveSpeed;
    public Transform target;
    private bool isAttacking = false;
    public PlayerCharacter playerCharacter;

    public override void Enter(BossBase boss)
    {
        playerCharacter = FindAnyObjectByType<PlayerCharacter>();
        path = boss.GetComponent<AIPath>();
        moveSpeed = 5;
        if (path == null)
        {
            Debug.LogError("path NOT FOUND");
        }
        else
        {
            Debug.Log("path found");
        }        

        target = playerCharacter.transform;
        Debug.Log("[HoarderCrow Phase 1] Entered!");
        attackTimer = 0f;
    }

    public override void UpdateBoss(BossBase boss, float deltaTime)
    {
        if (isAttacking) return;

        target = playerCharacter.transform;
        FollowPlayer();

        attackTimer += deltaTime;
        if (attackTimer >= attackInterval)
        {
            attackTimer = 0f;
            boss.StartCoroutine(PerformAttackRoutine(boss));
        }
    }

    public override void Attack(BossBase boss)
    {
        // Just alternate between two fake attacks for demonstration
        if (attackIndex == 0)
        {
            Debug.Log("HoarderCrow uses Gravitational Chaos!");
        }
        else
        {
            Debug.Log("HoarderCrow unleashes Spirit Barrage!");
        }
        attackIndex = (attackIndex + 1) % 2;
    }

    public override void Exit(BossBase boss)
    {
        Debug.Log("[HoarderCrow Phase 1] Exiting phase.");
    }

    // =========================
    // ATTACKS
    // =========================

    private IEnumerator PerformAttackRoutine(BossBase boss)
    {
        isAttacking = true;
        StopFollowingPlayer();

        // Choose the attack
        if (attackIndex == 0)
            yield return boss.StartCoroutine(AttackGravitationalChaos(boss));
        else
            yield return boss.StartCoroutine(AttackSpiritBarrage(boss));

        attackIndex = (attackIndex + 1) % 2;

        // Cooldown or transition back to following
        isAttacking = false;
    }

    private IEnumerator AttackGravitationalChaos(BossBase boss)
    {
        Debug.Log("HoarderCrow begins Gravitational Chaos!");

        // Example: charge up for 1s
        yield return new WaitForSeconds(1f);

        // Example: launch attack for 2s
        float duration = 2f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // Do attack logic here (e.g., spawn projectiles, move boss)
            yield return null;
        }

        // Maybe brief recovery
        yield return new WaitForSeconds(0.5f);

        Debug.Log("Gravitational Chaos ended!");
    }

    private IEnumerator AttackSpiritBarrage(BossBase boss)
    {
        Debug.Log("HoarderCrow begins Spirit Barrage!");
        yield return new WaitForSeconds(0.5f);

        int shots = 5;
        for (int i = 0; i < shots; i++)
        {
            // spawn projectile, etc.
            Debug.Log($"Spirit Barrage shot {i + 1}");
            yield return new WaitForSeconds(0.3f);
        }

        yield return new WaitForSeconds(1f);
        Debug.Log("Spirit Barrage ended!");
    }

    // =========================
    // Follow Logic
    // =========================

    protected void FollowPlayer()
    {
        path.maxSpeed = moveSpeed;
        path.destination = target.position;
    }
    
    protected void StopFollowingPlayer()
    {
        path.maxSpeed = 0;
    }
}
