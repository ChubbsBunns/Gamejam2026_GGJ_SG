using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FireMask : MaskAttackTemplate
{
    [SerializeField] private float attackInterval = 0.12f;
    [SerializeField] private int damagePerHitNormal = 5;
    [SerializeField] private int damagePerHitHeavy = 20;
    private float heavyAttackStartTime;

    public GameObject fireattack1;
    public GameObject fireattack2;

    private bool isAttacking;
    private Coroutine attackRoutine;
    private int index = 0;
    public HashSet<EnemyBase> enemiesToHitNormalAttack;
    public HashSet<EnemyBase> enemiesToHitHeavyAttack;
    [SerializeField] GameObject heavyAttackGameobject;
    [SerializeField] float delayBeforeHeavyAttackDamage = 0.1f;
    [SerializeField] private float minTimeHeavyAttackCharge = 0;
    [SerializeField] private float maxTimeHeavyAttackCharge = 0;

    [SerializeField] float minHeavyColliderSize = 1;
    [SerializeField] float maxHeavyColliderSize = 2;

    [SerializeField] Rigidbody2D playerBodyRB;
    [SerializeField] SpriteRenderer heavyAttackSprite;
    [SerializeField] float heavySpriteFlickerDuration = 0.2f;
    public bool heavyRockAttackInitiated = false;
    private PlayerCharacter player;
    private Camera mainCamera;

    [SerializeField] private float heavyAttackKnockbackForce = 8f;

    void Awake()
    {
        player = FindAnyObjectByType<PlayerCharacter>();
        mainCamera = Camera.main;
        heavyAttackSprite = heavyAttackGameobject.GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        enemiesToHitNormalAttack = new HashSet<EnemyBase>();
        enemiesToHitHeavyAttack = new HashSet<EnemyBase>();
    }

    void OnDisable()
    {
        enemiesToHitNormalAttack.Clear();
        enemiesToHitHeavyAttack.Clear();
    }


    public override void OnAttackStarted()
    {
        
        Debug.Log("Fire Attack started");
        if (isAttacking) return;
        if (attackRoutine != null) return;

        isAttacking = true;
        attackRoutine = StartCoroutine(AttackLoop());
    }

    public override void OnAttackCanceled()
    {
        Debug.Log("Fire Attack canceled");
        isAttacking = false;
        fireattack1.SetActive(false);
        fireattack2.SetActive(false);
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
        StartCoroutine(HeavyAttackCoroutine(chargeTime));

    }

    private IEnumerator HeavyAttackCoroutine(float chargeTime)
    {
        StartCoroutine(HeavyAttackSprite());        
        float clampedTime = Mathf.Clamp(chargeTime, minTimeHeavyAttackCharge, maxTimeHeavyAttackCharge);
        float t = Mathf.InverseLerp(minTimeHeavyAttackCharge, maxTimeHeavyAttackCharge, clampedTime);
        float size = Mathf.Lerp(minHeavyColliderSize, maxHeavyColliderSize, t);
        heavyAttackGameobject.transform.localScale = new Vector3(size, size, 1f);
        yield return new WaitForFixedUpdate();

        DamageEnemiesHeavyAttack();
        Vector2 knockbackDir = GetKnockbackDirection();
        player.BackwardDash(knockbackDir);
    }

    private IEnumerator HeavyAttackSprite()
    {
        heavyAttackSprite.enabled = true;
        yield return new WaitForSeconds(heavySpriteFlickerDuration);
        heavyAttackSprite.enabled = false;
    }

    private IEnumerator AttackLoop()
    {
        while (isAttacking)
        {
            PerformPunch();
            yield return new WaitForSeconds(attackInterval);
        }
    }

    private void PerformPunch()
    {
        Debug.Log("Fire Fist Punch!" + index);
        DamageEnemiesNormalAttack();
        if (index % 2 == 0)
        {
            fireattack1.SetActive(true);
            fireattack2.SetActive(false);
        } 
        else
        {
            fireattack1.SetActive(false);
            fireattack2.SetActive(true);
        }
        index += 1;
    }

    void DamageEnemiesNormalAttack()
    {
        foreach (EnemyBase enemy in enemiesToHitNormalAttack)
        {
            if (enemy == null) 
                continue;

            enemy.ApplyDamage(damagePerHitNormal);
        }
    }

    void DamageEnemiesHeavyAttack()
    {
        foreach (EnemyBase enemy in enemiesToHitHeavyAttack)
        {
            if (enemy == null)
                continue;
            enemy.ApplyDamage(damagePerHitHeavy);
        }
    }

    private Vector2 GetKnockbackDirection()
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0f;

        Vector2 playerPos = player.transform.position;
        Vector2 facingDir = (Vector2)mouseWorld - playerPos;

        return -facingDir.normalized;
    }

}
