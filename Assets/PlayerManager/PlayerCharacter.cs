using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : PlayerBase
{
    [Header("Dash Settings")]
    public float dashSpeed = 800f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 5f;
    public enum AbilityID { Dash };
    [Header("Components")]
    public SpriteRenderer dashIndicator;
    public Animator anim;
    public PlayerAttackComponent attackComponent;
    private bool isDashing = false;
    private bool dashOnCooldown = false;
    private float dashTimer = 0f;
    private Vector2 dashDirection = Vector2.zero;

    public Transform playerBodyPosition;

    [Header("Movement Ability Camera Shake Variables")]
    public float abilityShakeStrength = 0.15f;
    public float abilityShakeDuration = 0.15f;

    private AbilityCooldowns cooldowns;
    private const string DASH_ID = "Dash";
    private Coroutine dashRoutine;
    [SerializeField] private float attackOffsetDistance = 1.2f;
    [Header("Dash / Knockback")]
    [SerializeField] private float heavyAttackDashDistance = 2f;
    [SerializeField] private float heavyAttackDashDuration = 0.1f;

    // =========================
    // UNITY LIFECYCLE
    // =========================

    void Start()
    {
        base.animator = GetComponent<Animator>();

        attackComponent = GetComponentInChildren<PlayerAttackComponent>();
        cooldowns = GetComponent<AbilityCooldowns>();
        if (cooldowns == null)
            cooldowns = gameObject.AddComponent<AbilityCooldowns>();
        cooldowns.Register(DASH_ID, dashCooldown);
        if (base.animator == null)
        {
            print("Base anim not found");
        }
        else
        {
            print("Base anim found");
        }
        base.rb = GetComponent<Rigidbody2D>();
    }
    protected override void OnMovementAbilityStarted()
    {
        if (isDashing || !cooldowns.IsReady(DASH_ID) || !canMove)
            return;

        StartCoroutine(StartDash());
    }

#pragma warning disable CS0108
    void Update()
    {
        base.Update();
        
        if (!isActive || !canMove) return;
        //HandleWalkingAudio();
        //UpdateAnimation();
        UpdateAttackAim();
    }

    void FixedUpdate()
    {
        if (!isActive)
            return;
        if (isDashing)
        {
            DashMovement(Time.fixedDeltaTime);
        }
        else
        {
            base.ProcessMovement(Time.fixedDeltaTime);
        }
    }

    // =========================
    // ATTACK
    // =========================    

    protected override void OnAttackStarted()
    {
        attackComponent.OnAttackStarted();
    }

    protected override void OnAttackCanceled()
    {
        attackComponent.OnAttackCanceled();
    }

    protected override void OnAttackHeavyStarted()
    {
        attackComponent.OnAttackHeavyStarted();
    }

    protected override void OnAttackHeavyCanceled()
    {
        attackComponent.OnAttackHeavyCanceled();
    }    

    private void UpdateAttackAim()
    {
        if (attackComponent == null) return;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Vector3 aimDirection = (mouseWorldPos - attackComponent.transform.position).normalized;

        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        attackComponent.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        attackComponent.transform.position = this.transform.position + aimDirection * attackOffsetDistance;
    }

    // =========================
    // MASK SWAPPING LOGIC
    // =========================

    protected override void OnMask1Selected()
    {
        attackComponent.activeAttack = attackComponent.maskAttacks[0];
    }

    protected override void OnMask2Selected()
    {
        attackComponent.activeAttack = attackComponent.maskAttacks[1];        
    }

    protected override void OnMask3Selected()
    {
        attackComponent.activeAttack = attackComponent.maskAttacks[2];        
    }



    // =========================
    // DASH LOGIC
    // =========================
    private IEnumerator StartDash()
    {
        if (dashIndicator) dashIndicator.enabled = false;
        Audio_MovementAbilityStart();
        isDashing = true;
        dashTimer = dashDuration;
        // Use input direction if moving, otherwise facing
        dashDirection = direction != Vector2.zero ? direction : facing.normalized;
        rb.linearVelocity = dashDirection * dashSpeed;
        StartCoroutine(DirectionalCameraShake(dashDirection, abilityShakeStrength, abilityShakeDuration));
        yield return new WaitForSeconds(dashDuration);
        ManageDashCooldown();
        EndDash();
    }

    private void EndDash()
    {
        isDashing = false;
        rb.linearVelocity = Vector2.zero;
        dashDirection = Vector2.zero;
    }

    private void ManageDashCooldown()
    {
        cooldowns.StartCooldown(DASH_ID);
    }

    private void DashMovement(float delta)
    {
        dashTimer -= delta;
        rb.linearVelocity = dashDirection * dashSpeed;
        if (dashTimer <= 0f)
            EndDash();
    }

    public Transform GetActivePosition()
    {
        return playerBodyPosition.transform;
    }


    public void BackwardDash(Vector2 direction)
    {
        if (dashRoutine != null)
            StopCoroutine(dashRoutine);
        LockMovement(heavyAttackDashDuration);

        rb.linearVelocity = Vector2.zero;
        externalVelocity = Vector2.zero;
        dashRoutine = StartCoroutine(DashCoroutine(direction.normalized));
    }

    // =========================
    // BACKWARD DASH LOGIC
    // =========================
    private IEnumerator DashCoroutine(Vector2 direction)
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + (Vector3)(direction * heavyAttackDashDistance);

        while (elapsed < heavyAttackDashDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / heavyAttackDashDuration;

            // Ease out feels better than linear
            t = Mathf.SmoothStep(0f, 1f, t);

            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        transform.position = targetPos;
        dashRoutine = null;
    }


    // =========================
    // ANIMATION
    // =========================


    private void UpdateAnimation()
    {
        bool isMoving = direction.sqrMagnitude > 0.01f;
        FacingDirection facingDir = GetFacingDir();
        if (isMoving)
        {
            switch (facingDir)
            {
                case FacingDirection.Up:
                    base.animator.Play("walk_up");
                    break;
                case FacingDirection.Left:
                    base.animator.Play("walk_left");
                    break;
                case FacingDirection.Right:
                    base.animator.Play("walk_right");
                    break;
                case FacingDirection.Down:
                    base.animator.Play("walk_down");
                    break;
            }
            
        } else
        {
            switch (facingDir)
            {
                case FacingDirection.Up:
                    base.animator.Play("idle_up");
                    break;
                case FacingDirection.Left:
                    base.animator.Play("idle_up_right");
                    break;
                case FacingDirection.Right:
                    base.animator.Play("idle_up_left");
                    break;
                case FacingDirection.Down:
                    base.animator.Play("idle_down");
                    break;
            }
        }
    }

    protected override void TakeDamageAddOns()
    {
        Audio_Hurt();
    }

    // =========================
    // ADDITIONAL AUDIO HELPERS
    // =========================


    private void HandleWalkingAudio()
    {
        bool isMoving =
            direction.sqrMagnitude > 0.01f &&
            isActive;

        if (isMoving && !wasMovingLastFrame)
        {
            audioEmitter.StartLoop(CharacterAudioEvent.Walk);
        }
        else if (!isMoving && wasMovingLastFrame)
        {
            audioEmitter.StopLoop(CharacterAudioEvent.Walk);
        }
        wasMovingLastFrame = isMoving;
    }

    public void DialogueStartSettings()
    {
        print("Dialogue started");
        canMove = false;
    }

    public void DialogueEndSettings()
    {
        print("Dialogue ended");
        canMove = true;
    }
    // =========================
    // Movement Lock
    // =========================
    public bool MovementLocked { get; private set; }

    public void LockMovement(float duration)
    {
        if (MovementLocked) return;
        StartCoroutine(LockMovementRoutine(duration));
    }


    private IEnumerator LockMovementRoutine(float duration)
    {
        MovementLocked = true;
        yield return new WaitForSeconds(duration);
        MovementLocked = false;
    }

}
