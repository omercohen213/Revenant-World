using UnityEngine;

[CreateAssetMenu(fileName = "EntityBaseData", menuName = "Scriptable Objects/PlayerBaseData")]
public class PlayerBaseData : EntityBaseData
{
    [Tooltip("XP required for each level")]
    public int[] XpTable;
}
