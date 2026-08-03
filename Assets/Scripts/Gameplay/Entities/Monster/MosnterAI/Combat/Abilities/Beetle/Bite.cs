using UnityEngine;

public class Bite : AnimationDrivenAbility<BiteAbilityData>
{
    public Bite(MonsterAbilitiyContext context, BiteAbilityData data) : base(context, data)
    {
    }

    public override void Begin()
    {
        base.Begin();
        float multiplier = _data.Range / _data.AttackMovementDistance;

        _context.AnimController.SetRootMotionMultiplier(multiplier);

        _context.AnimController.TriggerAbility(_data.AnimationParameter);
    }

    public override void End()
    {
        _context.AnimController.SetRootMotionMultiplier(1f);
    }

    public override void OnAnimationEvent(AbilityAnimationEvent eventType)
    {
        base.OnAnimationEvent(eventType);   
        switch (eventType)
        {
            case AbilityAnimationEvent.HitboxStart:
                // enable hitbox
                break;

            case AbilityAnimationEvent.HitboxEnd:
                // disable hitbox
                break;
        }
    }

    public override void Cancel()
    {
        base.Cancel();  
        //disable hitbox
    }
}