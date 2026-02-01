using UnityEngine;

public class FireMaskHeavyAttackCollider : MonoBehaviour
{
    [SerializeField] FireMask fireMask;
    [SerializeField] Collider2D fireMaskHeavyAttack;


    void Start()
    {
        fireMask = GetComponentInParent<FireMask>();
        fireMaskHeavyAttack = GetComponentInParent<Collider2D>();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<EnemyBase>() != null)
        {
            fireMask.enemiesToHitHeavyAttack.Add(collision.gameObject.GetComponent<EnemyBase>());
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (fireMask.enemiesToHitHeavyAttack.Contains(collision.gameObject.GetComponent<EnemyBase>()))
        {
            fireMask.enemiesToHitHeavyAttack.Remove(collision.gameObject.GetComponent<EnemyBase>());            
        }        
    }
}
