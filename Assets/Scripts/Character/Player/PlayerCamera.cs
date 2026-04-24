using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    private Vector2 _rotateModifiers;

    [SerializeField]
    private Vector2 _rotationBounds = new(330, 42);

    [SerializeField]
    private GameObject _cameraLookTarget;
    [SerializeField]
    private float _camXRotOffet = 50;
    private const float _camStartRot = 180;

    internal void RotateCam(Vector2 input)
    {
        _cameraLookTarget.transform.rotation = GetNewRotation(input);
    }

    private Quaternion GetNewRotation(Vector2 input)
    {
        Vector3 camRot = _cameraLookTarget.transform.rotation.eulerAngles;

        camRot.x += input.y * _rotateModifiers.x;
        camRot.y += input.x * _rotateModifiers.y;

        camRot.x = ClampVerticalRotation(camRot.x);

        return Quaternion.Euler(camRot);
    }

    private float ClampVerticalRotation(float rotation)
        => Mathf.Clamp(rotation, _rotationBounds.x, _rotationBounds.y);

    internal void LookAtTarget(LockOnTarget currentTarget)
    {
        Vector3 rot = Quaternion.LookRotation(currentTarget.transform.position - transform.position).eulerAngles;
        rot.x += _camXRotOffet;
        rot.y += _camStartRot;
        _cameraLookTarget.transform.rotation = Quaternion.Euler(rot);
    }
}