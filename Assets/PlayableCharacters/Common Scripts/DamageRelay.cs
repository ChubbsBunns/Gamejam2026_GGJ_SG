using UnityEngine;

public class DamageRelay : MonoBehaviour
{
    public PlayerBase playerParent;

    private void Awake()
    {
        playerParent = GetComponentInParent<PlayerBase>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IDamageSource source = other.GetComponent<IDamageSource>();
        if (source != null && playerParent != null)
        {
            playerParent.TakeDamage(source.Damage);
        }
    }

}
