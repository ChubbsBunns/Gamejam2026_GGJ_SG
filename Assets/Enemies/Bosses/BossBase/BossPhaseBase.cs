using UnityEngine;

public abstract class BossPhaseBase : MonoBehaviour
{
    public virtual void Enter(BossBase boss) { }
    public virtual void UpdateBoss(BossBase boss, float deltaTime) { }
    public virtual void Attack(BossBase boss) { }
    public virtual void Exit(BossBase boss) { }
}
