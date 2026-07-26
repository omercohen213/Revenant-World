using System;
using UnityEngine;

[RequireComponent (typeof(Animator))]
public class MonsterAnimationController : MonoBehaviour
{
    private Animator _animator;

    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Move = Animator.StringToHash("Move");
    private static readonly int BiteAttack = Animator.StringToHash("Bite");
    private static readonly int FireballAttack = Animator.StringToHash("Fireball");

    private float _rootMotionMultiplier = 1f;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void PlayIdle()
    {
        _animator.CrossFade(Idle, 0.1f);
    }

    public void PlayMove()
    {
        _animator.CrossFade(Move, 0.1f);
    }

    public void PlayBiteAttack()
    {
        _animator.SetTrigger(BiteAttack);
    }

    public void PlayFireballAttack()
    {
        _animator.SetTrigger(FireballAttack);
    }

    public void SetMovementSpeed(float speed)
    {
        _animator.SetFloat(Speed, speed);
    }

    private void OnAnimatorMove()
    {
        transform.position += _animator.deltaPosition * _rootMotionMultiplier;
        transform.rotation *= _animator.deltaRotation;
    }

    public void SetRootMotionMultiplier(float multiplier)
    {
        _rootMotionMultiplier = multiplier;
    }
}
