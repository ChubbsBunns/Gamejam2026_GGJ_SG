using UnityEngine;

public class BossPhase_HoarderCrow_Phase2 : BossPhaseBase
{
    private float attackTimer = 0f;
    private float attackInterval = 2f;
    private int attackIndex = 0;

    public override void Enter(BossBase boss)
    {
        Debug.Log("[HoarderCrow Phase 2] Entered!");
        attackTimer = 0f;
    }

    public override void UpdateBoss(BossBase boss, float deltaTime)
    {
        attackTimer += deltaTime;
        if (attackTimer >= attackInterval)
        {
            attackTimer = 0f;
            Attack(boss);
        }
    }

    public override void Attack(BossBase boss)
    {
        // Just alternate between two fake attacks for demonstration
        if (attackIndex == 0)
        {
            Debug.Log("HoarderCrow uses Spirits Shield!");
        }
        else
        {
            Debug.Log("HoarderCrow Releases the spirit shield!");
        }

        attackIndex = (attackIndex + 1) % 2;
    }

    public override void Exit(BossBase boss)
    {
        Debug.Log("[HoarderCrow Phase 1] Exiting phase.");
    }
}
