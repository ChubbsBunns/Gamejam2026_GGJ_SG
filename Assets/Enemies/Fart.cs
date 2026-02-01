using UnityEngine;

public class Fart : MonoBehaviour, IDamageSource
{
    public int damage = 1;
    public int Damage => damage;

    void Start()
    {
        Destroy(gameObject, 3f);
    }


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
