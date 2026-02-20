using UnityEngine;

public class LiquidInertiaRotation : MonoBehaviour
{
    public float drag = 0.92f;  // Замедление (0.9–0.98)
    public float followStrength = 0.15f;  // Как сильно тянется к родителю

    private Quaternion _prevParentRotation;
    private Vector3 _angularVelocity;

    private void Start()
    {
        _prevParentRotation = transform.parent.rotation;
    }

    private void LateUpdate()
    {
        Transform parent = transform.parent;
        if (parent == null)
            return;

        // Угловая скорость родителя
        Quaternion delta = parent.rotation * Quaternion.Inverse(_prevParentRotation);
        delta.ToAngleAxis(out float angle, out Vector3 axis);
        if (angle > 180f)
            angle -= 360f;
        Vector3 parentAngularVel = (axis * angle * Mathf.Deg2Rad) / Time.deltaTime;
        _prevParentRotation = parent.rotation;

        // Наша угловая скорость с инерцией
        _angularVelocity = Vector3.Lerp(_angularVelocity, parentAngularVel, followStrength);
        _angularVelocity *= drag;

        transform.Rotate(_angularVelocity * Mathf.Rad2Deg * Time.deltaTime, Space.World);
    }
}