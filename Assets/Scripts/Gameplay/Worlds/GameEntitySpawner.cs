using System.Collections.Generic;
using UnityEngine;

public class GameEntitySpawner : MonoBehaviour
{

    // Spawn the initial monsters randomally distributed across the deditated spawn points of a world
    public List<Monster> SpawnInitialMonsters(List<GameObject> spawnPoints, List<MonsterSpawnData> monsterSpawnData, Transform parent, System.Action<Monster> onMonsterSpawned = null)
    {
        List<Monster> spawnedMonsters = new List<Monster>();

        if (spawnPoints == null || spawnPoints.Count == 0 || monsterSpawnData == null)
        {
            Debug.LogWarning("Missing spawn points or monster data.");
            return spawnedMonsters;
        }

        List<GameObject> availablePoints = new List<GameObject>(spawnPoints);

        foreach (var data in monsterSpawnData)
        {
            for (int i = 0; i < data.IntialAmountToSpawn; i++)
            {
                if (availablePoints.Count == 0)
                {
                    Debug.LogWarning("Not enough spawn points.");
                    return spawnedMonsters;
                }

                // Pick a random available spawn point
                int index = Random.Range(0, availablePoints.Count);
                GameObject point = availablePoints[index];

                // Spawn the monster
                GameObject instance = Instantiate(data.MonsterPrefab, point.transform.position, Quaternion.identity, parent);

                if (instance.TryGetComponent<Monster>(out var monster))
                {
                    spawnedMonsters.Add(monster);
                    onMonsterSpawned?.Invoke(monster);
                }
                else
                {
                    Debug.LogError("Spawned object missing Monster component.");
                }

                // Remove the spawn point to avoid reusing it
                availablePoints.RemoveAt(index);
            }
        }

        return spawnedMonsters;
    }

    // Spawn a player in a random deditated spawn point
    public void SpawnPlayers(List<GameObject> playerSpawnPoints, List<GameObject> players, Transform parent)
    {
        if (playerSpawnPoints == null || players == null || playerSpawnPoints.Count == 0)
        {
            Debug.LogWarning("Missing player spawn points or players.");
            return;
        }

        List<GameObject> availablePoints = new List<GameObject>(playerSpawnPoints);

        foreach (var player in players)
        {
            if (availablePoints.Count == 0)
                availablePoints = new List<GameObject>(playerSpawnPoints);

            int index = Random.Range(0, availablePoints.Count);
            GameObject point = availablePoints[index];

            player.transform.SetPositionAndRotation(point.transform.position, point.transform.rotation);
            player.transform.SetParent(parent);
            player.SetActive(true);

            availablePoints.RemoveAt(index);
        }
    }
}
