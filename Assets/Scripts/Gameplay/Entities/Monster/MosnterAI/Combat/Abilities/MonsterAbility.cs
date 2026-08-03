using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public abstract class MonsterAbility<TData> : IMonsterAbility where TData : MonsterAbilityData
{
    protected readonly MonsterAbilitiyContext _context;
    protected readonly TData _data;

    protected float _nextUseTime;
    public bool IsOnCooldown => Time.time < _nextUseTime;
    public bool IsFinished { get; private set; }

    protected MonsterAbility(MonsterAbilitiyContext context, TData data)
    {
        _context = context;
        _data = data;
    }

    public virtual bool CanUse()
    {
        return !IsOnCooldown && IsInRange();
    }

    public virtual bool IsInRange()
    {
        Vector3 ownerPosition = _context.Owner.transform.position;
        Vector3 targetPosition = _context.TargetContext.CurrentPosition;
        float distance = Vector3.Distance(ownerPosition, targetPosition);
        return distance <= _data.Range;
    }

    public virtual void Begin()
    {
        IsFinished = false;
        StartCooldown();
    }

    protected void StartCooldown()
    {
        _nextUseTime = Time.time + _data.BaseCooldown;
    }

    public virtual void Tick()
    {
    }

    public virtual void Cancel()
    {
        MarkFinished();
    }


    public virtual void End()
    {
        MarkFinished();
    }

    protected void MarkFinished()
    {
        IsFinished = true;
    }
}
