using UnityEngine;

[RequireComponent(typeof(PlayerData))]
[RequireComponent(typeof(PlayerHealth))]
public class Player : Entity
{
    protected override void Start()
    {
        base.Start();
    }

}
