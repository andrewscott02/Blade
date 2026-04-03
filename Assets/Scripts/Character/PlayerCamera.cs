using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    private Vector2 _rotateModifiers;

    [SerializeField]
    private Vector2 _rotationBounds = new(330, 42);

    [SerializeField]
    private GameObject _cameraLookTarget;

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
    //{
    //    if (rotation < _rotationBounds.x && rotation > _rotationBounds.y)
    //        return rotation;

    //    float xDiff = rotation - _rotationBounds.x;
    //    float yDiff = rotation - _rotationBounds.y;

    //    if (xDiff > yDiff)
    //        return yDiff;
    //    return xDiff;
    //}
        => Mathf.Clamp(rotation, _rotationBounds.x, _rotationBounds.y);
}