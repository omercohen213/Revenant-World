using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;

public class ProjectileContext
{
    public Entity Owner { get; }
    public Vector3 Direction { get; }
    public Vector3 ReleasePosition { get; }
    public float Damage { get; }

    public ProjectileContext(Entity owner, Vector3 releasePosition, Vector3 direction, float damage, Vector3 initialVelocity)
    {
        Owner = owner;
        ReleasePosition = releasePosition;
        Direction = direction;
        Damage = damage;
    }
}