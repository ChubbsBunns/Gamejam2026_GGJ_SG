using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{   
    [System.Serializable]
    public class EnemyInfo
    {
        public GameObject enemyPrefab;
        public int num;
    }
    public Transform UpperLeftSpawnBound;
    public Transform LowerRightSpawnBound;

    public List<EnemyInfo> enemies;

    private int numSpawned = 0;

    private bool firstEntry = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("ENemyManager start");
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            transform.position,
            GetComponent<BoxCollider2D>().bounds.size,
            0f
        );
        print(hits.Length);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("PlayerBody") && !firstEntry)
            {
                StartRoomBattle();
                firstEntry = true;
            }
        }
    }

    public void OnEnemyKilled()
    {
        numSpawned --;
        if (numSpawned <= 0)
        {
            // Map completed
            MapManager.instance.OnRoomCleared();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Entered room: " + other.name);
        if (!firstEntry)
        {
            firstEntry = true;
            StartRoomBattle();
        }
    }

    private void StartRoomBattle()
    {
        MapManager.instance.OnRoomEntered();
        Debug.Log("Spawning enemies");
        foreach (EnemyInfo ei in enemies)
        {
            int numToSpawn = ei.num;
            for (int i = 0; i < numToSpawn; i++)
            {
                Vector2 positionToSpawn;
                positionToSpawn.x = Random.Range(LowerRightSpawnBound.position.x, UpperLeftSpawnBound.position.x);
                positionToSpawn.y = Random.Range(LowerRightSpawnBound.position.y, UpperLeftSpawnBound.position.y);
                GameObject inst = Instantiate(ei.enemyPrefab, positionToSpawn, Quaternion.identity);
                numSpawned ++;
                EnemyBase eb = inst.GetComponent<EnemyBase>();
                if (eb != null)
                {
                    eb.AttachEnemyManager(this);
                }
                AIDestinationSetter ads = inst.GetComponent<AIDestinationSetter>();
                if (ads != null)
                {
                    ads.target = GameObject.FindGameObjectWithTag("Player").transform;
                }
            }
        }
    }

}
