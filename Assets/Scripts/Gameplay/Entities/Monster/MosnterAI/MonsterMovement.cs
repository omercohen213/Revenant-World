using UnityEngine;

public class MonsterMovement : MonoBehaviour
{
    /*public void MoveTowards(MonsterRuntimeContext context, Vector3 destination)
    {
        Vector3 targetPosition = destination;

        context.Transform.position = Vector3.MoveTowards(
            context.Transform.position,
            targetPosition,
            context.Data.MoveSpeed * Time.deltaTime
        );
    }

    public void FaceTarget(MonsterRuntimeContext context, Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - context.Transform.position;

        if (!context.Data.isFlying)
            direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);

        context.Transform.rotation = Quaternion.Slerp(
            context.Transform.rotation,
            targetRotation,
            context.Data.rotationSpeed * Time.deltaTime
        );
    }*/
}