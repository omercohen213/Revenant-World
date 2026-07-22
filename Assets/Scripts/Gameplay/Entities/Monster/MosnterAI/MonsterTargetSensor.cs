using UnityEngine;

public class MonsterTargetSensor
{
    private readonly LayerMask _playerLayer;

    public MonsterTargetSensor(LayerMask playerLayer)
    {
        _playerLayer = playerLayer;
    }

    public Transform FindClosestTarget(Vector3 position, float range)
    {
        Collider[] hits = Physics.OverlapSphere(position, range, _playerLayer);

        if (hits == null || hits.Length == 0)
            return null;

        Transform closestTarget = null;
        float closestDistance = float.MaxValue;

        foreach (Collider hit in hits)
        {
            float distance = Vector3.Distance(position, hit.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = hit.transform;
            }
        }

        return closestTarget;
    }
}