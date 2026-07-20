using TMPro;
using UnityEngine;

public class EntityMovement : MonoBehaviour
{
    protected CharacterController _controller;
    protected Vector2 _currentMovement;
    protected float _verticalVelocity; // for gravity

    [SerializeField] protected float _gravity = -9.81f;

    protected virtual void Move(Vector3 direction, float speed)
    {
        Vector3 movement = direction * speed;
        movement.y = _verticalVelocity;

        // Use FixedDeltaTime for consistent movement in FixedUpdate.
        _controller.Move(movement * Time.deltaTime);
    }

    protected virtual void ApplyGravity()
    {
        if (_controller.isGrounded && _verticalVelocity < 0)
        {
            _verticalVelocity = -2f;
        }

        _verticalVelocity += _gravity * Time.deltaTime;
    }

    protected virtual void Stop()
    {
        
    }
}
