using UnityEngine;

public interface IWeapon : IItem
{
    void Equip();
    void Unequip();
    void HandleActions();
}
