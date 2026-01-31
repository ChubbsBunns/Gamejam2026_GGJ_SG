using UnityEngine;

public class RockProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 6f;
    [SerializeField] private float lifetime = 5f;

    float minHeavyColliderSize = 1;
    float maxHeavyColliderSize = 2;


    private Vector2 moveDirection;
    private float timer;
    private int damage;
    private bool explodiBoi;

    public void Initialize(Vector2 direction, int inputDamage)
    {
        moveDirection = direction.normalized;
        damage = inputDamage;
        explodiBoi = false;
    }

    public void Initialize(Vector2 direction, int inputDamage, float chargePercentage)
    {
        moveDirection = direction.normalized;
        damage = inputDamage;
        explodiBoi = true;

        float size = Mathf.Lerp(minHeavyColliderSize, maxHeavyColliderSize, chargePercentage);
        this.transform.localScale = new Vector3(size, size, 1f);
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = Color.red;
    }

    void Update()
    {
        transform.position += (Vector3)(moveDirection * speed * Time.deltaTime);

        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            if (explodiBoi)
            {
                //explode
            }
            Destroy(this.gameObject);
        }

        if (collision.gameObject.GetComponent<EnemyBase>() != null)
        {
            collision.gameObject.GetComponent<EnemyBase>().ApplyDamage(damage);
            if (explodiBoi)
            {
                //explode
            }
            Destroy(this.gameObject);
        }
    }
}
