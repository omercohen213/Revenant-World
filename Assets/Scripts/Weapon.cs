using DevionGames.InventorySystem;
using GLTF.Schema;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public abstract class Weapon : MonoBehaviour
{
    [Header("Information")]
    public Entity Owner;
}
