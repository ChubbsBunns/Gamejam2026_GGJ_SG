using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacter : PlayerBase
{
    [Header("Dash Settings")]
    public float dashSpeed = 800f;
    public float dashDuration = 0.2f;
    
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
    [SerializeField] private GameObject rockWall;

    private AbilityCooldowns cooldowns;
    private const string DASH_ID = "Dash";
    private const string ROCK_WALL_ID = "RockWall";
    private const string ROCK_NORMAL_ATTACK = "RockNormalAttack";
    private const string ROCK_HEAVY_ATTACK = "RockHeavyAttack";
    private const string FIRE_NORMAL_ATTACK = "FireNormalAttack";
    private const string FIRE_HEAVY_ATTACK = "FireHeavyAttack";    
    private Coroutine dashRoutine;
    [SerializeField] private float attackOffsetDistance = 1.2f;
    [Header("Dash / Knockback")]
    [SerializeField] private float heavyAttackDashDistance = 2f;
    [SerializeField] private float heavyAttackDashDuration = 0.1f;
    public MaskID currentMaskID;
    public float rockWallCooldown = 7f;
    public float dashCooldown = 1f;
    public float rockNormalAttackCooldown = 0.4f;
    public float rockHeavyAttackCooldown = 1.0f;
    public float fireNormalAttackCooldown = 0.4f;
    public float fireHeavyAttackCooldown = 1.0f;

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
        cooldowns.Register(ROCK_WALL_ID, rockWallCooldown);
        cooldowns.Register(FIRE_NORMAL_ATTACK, fireNormalAttackCooldown);
        cooldowns.Register(FIRE_HEAVY_ATTACK, fireHeavyAttackCooldown);
        cooldowns.Register(ROCK_NORMAL_ATTACK, rockNormalAttackCooldown);
        cooldowns.Register(ROCK_HEAVY_ATTACK, rockHeavyAttackCooldown);
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
        if (currentMaskID == MaskID.Fire)
        {
            if (isDashing || !cooldowns.IsReady(DASH_ID))
                return;

            StartCoroutine(StartDash());            
        }
        else if (currentMaskID == MaskID.Rock)
        {
            if (!cooldowns.IsReady(ROCK_WALL_ID))
            {
                return;
            }
            ManageRockWallCooldown();
            CreateRockWall();            
        }
    }

#pragma warning disable CS0108
    void Update()
    {
        base.Update();
        
        if (!isActive) return;
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
        SetMaskID(attackComponent.maskAttacks[1].gameObject);
    }

    protected override void OnMask3Selected()
    {
        attackComponent.activeAttack = attackComponent.maskAttacks[2];        
        SetMaskID(attackComponent.maskAttacks[2].gameObject);
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

    private void ManageRockWallCooldown()
    {
        cooldowns.StartCooldown(ROCK_WALL_ID);
    }

    private void ManageNormalRockAttackCooldown()
    {
        cooldowns.StartCooldown(ROCK_NORMAL_ATTACK);
    }

    private void ManageHeavyRockAttackCooldown()
    {
        cooldowns.StartCooldown(ROCK_HEAVY_ATTACK);
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
    // ROCK WALL LOGIC
    // =========================

    private void CreateRockWall()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Vector3 aimDirection = (mouseWorldPos - attackComponent.transform.position).normalized;

        float normalAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg + 90f;
        Vector2 normalDir = new Vector2(
            Mathf.Cos(normalAngle * Mathf.Deg2Rad),
            Mathf.Sin(normalAngle * Mathf.Deg2Rad)
        );
        Quaternion rotation = Quaternion.FromToRotation(Vector3.left, normalDir);
        Instantiate(rockWall, mouseWorldPos, rotation);

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
                    base.animator.Play("rock_down_left");
                    break;                    
                case FacingDirection.DownRight:
                    base.animator.Play("rock_walk_down_right");
                    break;                    
            }

        }

    else
        {
            if (currentMaskID == MaskID.Rock)
            {
                base.animator.Play("idle_rock");
            }
            else if ( currentMaskID == MaskID.Fire)
            {
                base.animator.Play("idle_fire");                
            }
            else
            {
                base.animator.Play("idle_down");                
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
        
    }

    public void DialogueEndSettings()
    {
        print("Dialogue ended");
        
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
