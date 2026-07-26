using UnityEngine;

[CreateAssetMenu(fileName = "EntityBaseData", menuName = "Scriptable Objects/PlayerBaseData")]
public class PlayerBaseData : EntityBaseData
{
    [Header("Movement")]
    public float SpeedChangeRate = 20f;
    public float JumpHeight = 1.5f;
    public float SprintSpeedMultiplier = 2f;

    [Header("Gravity")]
    public float Gravity = -9.81f;

    [Header("Jump")]
    public float JumpTimeout = 0.1f;
    public float FallTimeout = 0.15f;

    [Header("Ground Check")]
    public float GroundedOffset = 0.14f;
    public float GroundedRadius = 0.5f;
    public LayerMask GroundLayers;

    public int[] XpTable;
}
