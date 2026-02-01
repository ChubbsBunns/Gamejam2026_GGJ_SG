using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    public GameObject healthBar;

    public void UpdateHealthBar(float scale)
    {
        print(this.gameObject.name + " Health " + scale);
        healthBar.transform.localScale = new Vector3(scale, 1, 1);
    }
    
}
