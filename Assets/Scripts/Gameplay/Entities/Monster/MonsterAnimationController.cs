using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MonsterAnimationController : MonoBehaviour
{
    [SerializeField] private Transform _rootTransform;
    private Animator _animator;

    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Move = Animator.StringToHash("Move");

    private float _rootMotionMultiplier = 1f;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        PlayIdle();
    }

    public void PlayIdle()
    {
        _animator.CrossFade(Idle, 0.1f);
    }

    public void PlayMove()
    {
        _animator.CrossFade(Move, 0.1f);
    }

    public void TriggerAbility(string animationParameter)
    {
        int hash = Animator.StringToHash(animationParameter);
        _animator.SetTrigger(hash);
    }

    public void SetMovementSpeed(float speed)
    {
        _animator.SetFloat(Speed, speed);
    }


    public void SetRootMotionMultiplier(float multiplier)
    {
        _rootMotionMultiplier = multiplier;
    }

    public float GetAnimationLength(string animationName)
    {
        RuntimeAnimatorController controller = _animator.runtimeAnimatorController;

        foreach (AnimationClip clip in controller.animationClips)
        {
            if (clip.name == animationName)
            {
                return clip.length;
            }
        }

        Debug.LogWarning($"Animation clip {animationName} not found");
        return 0f;
    }

    public void SetAnimationSpeed(float speed)
    {
        _animator.speed = speed;
    }

    private void OnAnimatorMove()
    {
        Vector3 delta = _animator.deltaPosition;

        delta *= _rootMotionMultiplier;

        _rootTransform.position += delta;

        _rootTransform.rotation *= _animator.deltaRotation;
    }

    public void StopAbilityAnimation()
    {
        PlayIdle();
    }
}
