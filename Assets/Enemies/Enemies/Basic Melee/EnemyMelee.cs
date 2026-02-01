using System;
using UnityEngine;

public class EnemyMelee : EnemyBase
{
    public float meleeRange = 3.0f;
    public int meleeDamage = 1;

    public GameObject fartPrefab;
    private float fartTimer = 0f;
    public float fartCooldown = 3f;

    public AudioClip fartSound;
    private AudioSource audioSource;

    public Sprite fireFartSprite;
    public Sprite rockFartSprite;

    public bool fireFart = false;
    public bool rockFart = false;
    public enum FartType
    {
        Fire,
        Rock
    }
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


    protected override void Start()
    {
        base.Start();
        fireTimer = 0f;
        fartTimer = UnityEngine.Random.Range(0f, 2f);
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    protected void Update(){
        Vector3 playerPos = targetPlayer.position;
        Vector2 toPlayer = playerPos - transform.position;
        float dist = toPlayer.magnitude;
        float angleToPlayer = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;
        PlayAppropriateWalkAnim(GetFacingDir(angleToPlayer));
        fireTimer = Math.Max(fireTimer -= Time.deltaTime, 0);
        fartTimer = Math.Max(fartTimer -= Time.deltaTime, 0);
        if (dist <= meleeRange)
        {
            if (fireTimer <= 0f)
            {
                Attack();
                fireTimer = fireRate;
            }
        }

        if (fartTimer <= 0f)
        {
            if (fireFart)
            {
                SpawnFart(toPlayer, FartType.Fire);
            }
            else if (rockFart)
            {
                SpawnFart(toPlayer, FartType.Rock);                
            }

            fartTimer = fartCooldown;
        }
    }

    private void PlayAppropriateWalkAnim(FacingDirection facingDirection)
    {
            switch (facingDirection)
            {
                case FacingDirection.Up:
                    anim.Play("up");
                    break;
                case FacingDirection.UpLeft:
                    anim.Play("up_left");
                    break;
                case FacingDirection.UpRight:
                    anim.Play("up_right");
                    break;
                case FacingDirection.Left:
                    anim.Play("left");
                    break;
                case FacingDirection.Right:
                    anim.Play("right");
                    break;
                case FacingDirection.Down:
                    anim.Play("down");
                    break;
                case FacingDirection.DownLeft:
                    anim.Play("left");
                    break;                    
                case FacingDirection.DownRight:
                    anim.Play("right");
                    break;                    
            }
    }

    private void SpawnFart(Vector2 toPlayer, FartType fartType)
    {
        if (fartPrefab == null) return;

        Vector2 behindDir = -toPlayer.normalized;
        Vector3 spawnPos = transform.position + (Vector3)behindDir * 1f;

        GameObject fart = Instantiate(fartPrefab, spawnPos, Quaternion.identity);
        SpriteRenderer sr = fart.GetComponent<SpriteRenderer>();
        if (fartType == FartType.Fire)
        {
            sr.sprite = fireFartSprite;
        }
        else if (fartType == FartType.Rock)
        {
            sr.sprite = rockFartSprite;
        }

        if (fartSound != null)
        {
            audioSource.PlayOneShot(fartSound);
        }
    }

    private void Attack()
    {
        PlayerBase player = targetPlayer.GetComponentInParent<PlayerBase>();
        if (player != null)
        {
            player.TakeDamage(meleeDamage);
        }
    }

    public FacingDirection GetFacingDir(float angle)
    {
        // print(angle);
        if (angle > 157.5 || angle <= -157.5)
            return FacingDirection.Left;
        else if (angle > -112.5 && angle <= -67.5)
            return FacingDirection.Down;
        else if (angle > -22.5 && angle <= 22.5)
            return FacingDirection.Right;
        else if (angle > 67.5 && angle <= 112.5)
            return FacingDirection.Up;
        else if (angle > 112.5 && angle <= 157.5)
            return FacingDirection.UpLeft;
        else if (angle > 22.5 && angle <= 67.5)
            return FacingDirection.UpRight;       
        else if (angle > -67.5 && angle <= -22.5)
            return FacingDirection.DownRight;
        else if (angle > -157.5 && angle <= -112.5)
            return FacingDirection.DownLeft;
        return FacingDirection.Down;
    }
    
}
