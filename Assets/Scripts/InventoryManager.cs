using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public Weapon StartingWeapon;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Weapon GetActiveWeapon()
    {
        return StartingWeapon;
    }
}
