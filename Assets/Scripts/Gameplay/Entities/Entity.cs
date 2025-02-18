using UnityEngine;

[RequireComponent(typeof(Health))]          
public abstract class Entity : MonoBehaviour
{
    protected EntityData _entityData;
    protected Health _health;

    protected virtual void Start()
    {
        _entityData = GetComponent<EntityData>();
        _health = GetComponent<Health>();
    }
}
