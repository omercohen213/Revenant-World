using UnityEngine;

[CreateAssetMenu(fileName = "EntityBaseData", menuName = "Scriptable Objects/PlayerBaseData")]
public class PlayerBaseData : EntityBaseData
{
    [Tooltip("Movment speed multiplyer when sprinting")]
    public float SprintSpeedMultiplyer;

    public int[] XpTable;
}
