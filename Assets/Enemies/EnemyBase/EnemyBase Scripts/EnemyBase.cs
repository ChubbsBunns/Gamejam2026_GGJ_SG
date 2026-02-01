using UnityEngine;
using System;

[RequireComponent(typeof(Collider2D))]
public class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("Base Stats")]
    public int maxHealth = 100;
    public float deathDelay = 0.5f;

    public float fireRate = 2f;
    public AudioSource hurtSound;
    public AudioSource deathSound;

    protected int currentHealth;
    protected bool isDead = false;
    protected Animator anim;

    protected Transform targetPlayer;

    protected float fireTimer;

    public EnemyHealthBar enemyHealthBar;

    public GameObject healthBarPrefab;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        if (enemyHealthBar == null)
        {
            enemyHealthBar = Instantiate(
                healthBarPrefab,
                new Vector3(-0.6f, 0.5f, 0),
                Quaternion.identity,
                transform
            ).GetComponent<EnemyHealthBar>();            
        }
        enemyHealthBar.transform.localPosition = new Vector3(-0.6f, 0.5f, 0f);
    }

    protected virtual void Start()
    {
        targetPlayer = FindAnyObjectByType<PlayerCharacter>().GetActivePosition();
    }

    protected virtual Vector3 GetCurrentTargetPosition()
    {
        return targetPlayer.position;
    }

    public virtual void ApplyDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Remaining HP: {currentHealth}");
        //hurtSound?.Play();
        OnDamaged(damage);
        enemyHealthBar.UpdateHealthBar((float) currentHealth/maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void OnDamaged(int damage, float hitStopDuration = 0.05f)
    {
        HitStopManager.Instance.StartHitStopTime(hitStopDuration);
        if (anim)
            anim.SetTrigger("Hurt");
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log($"{gameObject.name} has died.");

        deathSound?.Play();
        if (anim)
            anim.SetTrigger("Die");

        StartCoroutine(DestroyAfterDelay());
    }

    private System.Collections.IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(deathDelay);
        Destroy(gameObject);
    }
    
    
}
