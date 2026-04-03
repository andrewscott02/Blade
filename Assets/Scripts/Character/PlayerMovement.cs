using UnityEngine;

public class PlayerMovement : CharacterMovement
{
    private PlayerRotation _playerRotation;

    [SerializeField]
    private float _speedModifier = 0.01f;

    [SerializeField]
    private GameObject _cameraObject;
    private const float _camStartRot = 180;

    [SerializeField]
    private GameObject _characterBody;

    [Header("Animation")]
    [SerializeField]
    private Animator _animController;

    [SerializeField]
    private AnimationCurve _speedToAnimationCurve;

    private void Awake()
    {
        _playerRotation = new(_characterBody);
    }

    public void Move(Vector2 input, CharacterStates currentState)
    {
        Vector2 movement = ConvertInputToDirection(input) * _speedModifier;

        transform.position = ConvertDirectionToPosition(movement);
        CheckCharacterRotation(movement, currentState);
        AnimateMovement(movement);
    }

    #region Movement - Calculate Position

    private Vector2 ConvertInputToDirection(Vector2 input)
    {
        float direction = _cameraObject.transform.eulerAngles.y - _camStartRot;
        return input.Rotate(-direction);
    }

    private Vector3 ConvertDirectionToPosition(Vector2 movement)
    {
        Vector3 newPos = transform.position;
        newPos.x += movement.x;
        newPos.z += movement.y;
        return newPos;
    }

    #endregion

    #region Movement - Rotation

    private void CheckCharacterRotation(Vector2 movement, CharacterStates currentState)
    {
        switch (currentState)
        {
            case CharacterStates.NonCombat:
                _playerRotation.RotateBodyFromMovment(movement);
                break;
            case CharacterStates.Combat:
                break;
            case CharacterStates.Dead:
                break;
            default:
                throw new System.Exception("State not implemented");
        }
    }

    #endregion

    #region Movement - Animation

    private void AnimateMovement(Vector2 movement)
    {
        float animSpeed = _speedToAnimationCurve.Evaluate(movement.magnitude);
        _animController.SetFloat("MoveSpeed", animSpeed);
    }

    #endregion
}