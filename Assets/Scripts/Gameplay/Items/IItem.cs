using UnityEngine;

public interface IItem
{
    void PickUp();
    void Drop();
    void SetOwner(Player owner);
}
