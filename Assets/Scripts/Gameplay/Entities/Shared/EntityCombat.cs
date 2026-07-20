using UnityEngine;

public class EntityCombat : MonoBehaviour
{
    protected virtual bool CanAttack()
    {
        return true;

            /*!IsDead &&
            !IsStunned &&
            !IsAttacking;*/
    }
}
