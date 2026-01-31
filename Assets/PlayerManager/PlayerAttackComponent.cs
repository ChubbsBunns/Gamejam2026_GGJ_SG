using UnityEngine;

public class PlayerAttackComponent : MonoBehaviour
{
    public MaskAttackTemplate[] maskAttacks;
    public MaskAttackTemplate activeAttack;

    void Start()
    {
        activeAttack = maskAttacks[0];
    }
    public void OnAttackStarted()
    {
        activeAttack.OnAttackStarted();
    }

    public void OnAttackCanceled()
    {
        activeAttack.OnAttackCanceled();
    }

    public void OnAttackHeavyStarted()
    {
        Debug.Log("Attack heavy start");
        activeAttack.OnAttackHeavyStarted();
    }

    public void OnAttackHeavyCanceled()
    {
        Debug.Log("Attack heavy cancel");
        activeAttack.OnAttackHeavyCanceled();
    }
}
