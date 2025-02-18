using UnityEngine;


public abstract class Weapon : MonoBehaviour
{
    [Header("Information")]
    public Entity Owner;

    [Header("Weapon Data")]
    [Tooltip("The root object for the weapon, this is what will be deactivated when the weapon isn't active")]
    public GameObject WeaponRoot;
}
