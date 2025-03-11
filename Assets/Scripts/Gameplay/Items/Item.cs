using UnityEngine;

public abstract class Item : MonoBehaviour, IItem
{
    [HideInInspector] public ItemData data;

    public void Drop()
    {
        throw new System.NotImplementedException();
    }

    public void PickUp()
    {
        throw new System.NotImplementedException();
    }

    public void SetOwner(Player owner)
    {
        throw new System.NotImplementedException();
    }
}
