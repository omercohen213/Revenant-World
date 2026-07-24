using System;
using UnityEngine;

[RequireComponent (typeof(Animator))]
public class MonsterAnimationController : MonoBehaviour
{
    private Animator _animator;

    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Move = Animator.StringToHash("Move");
    private static readonly int Attack = Animator.StringToHash("Attack");

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

    public void PlayAttack()
    {
        _animator.CrossFade(Attack, 0.05f);
    }

    public void SetMovementSpeed(float speed)
    {
        _animator.SetFloat(Speed, speed);
    }
}
