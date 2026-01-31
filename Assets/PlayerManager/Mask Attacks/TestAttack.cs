using UnityEngine;

public class TestAttack : MaskAttackTemplate
{
    public override void OnAttackStarted()
    {
        Debug.Log("Test Attack started");
    }

    public override void OnAttackCanceled()
    {
        Debug.Log("Test Attack canceled");
    }

    public override void OnAttackHeavyStarted()
    {
        Debug.Log("Test Heavy Attack Started");
    }
}
