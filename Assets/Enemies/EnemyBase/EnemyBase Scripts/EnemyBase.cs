using UnityEngine;
using System;
using Unity.VisualScripting;

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

    private EnemyManager enemyManager;



    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
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

        if (enemyManager != null)
        {
            enemyManager.OnEnemyKilled();
        }

        StartCoroutine(DestroyAfterDelay());
    }

    private System.Collections.IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(deathDelay);
        Destroy(gameObject);
    }

    public void AttachEnemyManager(EnemyManager em)
    {
        enemyManager = em;
    }
}
