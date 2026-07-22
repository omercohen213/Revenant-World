using UnityEngine;

// Responsible for every aspect of the combat behaviour of an entity
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
