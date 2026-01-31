using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public abstract class PlayerBase : MonoBehaviour
{
    // =========================
    // CONFIG
    // =========================
    [Header("Config")]
    public float moveSpeed = 300f;
    public int maxHealth = 5;
    public float externalVelocityDamp = 6f;

    // =========================
    // STATE
    // =========================
    [Header("Runtime State")]
    public int currentHealth;
    public Vector2 direction = Vector2.zero;
    public Vector2 facing = Vector2.down;

    // COLLIDERS
    public List<Collider2D> colliders = new List<Collider2D>();
    public List<Collider2D> areaColliders = new List<Collider2D>();
    public int originalLayer;
    public int originalLayerMask;

    // CONTROL FLAGS
    public bool isActive = true;
    public bool canMove = true;
    public bool slowed = false;
    [SerializeField] private float activeSlowMultiplier = 1f;
    public bool isBound = false;

    // EXTERNAL AFFECTORS
    public Vector2 externalVelocity = Vector2.zero;

    // COMPONENTS
    public Rigidbody2D rb;
    protected Animator animator;

    [Header("Player Controls")]
    public PlayerInputActions playerControls;
    protected InputAction move;
    protected InputAction attack;
    protected InputAction movementAbility;
    protected InputAction utilityAbility;
    protected InputAction utility2Ability;
    protected InputAction attackHeavy;
    protected InputAction mask1;
    protected InputAction mask2;
    protected InputAction mask3;

    // EVENTS
    public event Action<int, int> OnHealthChanged;
    public event Action OnDied;

    public event Action<string> OnAbilityRead;

    // CROSSHAIR MODULE
    public GameObject crossHairObject;
    [SerializeField] private float crossHairSmoothingVal = 20f;

    [Header("Damage Settings")]
    public float postHitInvulnerability = 0.15f;
    private float invulnerableUntil = 0f;

    [Header("Modifiers")]
    private float currentSpeedMultiplier = 1f;

    [Header("Dialogue Helpers")]
    private bool overrideFacing = false;
    private Vector2 forcedFacing = Vector2.down;

    [Header("Peripheral Variables")]
    public float histStopTime = 0.1f;

    [Header("Audio Helpers")]
    [SerializeField] public CharacterAudioEmitter audioEmitter;
    [SerializeField] public bool wasMovingLastFrame = false;

    [Header("Stamina State")]
    [SerializeField] private float exhaustedMoveMultiplier = 0.4f;

    [SerializeField] private GameObject hurtCanvasLayer;
    PlayerHealthUI playerHealthUI;

    public enum FacingDirection
    {
        Up,
        UpLeft,
        UpRight,
        Down,
        DownLeft,
        DownRight,
        Left,
        Right,
    }

    public enum PlayerCharacterID
    {
        Player
    }

    public PlayerCharacterID characterID;

    //Helpers
    public bool ableToSwap = true;
    
    [System.Serializable]
    public struct AbilityInfo
    {
        public PlayerAbilityKey key;
        public float cooldown;
        public AbilityInfo(PlayerAbilityKey key, float cooldown)
        {
            this.key = key;
            this.cooldown = cooldown;
        }
    }

    public virtual List<AbilityInfo> GetAbilities()
    {
        return new List<AbilityInfo>();
    }

    // =========================
    // UNITY LIFECYCLE
    // =========================
    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerInputActions();            
        }        
        move = playerControls.Player.Move;
        move.Enable();
        attack = playerControls.Player.Attack;
        attack.Enable();
        attack.performed += Attack;
        attackHeavy = playerControls.Player.AttackHeavy;
        attackHeavy.Enable();
        attackHeavy.performed += AttackHeavy;
        movementAbility = playerControls.Player.MovementAbility;
        movementAbility.Enable();
        movementAbility.performed += MovementAbility;
        utilityAbility = playerControls.Player.UtilityAbility;
        utilityAbility.Enable();
        utilityAbility.performed += UtilityAbility;
        utility2Ability = playerControls.Player.UtilityAbility2;
        utility2Ability.Enable();
        utility2Ability.performed += Utility2Ability;
        
        mask1 = playerControls.Player.Mask1;
        mask1.Enable();
        mask1.performed += Mask1;

        mask2 = playerControls.Player.Mask2;
        mask2.Enable();
        mask2.performed += Mask2;

        mask3 = playerControls.Player.Mask3;
        mask3.Enable();
        mask3.performed += Mask3;

        movementAbility.started += ctx => OnMovementAbilityStarted();
        movementAbility.canceled += ctx => OnMovementAbilityCanceled();
        utilityAbility.started += ctx => OnUtilityAbilityStarted();
        utilityAbility.canceled += ctx => OnUtilityAbilityCanceled();
        utility2Ability.started += ctx => OnUtility2AbilityStarted();
        utility2Ability.canceled += ctx => OnUtility2AbilityCanceled();
        attack.started += ctx => OnAttackStarted();
        attack.canceled += ctx => OnAttackCanceled();
        attackHeavy.started += ctx => OnAttackHeavyStarted();
        attackHeavy.canceled += ctx => OnAttackHeavyCanceled();
        mask1.started += ctx => OnMask1Selected();
        mask2.started += ctx => OnMask2Selected();
        mask3.started += ctx => OnMask3Selected();
    }

    void OnDisable()
    {
        move.Disable();
        attack.Disable();
        movementAbility.Disable();

    }

    void Awake()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerInputActions();            
        }
        animator = GetComponent<Animator>();
        CacheColliders();
        originalLayer = gameObject.layer;
        currentHealth = maxHealth;
        playerHealthUI = FindAnyObjectByType<PlayerHealthUI>();
        playerHealthUI.UpdateHearts(maxHealth, maxHealth);
    }

    protected void Update()
    {
        if (!isActive)
            return;
        if (!(this is PlayerCharacter pc && pc.MovementLocked))
        {
            direction = move.ReadValue<Vector2>().normalized;
        }
        else
        {
            direction = Vector2.zero;
        }

        if (!overrideFacing)
        {
            Vector2 mouseScreen = Mouse.current.position.ReadValue();
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(
                new Vector3(mouseScreen.x, mouseScreen.y, Camera.main.nearClipPlane));

            facing = (mouseWorld - transform.position).normalized;
        }
        else
        {
            facing = forcedFacing.normalized;
        }        
    }

    void FixedUpdate()
    {
        ProcessMovement(Time.fixedDeltaTime);
    }

    // =========================
    // MOVEMENT
    // =========================
    public void ProcessMovement(float delta)
    {
        if (!isActive)
            return;

        Vector2 finalVelocity = Vector2.zero;

        if (canMove)
        {
            float speed = moveSpeed * currentSpeedMultiplier;
            finalVelocity = direction * speed;
        }

        Vector2 velocity = finalVelocity + externalVelocity;
        rb.linearVelocity = velocity;
    }

    // =========================
    // ACTIVATION / DEACTIVATION
    // =========================
    public void Activate()
    {
        isActive = true;
        gameObject.SetActive(true);
        SetCollisionsActive(true);
    }


    public void Deactivate()
    {
        isActive = false;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;            
        }

        externalVelocity = Vector2.zero;
        SetCollisionsActive(false);
        gameObject.SetActive(false);
    }

    // =========================
    // HEALTH
    // =========================
    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0 || Time.time < invulnerableUntil)
            return;
        invulnerableUntil = Time.time + postHitInvulnerability;
        HitStopManager.Instance.StartHitStopTime(histStopTime);
        currentHealth = Mathf.Max(0, currentHealth - amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        TakeDamageAddOns();
        Animator hurtCanvasAnimator = hurtCanvasLayer.GetComponent<Animator>();
        playerHealthUI.UpdateHearts(currentHealth, maxHealth);
        hurtCanvasAnimator.Play("Hurt");

        if (currentHealth == 0)
            Die();
    }



    abstract protected void TakeDamageAddOns();


    public void Heal(int amount)
    {
        if (currentHealth <= 0)
            return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    void Die()
    {
        OnDied?.Invoke();
        Debug.Log($"{name} has died!");
    }

    // =========================
    // COLLISION HANDLING
    // =========================
    void CacheColliders()
    {
        colliders.Clear();
        areaColliders.Clear();

        Collider2D[] all = GetComponentsInChildren<Collider2D>(true);
        foreach (var c in all)
        {
            if (c.isTrigger)
                areaColliders.Add(c);
            else
                colliders.Add(c);
        }
    }

    protected void SetCollisionsActive(bool active)
    {
        foreach (var c in colliders)
            if (c != null) c.enabled = active;

        foreach (var c in areaColliders)
            if (c != null) c.enabled = active;

        gameObject.layer = active ? originalLayer : 0;
    }

    // =========================
    // ANIMATION & FACING HELPERS
    // =========================
    public FacingDirection GetFacingDir()
    {
        float angle = GetFacingAngle();
        if (angle > 0 && angle <= 60)
            return FacingDirection.Left;
        if (angle > -60 && angle <= 0)
            return FacingDirection.Down;
        if (angle > -120 && angle <= -50)
            return FacingDirection.Right;
        if (angle > 60 && angle <= 120)
            return FacingDirection.Up;

        return FacingDirection.Down;
    }

    public float GetFacingAngle()
    {
        return Mathf.Atan2(facing.y, facing.x) * Mathf.Rad2Deg;
    }

    // =========================
    // EXTERNAL INFLUENCES
    // =========================
    public void AddExternalVelocity(Vector2 force)
    {
        externalVelocity = force;
    }

    public void ClearExternalVelocity()
    {
        externalVelocity = Vector2.zero;
    }

    public void StopMovement()
    {
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
        direction = Vector2.zero;
    }

    public void SetMovementEnabled(bool enabled)
    {
        canMove = enabled;
    }

    public void OverrideFacing(Vector2 worldPoint)
    {
        overrideFacing = true;
        forcedFacing = (worldPoint - (Vector2)transform.position).normalized;
    }

    public void OverrideFacingDirection(Vector2 dir)
    {
        overrideFacing = true;
        forcedFacing = dir.normalized;
    }

    public void ClearOverrideFacing()
    {
        overrideFacing = false;
    }

    // =========================
    // HITSTOP (Time Freeze)
    // =========================
    public IEnumerator ApplyHitstop(float duration = 0.01f, float slowFactor = 0.1f)
    {
        float oldScale = Time.timeScale;
        Time.timeScale = slowFactor;
        yield return new WaitForSecondsRealtime(duration * slowFactor);
        Time.timeScale = oldScale;
    }

    public void ApplyHitstop(float t)
    {
        //StartCoroutine(HitstopRoutine(t));
    }

    private void MovementAbility(InputAction.CallbackContext context)
    {
        //Debug.Log("We Move ");
    }

    private void Attack(InputAction.CallbackContext context)
    {
    }

    private void AttackHeavy(InputAction.CallbackContext context)
    {
    }    

    private void UtilityAbility(InputAction.CallbackContext context)
    {
    }

    private void Utility2Ability(InputAction.CallbackContext context)
    {
    }

    private void Mask1(InputAction.CallbackContext context)
    {}

    private void Mask2(InputAction.CallbackContext context)
    {}

    private void Mask3(InputAction.CallbackContext context)
    {}




    // =========================
    // CAMERA DIRECTIONAL SHAKE
    // =========================
    public IEnumerator DirectionalCameraShake(Vector2 dir, float strength = 0.2f, float duration = 0.1f)
    {
        Camera cam = Camera.main;
        if (cam == null)
            yield break;

        // Normalize direction so strength is consistent
        Vector3 shakeDir = ((Vector3)dir).normalized;
        Vector3 originalPos = cam.transform.position;

        // Offset target (z stays same)
        Vector3 offset = shakeDir * strength;
        offset.z = 0f;

        // Move camera toward offset
        float halfDuration = duration * 0.5f;
        float elapsed = 0f;

        // Ease in
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            cam.transform.position = Vector3.Lerp(originalPos, originalPos + offset, t);
            yield return null;
        }

        // Ease out back to original
        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            cam.transform.position = Vector3.Lerp(originalPos + offset, originalPos, t);
            yield return null;
        }

        cam.transform.position = originalPos;
    }


    // =========================
    // DIALOGUE STATE HELPER FUNCTIONS
    // =========================


    public void SetDialogueFacing(Vector2 targetPoint)
    {
        overrideFacing = true;
        forcedFacing = (targetPoint - (Vector2)transform.position).normalized;
    }

    public void SetDialogueFacingDirection(Vector2 direction)
    {
        overrideFacing = true;
        forcedFacing = direction.normalized;
    }

    public void ClearDialogueFacing()
    {
        overrideFacing = false;
    }


    // =========================
    // ANIMATION & FACING HELPERS
    // =========================
    public void EnableCrosshair(bool enable)
    {
        if (crossHairObject == null)
            return;

        if (enable)
        {
            // Snap instantly to current mouse world position
            Camera cam = Camera.main;
            if (cam != null)
            {
                Vector2 mouseScreen = Mouse.current.position.ReadValue();
                Vector3 mouseWorld = cam.ScreenToWorldPoint(
                    new Vector3(mouseScreen.x, mouseScreen.y, -cam.transform.position.z)
                );
                mouseWorld.z = 0f;
                crossHairObject.transform.position = mouseWorld;
                crossHairObject.SetActive(false);
                StartCoroutine(ReenableCrosshairNextFrame());
            }
        }
    }

    private IEnumerator ReenableCrosshairNextFrame()
    {
        yield return null;
        if (crossHairObject != null)
            crossHairObject.SetActive(true);
    }

    public void UpdateCrosshair(Camera cam)
    {
        if (crossHairObject == null || !crossHairObject.activeSelf)
            return;

        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = cam.ScreenToWorldPoint(
            new Vector3(mouseScreen.x, mouseScreen.y, -cam.transform.position.z)
        );
        mouseWorld.z = 0f;

        // Smooth follow
        crossHairObject.transform.position = Vector3.Lerp(
            crossHairObject.transform.position, mouseWorld, Time.deltaTime * crossHairSmoothingVal
        );
    }


    protected virtual void OnMovementAbilityStarted() { }
    protected virtual void OnMovementAbilityCanceled() { }
    protected virtual void OnUtilityAbilityStarted() { }
    protected virtual void OnUtilityAbilityCanceled() { }
    protected virtual void OnUtility2AbilityStarted() { }
    protected virtual void OnUtility2AbilityCanceled() { }    
    protected virtual void OnAttackStarted() { }
    protected virtual void OnAttackCanceled() { }

    protected virtual void OnAttackHeavyStarted() { }
    protected virtual void OnAttackHeavyCanceled() { } 

    protected virtual void OnMask1Selected() {}
    protected virtual void OnMask2Selected() {}
    protected virtual void OnMask3Selected() {}   

    // =========================
    // AUDIO HELPERS
    // =========================
    public void Audio_AttackCharge()
    {
        audioEmitter.Play(CharacterAudioEvent.AttackCharge);
    }

    public void Audio_AttackCriticalStart()
    {
        audioEmitter.Play(CharacterAudioEvent.AttackCriticalStart);
    }

    public void Audio_AttackStart()
    {
        audioEmitter.Play(CharacterAudioEvent.AttackStart);
    }

    public void Audio_AttackStartWeaponSound()
    {
        print("I am called");
        audioEmitter.Play(CharacterAudioEvent.AttackStartWeaponSound);
    }

    public void Audio_AttackHit()
    {
        audioEmitter.Play(CharacterAudioEvent.AttackHit);
    }

    public void Audio_AttackMiss()
    {
        audioEmitter.Play(CharacterAudioEvent.AttackMiss);
    }

    public void Audio_ChargeStart()
    {
        audioEmitter.Play(CharacterAudioEvent.ChargeStart);
    }

    public void Audio_ChargeRelease()
    {
        audioEmitter.Play(CharacterAudioEvent.ChargeRelease);
    }

    public void Audio_Hurt()
    {
        audioEmitter.Play(CharacterAudioEvent.Hurt);
    }

    public void Audio_Death()
    {
        audioEmitter.Play(CharacterAudioEvent.Death);
    }

    public void Audio_MovementAbilityStart()
    {
        audioEmitter.Play(CharacterAudioEvent.MovementAbilityStart);
    }

    public void Audio_MovementAbilityEnd()
    {
        audioEmitter.Play(CharacterAudioEvent.MovementAbilityEnd);
    }

    public void Audio_MovementAbilityStart2()
    {
        audioEmitter.Play(CharacterAudioEvent.MovementAbilityStart2);
    }

    public void Audio_MovementAbilityEnd2()
    {
        audioEmitter.Play(CharacterAudioEvent.MovementAbilityEnd2);
    }

    public void AudioStart_ChargeLoop()
    {
        audioEmitter?.StartLoop(CharacterAudioEvent.ChargeHold);
    }

    public void AudioStart_ChargeLoop_End()
    {
        audioEmitter?.StopLoop(CharacterAudioEvent.ChargeHold);
    }

    public void Audio_StartMovementAbilityLoop()
    {
        audioEmitter.StartLoop(CharacterAudioEvent.MovementAbilityLoop);
    }

    public void Audio_StopMovementAbilityLoop()
    {
        audioEmitter.StopLoop(CharacterAudioEvent.MovementAbilityLoop);
    }



}
