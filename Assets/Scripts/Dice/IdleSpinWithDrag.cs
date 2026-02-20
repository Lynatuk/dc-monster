using UnityEngine;

public class IdleSpinWithDrag : MonoBehaviour
{
    [Header("Settings")]
    public Vector3 idleEulerSpeed = new Vector3(15f, 30f, 0f);
    public float dragSensitivity = 0.25f;
    public float resumeDelay = 5f;

    [Header("Snap Settings")]
    public float snapSpeed = 540f;
    public float snapStopAngle = 0.2f;

    [Header("Area Restriction")]
    [Tooltip("UI RectTransform that defines the touchable area")]
    public RectTransform interactionRect;

    private Camera _cam;
    private bool _isDragging = false;
    private int _activeFingerId = -1;
    private Vector2 _lastPointerPos;
    private float _lastInteractionTime = -999f;
    private bool _idleEnabled = true;
    private bool _isSnapping = false;
    private Quaternion _snapTarget;
    private Quaternion[] _faceRotations;

    private void Awake()
    {
        _cam = Camera.main;
        _faceRotations = new DiceVisualInfo().faceRotations;
    }

    private void Update()
    {
        HandleInput();

        if (!_isDragging && !_isSnapping && _idleEnabled)
            transform.Rotate(idleEulerSpeed * Time.deltaTime, Space.Self);

        if (!_isDragging && !_isSnapping && !_idleEnabled && Time.time - _lastInteractionTime >= resumeDelay)
            _idleEnabled = true;

        if (_isSnapping)
            ContinueSnap();
    }

    private void HandleInput()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0) && IsInInteractionArea(Input.mousePosition))
            BeginDrag(Input.mousePosition);

        if (Input.GetMouseButton(0) && _isDragging)
            Drag(Input.mousePosition);

        if (Input.GetMouseButtonUp(0) && _isDragging)
            EndDrag();
#endif

        if (Input.touchCount > 0)
        {
            if (_isDragging)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    var t = Input.GetTouch(i);
                    if (t.fingerId != _activeFingerId)
                        continue;

                    if (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary)
                        Drag(t.position);
                    else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
                        EndDrag();
                }
            }
            else
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    var t = Input.GetTouch(i);
                    if (t.phase != TouchPhase.Began)
                        continue;

                    // Check if touch started inside designated area
                    if (!IsInInteractionArea(t.position))
                        continue;

                    _activeFingerId = t.fingerId;
                    BeginDrag(t.position);
                    break;
                }
            }
        }
    }

    private bool IsInInteractionArea(Vector2 screenPos)
    {
        if (interactionRect == null)
            return true; // Full screen if no area assigned

        // Check if screen point is inside RectTransform
        return RectTransformUtility.RectangleContainsScreenPoint(interactionRect, screenPos, null);
    }

    private void BeginDrag(Vector2 screenPos)
    {
        _isDragging = true;
        _isSnapping = false;
        _idleEnabled = false;
        _lastPointerPos = screenPos;
        _lastInteractionTime = Time.time;
    }

    private void Drag(Vector2 screenPos)
    {
        Vector2 delta = screenPos - _lastPointerPos;
        _lastPointerPos = screenPos;

        float yaw = -delta.x * dragSensitivity;
        transform.Rotate(Vector3.up, yaw, Space.World);

        float pitch = delta.y * dragSensitivity;
        Vector3 rightAxis = _cam ? _cam.transform.right : Vector3.right;
        transform.Rotate(rightAxis, pitch, Space.World);

        _lastInteractionTime = Time.time;
    }

    private void EndDrag()
    {
        _isDragging = false;
        _activeFingerId = -1;
        _lastInteractionTime = Time.time;

        if (_faceRotations != null && _faceRotations.Length > 0)
            StartSnapToNearestFace();
    }

    private void StartSnapToNearestFace()
    {
        _idleEnabled = false;
        _isSnapping = true;
        _snapTarget = FindNearestFaceRotation();
    }

    private void ContinueSnap()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, _snapTarget, snapSpeed * Time.deltaTime);

        if (Quaternion.Angle(transform.rotation, _snapTarget) <= snapStopAngle)
        {
            transform.localRotation = _snapTarget;
            _isSnapping = false;
            _lastInteractionTime = Time.time;
        }
    }

    private Quaternion FindNearestFaceRotation()
    {
        Quaternion current = transform.rotation;
        float best = float.MaxValue;
        Quaternion bestQ = current;

        for (int i = 0; i < _faceRotations.Length; i++)
        {
            float angle = Quaternion.Angle(current, _faceRotations[i]);
            if (angle < best)
            {
                best = angle;
                bestQ = _faceRotations[i];
            }
        }
        return bestQ;
    }
}