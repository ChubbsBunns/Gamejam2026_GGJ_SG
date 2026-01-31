using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

   

    [SerializeField]
    public enum RoomGroup
    {
        A,
        B,
        C,
        D
    }

    [System.Serializable]
     public class RoomPrefabs
    {
        public RoomGroup roomGroup;
        public GameObject roomPrefab;
    }

    [System.Serializable]
     public class RoomSpawnPoints
    {
        public RoomGroup roomGroup;
        public Vector3 spawnPoint;
    }

    [Header("Rooms (in order)")]
    public List<RoomPrefabs> roomPrefabs;

    [Header("Spawn Settings")]
    public List<RoomSpawnPoints> roomSpawnPoints;

    private GameObject currentRoomInstance;

    private GameObject previousRoomInstance;

    private RoomGroup currentRoomGroup = RoomGroup.A;

    private void Start()
    {
        Debug.Log("Spawning room A");
        SpawnRoom();
    }

    private void SpawnRoom()
    {
        GameObject roomToSpawn = null;
        foreach (RoomPrefabs rg in roomPrefabs)
        {
            if (rg.roomGroup == currentRoomGroup)
            {
                roomToSpawn = rg.roomPrefab;
            }
        }
        if (roomToSpawn == null)
        {
            roomToSpawn = roomPrefabs[0].roomPrefab;
        }
        Vector3 roomSpawnPoint;
        roomSpawnPoint = roomSpawnPoints[0].spawnPoint;
        foreach (RoomSpawnPoints rsp in roomSpawnPoints)
        {
            if (rsp.roomGroup == currentRoomGroup)
            {
                roomSpawnPoint = rsp.spawnPoint;
            }
        }

        currentRoomInstance = Instantiate(
            roomToSpawn,
            roomSpawnPoint,
            Quaternion.identity
        );
    }

    private void NextRoom()
    {
        switch (currentRoomGroup)
        {
            case RoomGroup.A:
                currentRoomGroup = RoomGroup.B;
                break;
            case RoomGroup.B:
                currentRoomGroup = RoomGroup.C;
                break;
            case RoomGroup.C:
                currentRoomGroup = RoomGroup.D;
                break;
            case RoomGroup.D:
                currentRoomGroup = RoomGroup.A;
                break;
        }

        previousRoomInstance = currentRoomInstance;
        SpawnRoom();
    }

    // Call this when all enemies in the room are defeated
    public void OnRoomCleared()
    {
        Debug.Log("Room cleared, next");
        NextRoom();
    }

    public void OnRoomEntered()
    {
        Destroy(previousRoomInstance);
    }
}
