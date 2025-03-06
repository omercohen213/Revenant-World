using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWorldData", menuName = "Game/WorldData")]
public class WorldData : ScriptableObject
{
    [Header("Basic Info")]
    public string WorldName;

    [Header("Objective")]
    public Objective Objective;

    [Header("Monster Settings")]
    public List<MonsterSpawnData> MonsterSpawnData;
    public int MaxMonsterAmount;


    [Header("World Environment")]
    public GameObject EnvironmentPrefab;
}

[Serializable]
public struct MonsterSpawnData
{
    public GameObject MonsterPrefab;
    public int IntialAmountToSpawn;
}