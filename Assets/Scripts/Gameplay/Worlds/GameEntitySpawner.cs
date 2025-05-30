using UnityEngine;

public class GameEntitySpawner : MonoBehaviour
{
    [SerializeField] private GameObject _monsterPrefab;
    [SerializeField] private GameObject _monstersParent;
    public World World; // Assign via inspector or find at runtime

    private void Start()
    {
        SpawnMonster(transform.position);
    }

    public void SpawnMonster(Vector3 position)
    {
        GameObject monsterInstance = Instantiate(_monsterPrefab, position, Quaternion.identity);
        Monster monster = monsterInstance.GetComponent<Monster>();
        if (World != null && monster != null)
        {
            World.RegisterMonster(monster);
        }
    }
}
