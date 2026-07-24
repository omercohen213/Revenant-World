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

    [SerializeField] private GameEntitySpawner _entitySpawner;
    [SerializeField] private Transform _monstersParent;
    [SerializeField] private List<GameObject> _monsterSpawnPoints;

    [SerializeField] public List<Monster> _monstersAlive;
    [SerializeField] private List<Portal> _portals;

    public UnityAction<Entity> OnMonsterKilled;

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
        RegisterExistingMonsters();
    }

    private void SetupWorld()
    {
        if (WorldData.Objective != null)
        {
            WorldData.Objective.Initialize();
        }

        // Spawn initial monsters and register them
        var monsters = _entitySpawner.SpawnInitialMonsters(_monsterSpawnPoints, WorldData.MonsterSpawnData, _monstersParent, RegisterMonster);
        _monstersAlive.AddRange(monsters);
    }

    // **************** TEMPORARY SOLUTION. REGISTER EVERY MONSTER ON ENTITY ***************
    private void RegisterExistingMonsters()
    {
        var monsters = FindObjectsOfType<Monster>();

        foreach (var monster in monsters)
        {
            RegisterMonster(monster);
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
    public void HandleMonsterKilled(Entity monster, GameObject killer)
    {
        monster.OnKilled -= HandleMonsterKilled;

        if (WorldData.Objective is KillObjective killObjective)
        {
            _monstersAlive.Remove((Monster)monster);
            killObjective.RegisterKill();
            OnMonsterKilled?.Invoke(monster);

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
