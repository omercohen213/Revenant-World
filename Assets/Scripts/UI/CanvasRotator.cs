using Unity.Cinemachine;
using UnityEngine;

public class CanvasRotator : MonoBehaviour
{
    [SerializeField] private CinemachineCamera playerCamera;

    void Update()
    {
        if (playerCamera != null)
        {
            // Rotate the canvas to face the local player's camera
            transform.LookAt(transform.position + playerCamera.transform.rotation * Vector3.forward,
                             playerCamera.transform.rotation * Vector3.up);
        }
    }
}
