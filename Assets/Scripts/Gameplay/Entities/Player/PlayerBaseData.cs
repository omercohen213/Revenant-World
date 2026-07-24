using UnityEngine;

[CreateAssetMenu(fileName = "EntityBaseData", menuName = "Scriptable Objects/PlayerBaseData")]
public class PlayerBaseData : EntityBaseData
{
    [Tooltip("Movment speed multiplier when sprinting")]
    public float SprintSpeedMultiplier;

    public int[] XpTable;
}
