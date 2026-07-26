using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class FireballAttack : IMonsterAttack
{
    private readonly MonsterBrain _brain;
    private readonly MonsterAnimationController _animationController;
    private readonly MonsterCombat _combat;
    private readonly FireballAttackData _data;

    private float _timer;
    private bool _spawned;
    private bool _finished;
    public bool Finished => _finished;

    public FireballAttack(
        MonsterBrain brain,
        MonsterCombat combat,
        FireballAttackData data,
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
        Debug.Log("Fireball started");

        _timer = 0;
        _spawned = false;
        _finished = false;

        _animationController.PlayFireballAttack();
    }


    public void Tick()
    {
        _timer += Time.deltaTime;


        if (_timer >= 0.7f && !_spawned)
        {
            SpawnFireball();
            _spawned = true;
        }


        if (_timer >= 1.5f)
        {
            _finished = true;
        }
    }


    private void SpawnFireball()
    {
        Debug.Log("Spawn fireball projectile");
    }


    public void End()
    {
        Debug.Log("Fireball finished");
    }
}