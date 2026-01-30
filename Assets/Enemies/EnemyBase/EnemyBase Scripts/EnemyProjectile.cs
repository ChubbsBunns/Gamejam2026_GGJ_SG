using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyProjectile : MonoBehaviour, IDamageSource
{
    public int damage = 1;
    public int Damage => damage;

    public AudioClip shieldBlockedAudioClip;

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageRelay player = other.GetComponent<DamageRelay>();
        if (player != null)
        {
            
            print("Player found, player taking damage");
            Destroy(gameObject);
        }
    }
}