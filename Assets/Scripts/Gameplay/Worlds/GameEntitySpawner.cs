using UnityEngine;

public class GameEntitySpawner : MonoBehaviour
{
    public GameObject MonsterPrefab;
    public World World; // Assign via inspector or find at runtime

    public void SpawnMonster(Vector3 position)
    {
        GameObject monsterInstance = Instantiate(MonsterPrefab, position, Quaternion.identity);
        Monster monster = monsterInstance.GetComponent<Monster>();
        if (World != null && monster != null)
        {
            World.RegisterMonster(monster);
        }
    }
}
