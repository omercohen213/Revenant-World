using UnityEngine;

public class BiteAttack : IMonsterAttack
{
    private readonly MonsterBrain _brain;
    private readonly MonsterAnimationController _animationController;
    private readonly MonsterCombat _combat;
    private readonly BiteAttackData _data;

    private float _timer;
    private bool _finished;

    public bool Finished => _finished;

    public BiteAttack(
        MonsterBrain brain,
        MonsterCombat combat,
        BiteAttackData data,
        MonsterAnimationController animationController)
    {
        _brain = brain;
        _combat = combat;
        _data = data;
        _animationController = animationController;
    }

    public bool CanUse()
    {
        float distance = _brain.DistanceToTarget;

        return distance <= _data.Range;
    }


    public void Begin()
    {
        Debug.Log("Bite started");

        _timer = 0;
        _finished = false;

        _animationController.SetRootMotionMultiplier(3f);
        _animationController.PlayBiteAttack();
    }


    public void Tick()
    {
        _timer += Time.deltaTime;


        if (_timer >= 0.5f)
        {
            DealDamage();
        }


        if (_timer >= 1f)
        {
            _finished = true;
        }
    }


    private void DealDamage()
    {
        Debug.Log("Bite damage");
    }


    public void End()
    {
        _animationController.SetRootMotionMultiplier(1f);
        Debug.Log("Bite finished");
    }
}