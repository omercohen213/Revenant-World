using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private float _defaultFov = 60f;

    public CinemachineCamera Camera;
    private Coroutine _fovCoroutine;


    void Start()
    {
        Camera = GetComponentInChildren<CinemachineCamera>();
        SetFov(_defaultFov);
    }

    public void StartFovTransition(float targetFov, float duration)
    {
        if (_fovCoroutine != null)
        {
            StopCoroutine(_fovCoroutine);
        }
        _fovCoroutine = StartCoroutine(UpdateFovCoroutine(targetFov, duration));
    }

    private IEnumerator UpdateFovCoroutine(float targetFov, float duration)
    {
        float startFov = Camera.Lens.FieldOfView;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration; // Normalized progress 0-1
            SetFov(Mathf.Lerp(startFov, targetFov, t));
            yield return null;
        }

        SetFov(targetFov);
    }

    private void SetFov(float fov)
    {
        Camera.Lens.FieldOfView = fov;
    }
}
