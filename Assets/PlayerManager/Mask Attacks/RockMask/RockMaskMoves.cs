using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RockMask : MaskAttackTemplate
{
    [SerializeField] private float attackInterval = 0.12f;
    [SerializeField] private int damagePerHitNormal = 5;
    [SerializeField] private int damagePerHitHeavy = 20;

    [SerializeField] GameObject normalAttackRock;

    private float heavyAttackStartTime;

    private bool isAttacking;
    private Coroutine attackRoutine;
    private Camera mainCamera;

    [SerializeField] float delayBeforeHeavyAttackDamage = 0.1f;
    [SerializeField] private float minTimeHeavyAttackCharge = 0;
    [SerializeField] private float maxTimeHeavyAttackCharge = 0;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    public override void OnAttackStarted()
    {
        Debug.Log("Rock Attack started");
        if (isAttacking) return;
        if (attackRoutine != null) return;

        isAttacking = true;
        attackRoutine = StartCoroutine(AttackLoop());
    }

    public override void OnAttackCanceled()
    {
        Debug.Log("Fire Attack canceled");
        isAttacking = false;

        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }        
    }

    public override void OnAttackHeavyStarted()
    {
        OnAttackCanceled();
        heavyAttackStartTime = Time.time;
    }

    public override void OnAttackHeavyCanceled()
    {
        
        float chargeTime = Time.time - heavyAttackStartTime;
        DamageEnemiesHeavyAttack(chargeTime);
        HeavyAttackCoroutine(chargeTime);
    }

    private void HeavyAttackCoroutine(float chargeTime)
    {
        float clampedTime = Mathf.Clamp(chargeTime, minTimeHeavyAttackCharge, maxTimeHeavyAttackCharge);
        float t = Mathf.InverseLerp(minTimeHeavyAttackCharge, maxTimeHeavyAttackCharge, clampedTime);
    }

    private IEnumerator AttackLoop()
    {
        while (isAttacking)
        {            
            DamageEnemiesNormalAttack();
            yield return new WaitForSeconds(attackInterval);
        }
    }

    void DamageEnemiesNormalAttack()
    {
        Vector2 direction = GetRockDirection();
        GameObject rock = Instantiate(normalAttackRock, transform.position, Quaternion.identity);
        rock.GetComponent<RockProjectile>().Initialize(direction, damagePerHitNormal);
    }
    private Vector2 GetRockDirection()
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0f;

        Vector2 attackInstantiationPos = this.transform.position;
        Vector2 facingDir = (Vector2)mouseWorld - attackInstantiationPos;

        return facingDir.normalized;
    }

    private void DamageEnemiesHeavyAttack(float chargeTime)
    {
        float clampedTime = Mathf.Clamp(chargeTime, minTimeHeavyAttackCharge, maxTimeHeavyAttackCharge);
        float inputCharge = Mathf.InverseLerp(minTimeHeavyAttackCharge, maxTimeHeavyAttackCharge, clampedTime);

        Vector2 direction = GetRockDirection();
        GameObject rock = Instantiate(normalAttackRock, transform.position, Quaternion.identity);
        rock.GetComponent<RockProjectile>().Initialize(direction, damagePerHitHeavy, inputCharge);
    }
}
