using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacter : PlayerBase
{
    [Header("Dash Settings")]
    public float dashSpeed = 800f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
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

    public enum MaskID {Fire, Rock};

    private AbilityCooldowns cooldowns;
    private const string DASH_ID = "Dash";
    private Coroutine dashRoutine;
    [SerializeField] private float attackOffsetDistance = 1.2f;
    [Header("Dash / Knockback")]
    [SerializeField] private float heavyAttackDashDistance = 2f;
    [SerializeField] private float heavyAttackDashDuration = 0.1f;
    public MaskID currentMaskID;

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
        attackComponent.activeAttack = null;
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
        UpdateAnimation();
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
        SetMaskID(attackComponent.maskAttacks[0].gameObject);
    }

    protected override void OnMask2Selected()
    {
        attackComponent.activeAttack = attackComponent.maskAttacks[1];        
        SetMaskID(attackComponent.maskAttacks[0].gameObject);
    }

    protected override void OnMask3Selected()
    {
        attackComponent.activeAttack = attackComponent.maskAttacks[2];        
        SetMaskID(attackComponent.maskAttacks[0].gameObject);
    }

    protected void SetMaskID(GameObject maskGameObject)
    {
        if (maskGameObject.GetComponent<FireMask>() != null)
        {
            currentMaskID = MaskID.Fire;
        }
        if (maskGameObject.GetComponent<RockMask>() != null)
        {
            currentMaskID = MaskID.Rock;
        }
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
        // print(facingDir);
        if (isMoving && attackComponent.activeAttack == null)
        {
            switch (facingDir)
            {
                case FacingDirection.Up:
                    base.animator.Play("walk_up");
                    break;
                case FacingDirection.UpLeft:
                    base.animator.Play("walk_up_left");
                    break;
                case FacingDirection.UpRight:
                    base.animator.Play("walk_up_right");
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
                case FacingDirection.DownLeft:
                    base.animator.Play("walk_down_left");
                    break;                    
                case FacingDirection.DownRight:
                    base.animator.Play("walk_down_right");
                    break;                    
            }
            
        } 

        else if (isMoving && currentMaskID == MaskID.Fire)
        {
            switch (facingDir)
            {
                case FacingDirection.Up:
                    base.animator.Play("fire_walk_up");
                    break;
                case FacingDirection.UpLeft:
                    base.animator.Play("fire_walk_up_left");
                    break;
                case FacingDirection.UpRight:
                    base.animator.Play("fire_walk_up_right");
                    break;
                case FacingDirection.Left:
                    base.animator.Play("fire_walk_left");
                    break;
                case FacingDirection.Right:
                    base.animator.Play("fire_walk_right");
                    break;
                case FacingDirection.Down:
                    base.animator.Play("fire_walk_down");
                    break;
                case FacingDirection.DownLeft:
                    base.animator.Play("fire_walk_down_left");
                    break;                    
                case FacingDirection.DownRight:
                    base.animator.Play("fire_walk_down_right");
                    break;                    
            }}

        else if (isMoving && currentMaskID == MaskID.Rock)
        {
            switch (facingDir)
            {
                case FacingDirection.Up:
                    base.animator.Play("rock_walk_up");
                    break;
                case FacingDirection.UpLeft:
                    base.animator.Play("rock_walk_up_left");
                    break;
                case FacingDirection.UpRight:
                    base.animator.Play("rock_walk_up_right");
                    break;
                case FacingDirection.Left:
                    base.animator.Play("rock_walk_left");
                    break;
                case FacingDirection.Right:
                    base.animator.Play("rock_walk_right");
                    break;
                case FacingDirection.Down:
                    base.animator.Play("rock_walk_down");
                    break;
                case FacingDirection.DownLeft:
                    base.animator.Play("rock_walk_down_left");
                    break;                    
                case FacingDirection.DownRight:
                    base.animator.Play("rock_walk_down_right");
                    break;                    
            }

        }

    else
        {
            base.animator.Play("idle_down");

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
