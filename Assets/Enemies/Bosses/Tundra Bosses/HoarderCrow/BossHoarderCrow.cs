using Pathfinding;
using UnityEngine;

public class BossHoarderCrow : BossBase
{
    public AIPath aiPath;
    protected override void Awake()
    {
        base.Awake();
        aiPath = GetComponent<AIPath>();
        // You can either assign these manually in Inspector
        // OR create them dynamically here
        if (phases == null)
        {
            phases = new BossPhaseBase[]
            {
                GetComponent<BossPhase_HoarderCrow_Phase1>()
                // Later: add BossPhase_HoarderCrow_Phase2(), etc.
            };
        }
        activePhase = phases[0];
    }

    protected override void Die()
    {
        Debug.Log("HoarderCrow has fallen...");
        base.Die();
    }
}
