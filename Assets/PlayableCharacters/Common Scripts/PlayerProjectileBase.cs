using UnityEngine;

public class PlayerProjectileBase : MonoBehaviour
{
    protected Vector2 direction;
    protected float speed;
    protected float range;
    protected Vector2 startPos;
    public int damage = 20;

    public virtual void Launch(Vector2 dir, float spd, float rng)
    {
        direction = dir.normalized;
        speed = spd;
        range = rng;
        startPos = transform.position;
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        if (Vector2.Distance(startPos, transform.position) >= range)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.ApplyDamage(damage);
            Destroy(gameObject);
        }
    }
}