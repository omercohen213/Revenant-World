using UnityEngine;

public class PatrolArea : MonoBehaviour
{
    [SerializeField] private float _radius;

    // Get a random point around the tranform inside a circle
    public Vector3 GetRandomPoint()
    {
        Vector2 rnd = Random.insideUnitCircle * _radius;
        return transform.position + new Vector3 (rnd.x, 0, rnd.y); 
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
#endif
}
