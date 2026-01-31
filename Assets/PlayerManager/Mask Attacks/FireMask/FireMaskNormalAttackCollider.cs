using UnityEngine;

public class FireMaskNormalAttackCollider : MonoBehaviour
{
    [SerializeField] FireMask fireMask;

    void Start()
    {
        fireMask = GetComponentInParent<FireMask>();
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<EnemyBase>() != null)
        {
            fireMask.enemiesToHitNormalAttack.Add(collision.gameObject.GetComponent<EnemyBase>());
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (fireMask.enemiesToHitNormalAttack.Contains(collision.gameObject.GetComponent<EnemyBase>()))
        {
            fireMask.enemiesToHitNormalAttack.Remove(collision.gameObject.GetComponent<EnemyBase>());            
        }        
    }
}
