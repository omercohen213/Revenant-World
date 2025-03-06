using System;
using Unity.Cinemachine;
using UnityEngine;

public class CanvasVisabilityManager : MonoBehaviour
{
    [SerializeField] private float _visibilityDistance = 10f; // Distance at which the health bar becomes visable

    private Transform _targetPlayer;
    private CinemachineCamera _targetPlayerCamera;
    [SerializeField] private GameObject _barsParent;

    private void Start()
    {
        GameObject targetPlayerGameObject = GameObject.FindGameObjectWithTag("Player");
        _targetPlayer = targetPlayerGameObject.transform;
    }

    private void Update()
    {
        if (_targetPlayer != null)
        {
            HandleBarVisability();
            RotateCanvas();          
        }
    }   

    // Show/hide the health bar based on distance
    private void HandleBarVisability()
    {
        float distance = Vector3.Distance(transform.position, _targetPlayer.position);

        bool isVisible = distance <= _visibilityDistance;
        if (isVisible)
        {

            _barsParent.SetActive(true);
        }
        else
        {
            _barsParent.SetActive(false);
        }
    }

    // Rotate the canvas to face the local player's camera
    private void RotateCanvas()
    {
        _targetPlayerCamera = _targetPlayer.GetComponentInChildren<CinemachineCamera>();

        if (_targetPlayerCamera != null)
        {
            transform.LookAt(transform.position + _targetPlayerCamera.transform.rotation * Vector3.forward,
                             _targetPlayerCamera.transform.rotation * Vector3.up);
        }
    }
}
