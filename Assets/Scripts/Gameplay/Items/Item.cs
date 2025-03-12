using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class Item : MonoBehaviour
{
    public ItemData ItemData;
    protected Player _owner; 
}
