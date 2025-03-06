using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class World : MonoBehaviour
{
    public WorldData WorldData;
    [SerializeField] private List<GameObject> _monsterSpawnPoints;
    [SerializeField] private List<GameObject> _playerSpawnPoints;
    [SerializeField] public List<Monster> _monstersAlive;
    [SerializeField] private List<Portal> _portals;

    public UnityAction<GameEntity> OnMonsterKilled;

    private void Awake()
    {
        _monstersAlive = new List<Monster>();
    }

    private void OnEnable()
    {
        foreach (var monster in _monstersAlive)
        {
            monster.OnKilled += HandleMonsterKilled;
        }
        WorldData.Objective.OnObjectiveCompleted += HandleObjectiveCompleted;
    }

    private void OnDisable()
    {
        foreach (var monster in _monstersAlive)
        {
            monster.OnKilled -= HandleMonsterKilled;
        }
        WorldData.Objective.OnObjectiveCompleted -= HandleObjectiveCompleted;
    }

    private void Start()
    {
        SetupWorld();
        SpawnInitialMonsters();
    }
    private void SetupWorld()
    {
        if (WorldData.Objective != null)
        {
            WorldData.Objective.Initialize();
        }
    }

    // Spawn the initial monsters randomally distributed across the deditated spawn points
    private void SpawnInitialMonsters()
    {
        if (WorldData == null || WorldData.MonsterSpawnData == null || _monsterSpawnPoints.Count == 0)
        {
            Debug.LogWarning("WorldData or MonsterSpawnData is missing, or no spawn points available.");
            return;
        }

        List<GameObject> availableSpawnPoints = new List<GameObject>(_monsterSpawnPoints);

        foreach (var monsterData in WorldData.MonsterSpawnData)
        {
            for (int i = 0; i < monsterData.IntialAmountToSpawn; i++)
            {
                if (availableSpawnPoints.Count == 0)
                {
                    Debug.LogWarning("Not enough spawn points available for all monsters.");
                    return;
                }

                // Pick a random available spawn point
                int spawnIndex = Random.Range(0, availableSpawnPoints.Count);
                GameObject spawnPoint = availableSpawnPoints[spawnIndex];

                // Spawn the monster
                GameObject monsterInstance = Instantiate(monsterData.MonsterPrefab, spawnPoint.transform.position, Quaternion.identity);

                if (monsterInstance.TryGetComponent<Monster>(out var monsterComponent))
                {
                    RegisterMonster(monsterComponent);
                }
                else
                {
                    Debug.LogError($"Spawned object {monsterInstance.name} does not have a Monster component.");
                }

                // Remove the spawn point to avoid reusing it
                availableSpawnPoints.RemoveAt(spawnIndex);
            }
        }
    }


    // Spawn a player in a random deditated spawn point
    private void SpawnPlayer(List<GameObject> players)
    {
        if (players == null || players.Count == 0 || _playerSpawnPoints.Count == 0)
        {
            Debug.LogWarning("No players to spawn or no spawn points available.");
            return;
        }

        List<GameObject> availableSpawnPoints = new List<GameObject>(_playerSpawnPoints);

        foreach (var player in players)
        {
            if (availableSpawnPoints.Count == 0)
            {
                Debug.LogWarning("Not enough unique spawn points available; some players may share a location.");
                availableSpawnPoints = new List<GameObject>(_playerSpawnPoints); // Reset spawn points to allow reuse.
            }

            int spawnIndex = Random.Range(0, availableSpawnPoints.Count);
            GameObject spawnPoint = availableSpawnPoints[spawnIndex];

            // Spawn player at the chosen spawn point
            player.transform.position = spawnPoint.transform.position;
            player.transform.rotation = spawnPoint.transform.rotation;
            player.SetActive(true);

            // Remove the spawn point from the list to avoid reuse (unless all are used)
            availableSpawnPoints.RemoveAt(spawnIndex);
        }
    }

    public void RegisterMonster(Monster monster)
    {
        if (monster != null)
        {
            _monstersAlive.Add(monster);
            monster.OnKilled += HandleMonsterKilled;
        }
    }

    // Register the kill in the world and in the objective
    public void HandleMonsterKilled(GameEntity monster, GameObject killer)
    {
        monster.OnKilled -= HandleMonsterKilled;

        if (WorldData.Objective is KillObjective killObjective)
        {
            _monstersAlive.Remove((Monster)monster);
            killObjective.RegisterKill();
            OnMonsterKilled.Invoke(monster);

            Debug.Log($"{killer} killed {monster}");
        }
    }

    private void HandleObjectiveCompleted()
    {
        ActivatePortals();
    }

    // Activate portals in the environment
    private void ActivatePortals()
    {
        foreach (Portal portal in _portals)
        {
            portal.Activate();
        }
    }
}
