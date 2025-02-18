using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class CameraControl : MonoBehaviour
{
    public Transform aimTarget;
    [FormerlySerializedAs("m_CameraPositions")] public Transform[] _cameraPositions;
    [FormerlySerializedAs("m_AimPositions")] public Transform[] _aimPositions;
    public float[] _fieldOfViews;
    [FormerlySerializedAs("_transitionTime")] [FormerlySerializedAs("m_TransitionTime")] [FormerlySerializedAs("m_SmoothTime")] [Min(0)]
    public float transitionTime = 1.0f;
    
    // REMOVE BEFORE RELEASE
    public bool DrawGizmos = false;
    public bool EditMode = false;
    [Range(0, 3)]
    public int currentIndex = 0;

    private Vector3 _targetPos;
    private Vector3 _aimTargetPos;
    private float _fieldOfView;

    private Camera _cam;
    //private float _camFieldOfView;
    private int _targetIndex = 0;
    private Transform _camera;
    private Vector3 _velocity = Vector3.zero;
    private Vector3 _aimVelocity = Vector3.zero;
    
    private static float _filedOfViewTransitionTime = 0.0f;
    
    void Start()
    {
        _targetPos = _cameraPositions[0].position;
        _aimTargetPos = _aimPositions[0].position;
        _fieldOfView = _fieldOfViews[0];
        _camera = Camera.main.transform;
        //_camFieldOfView = Camera.main.fieldOfView;
    }

    void Update()
    {
        if (EditMode)
        {
            _targetPos = _cameraPositions[currentIndex].position;
            _aimTargetPos = _aimPositions[currentIndex].position;
            
            _camera.position = Vector3.SmoothDamp(_camera.position, _targetPos, ref _velocity, transitionTime);
            aimTarget.position = Vector3.SmoothDamp(aimTarget.position, _aimTargetPos, ref _aimVelocity, transitionTime);
            Camera.main.fieldOfView = _fieldOfViews[currentIndex];
        }

        else if ((_targetPos - _camera.position).sqrMagnitude > 0.00001f && !EditMode)
        {
            _camera.position = Vector3.SmoothDamp(_camera.position, _targetPos, ref _velocity, transitionTime);
            aimTarget.position = Vector3.SmoothDamp(aimTarget.position, _aimTargetPos, ref _aimVelocity, transitionTime);
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, _fieldOfView, _filedOfViewTransitionTime += transitionTime * Time.deltaTime);
        }
    }
    
    public void UpdateCamera()
    {
        _targetIndex = (_targetIndex + 1) % _cameraPositions.Length;
        _targetPos = _cameraPositions[_targetIndex].position;
        _aimTargetPos = _aimPositions[_targetIndex].position;
        _fieldOfView = _fieldOfViews[_targetIndex];
        _filedOfViewTransitionTime = 0.0f;
        
    }
    
    void OnDrawGizmos()
    {
        if (!DrawGizmos)
            return;
        foreach (var t in _cameraPositions)
        {
            // Draw a yellow sphere at the transform's position
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(t.position, 0.02f);
        }
        
        foreach (var t in _aimPositions)
        {
            // Draw a yellow sphere at the transform's position
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(t.position, 0.02f);
        }
        
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_camera.position, 0.02f);
        
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(aimTarget.position, 0.01f);
        
    }
}
