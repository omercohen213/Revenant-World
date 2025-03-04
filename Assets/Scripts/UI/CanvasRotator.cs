using Unity.Cinemachine;
using UnityEngine;

public class CanvasRotator : MonoBehaviour
{
    [SerializeField] private float _visibilityDistance = 10f; // Distance at which the health bar becomes visable

    private Transform _targetPlayer;
    private CinemachineCamera _targetPlayerCamera;
    private GameObject _barsParent;

    private void Start()
    {
        GameObject targetPlayerGameObject = GameObject.FindGameObjectWithTag("Player");
        _targetPlayer = targetPlayerGameObject.transform;
        _barsParent = transform.Find("Bars").gameObject;
    }

    private void Update()
    {
        if (_targetPlayer != null)
        {
            float distance = Vector3.Distance(transform.position, _targetPlayer.position);

            // Show/hide the health bar based on distance
            bool isVisible = distance <= _visibilityDistance;
            if (isVisible)
            {

                _barsParent.SetActive(true);
            }
            else
            {
                _barsParent.SetActive(false);
            }

            _targetPlayerCamera = _targetPlayer.GetComponentInChildren<CinemachineCamera>();

            // Rotate the canvas to face the local player's camera
            if (_targetPlayerCamera != null)
            {
                transform.LookAt(transform.position + _targetPlayerCamera.transform.rotation * Vector3.forward,
                                 _targetPlayerCamera.transform.rotation * Vector3.up);
            }
        }
    }
   
}
