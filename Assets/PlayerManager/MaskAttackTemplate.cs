using UnityEngine;

public abstract class MaskAttackTemplate : MonoBehaviour
{
    public enum AttackInput
    {
        Light,
        Heavy,
        Special
    }
    

    public virtual void OnAttackStarted()
    {
        Debug.Log("Mask template attack start");
    }

    public virtual void OnAttackCanceled()
    {
        Debug.Log("Mask template attack cancelled");
    }

    public virtual void OnAttackHeavyStarted()
    {
        Debug.Log("Mask template heavy attack start");
    }

    public virtual void OnAttackHeavyCanceled()
    {
        Debug.Log("Mask template heavy attack cancelled");
    }

}
