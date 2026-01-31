using System;
using UnityEngine;

public class EnemyMelee : EnemyBase
{
    public float meleeRange = 3.0f;
    public int meleeDamage = 1;

    protected override void Start()
    {
        base.Start();
        fireTimer = 0f;
    }

    protected void Update(){
        Vector3 playerPos = targetPlayer.position;
        Vector2 toPlayer = playerPos - transform.position;
        float dist = toPlayer.magnitude;

        fireTimer = Math.Max(fireTimer -= Time.deltaTime, 0);
        if (dist <= meleeRange)
        {
            if (fireTimer <= 0f)
            {
                Attack();
                fireTimer = fireRate;
            }
        }
    }

    private void Attack()
    {   
        Debug.Log("Melee attk");
        PlayerBase player = targetPlayer.GetComponentInParent<PlayerBase>();
        if (player != null)
        {
            player.TakeDamage(meleeDamage);
        }
        
    }
}
