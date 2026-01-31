using UnityEngine;

public class RockWall : MonoBehaviour
{
    void Start()
    {
        Animator animator= GetComponent<Animator>();
        animator.Play("RockWallFade");
        Destroy(this.gameObject, 4.0f);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<EnemyProjectile>() != null || collision.gameObject.GetComponent<RockProjectile>() != null)
        {
            Destroy(collision.gameObject);
        }
        else
        {
            print("Nah u dont have da balls");
        }
    }
}
