using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class BossBase : EnemyBase, IDamageable
{
    [Header("Boss Config")]
    public float introDuration = 2f;
    public float outroDuration = 2.5f;
    public float[] phaseThresholds = { 0.66f, 0.33f };

    [Header("Events")]
    public UnityEvent<int> onPhaseChanged;
    public UnityEvent onFightStarted;
    public UnityEvent onFightEnded;
    public UnityEvent<int, int> onHealthChanged;

    protected bool hasIntroPlayed = false;
    protected bool inIntro = false;
    protected bool inOutro = false;
    protected bool isAlive = true;
    protected int currentPhase = 1;

    [SerializeField] protected BossPhaseBase activePhase;
    [SerializeField] public BossPhaseBase[] phases;

    protected override void Awake()
    {
        base.Awake();
        isAlive = true;
        currentPhase = 1;

        //The below line of code needs to be placed in every custom bosses Awake Function
        //activePhase = phases[0];
    }

    private void Start()
    {
        StartCoroutine(PlayIntro());
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Update()
    {
        if (!isAlive || inIntro || inOutro) return;

        activePhase?.UpdateBoss(this, Time.deltaTime);
        CheckPhaseChange();
    }

    // -----------------------------
    // INTRO / OUTRO
    // -----------------------------
    private IEnumerator PlayIntro()
    {
        if (hasIntroPlayed) yield break;
        hasIntroPlayed = true;
        inIntro = true;

        onFightStarted?.Invoke();
        yield return new WaitForSeconds(introDuration);
        inIntro = false;
        activePhase?.Enter(this);
    }

    private IEnumerator PlayOutro()
    {
        if (inOutro) yield break;
        inOutro = true;
        yield return new WaitForSeconds(outroDuration);
        onFightEnded?.Invoke();
    }

    // -----------------------------
    // PHASE HANDLING
    // -----------------------------
    protected void CheckPhaseChange()
    {
        float ratio = (float)currentHealth / maxHealth;
        for (int i = 0; i < phaseThresholds.Length; i++)
        {
            if (ratio <= phaseThresholds[i] && currentPhase == i + 1)
            {
                currentPhase = i + 2;
                onPhaseChanged?.Invoke(currentPhase);

                if (currentPhase - 1 < phases.Length)
                {
                    activePhase.Exit(this);
                    activePhase = phases[currentPhase - 1];
                    activePhase.Enter(this);
                }

                Debug.Log($"Boss entered Phase {currentPhase}");
            }
        }
    }

    // -----------------------------
    // DAMAGE / HEALTH
    // -----------------------------
    public override void ApplyDamage(int damage)
    {
        if (!isAlive || inIntro || inOutro) return;
        base.ApplyDamage(damage);
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    protected override void Die()
    {
        base.Die();
        isAlive = false;
        StartCoroutine(PlayOutro());
    }
}
