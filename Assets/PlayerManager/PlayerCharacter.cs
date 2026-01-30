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
        HandleWalkingAudio();
        UpdateAnimation();

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
        
    }


    protected override void OnAttackCanceled()
    {
        
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
                case FacingDirection.UpRight:
                    base.animator.Play("walk_up_right");
                    break;
                case FacingDirection.UpLeft:
                    base.animator.Play("walk_up_left");
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
            
        } else
        {
            switch (facingDir)
            {
                case FacingDirection.Up:
                    base.animator.Play("idle_up");
                    break;
                case FacingDirection.UpRight:
                    base.animator.Play("idle_up_right");
                    break;
                case FacingDirection.UpLeft:
                    base.animator.Play("idle_up_left");
                    break;
                case FacingDirection.Down:
                    base.animator.Play("idle_down");
                    break;
                case FacingDirection.DownLeft:
                    base.animator.Play("idle_down_left");
                    break;
                case FacingDirection.DownRight:
                    base.animator.Play("idle_down_right");
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
}
