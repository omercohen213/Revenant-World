using UnityEngine;

public abstract class AnimationDrivenAbility<TData> : MonsterAbility<TData>, IAnimationDrivenAbility where TData : MonsterAbilityData
{
    protected AnimationDrivenAbility(MonsterAbilitiyContext context, TData data) : base(context, data)
    {
    }

    public override void Begin()
    {
        base.Begin();
        PlayAnimation();
    }

    protected virtual void PlayAnimation()
    {
        // Speed up or slow down the animation so it matches the cooldown.
        if (_data.BaseCooldown > 0f)
        {
            float animationLength = _context.AnimController.GetAnimationLength(_data.AnimationParameter);
            float speed = animationLength / _data.BaseCooldown;
            _context.AnimController.SetAnimationSpeed(speed);
        }

        _context.AnimController.TriggerAbility(_data.AnimationParameter);
    }

    public override void Cancel()
    {
        base.Cancel();

        _context.AnimController.SetAnimationSpeed(1f);
        _context.AnimController.StopAbilityAnimation();
    }

    public override void End()
    {
        base.End();
        _context.AnimController.SetAnimationSpeed(1f);
    }


    public virtual void OnAnimationEvent(AbilityAnimationEvent eventType)
    {
        if (eventType == AbilityAnimationEvent.Finished)
        {
            MarkFinished();
        }
    }
}
