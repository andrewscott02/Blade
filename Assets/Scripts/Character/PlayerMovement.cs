using System;
using UnityEngine;

public class PlayerMovement : CharacterMovement
{
    private PlayerRotation _playerRotation;

    [SerializeField]
    private float _maxMoveSpeed = 7f;
    [SerializeField]
    private AnimationCurve _maxAccelerationByCurrentSpeed;
    [SerializeField]
    private float _movementThreshold = 0.5f;
    private float _currentSpeed;

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
        Vector2 movement = Vector2.zero;
        float speedModifier = 0;

        if (input.magnitude > _movementThreshold)
        {
            movement = ConvertInputToDirection(input);
            speedModifier = GetMovementSpeed(movement);
        }
        else
        {
            _currentSpeed = 0;
        }

        MoveCharacter(movement, speedModifier);
        CheckCharacterRotation(movement, currentState);
        AnimateMovement(movement);
    }

    private void MoveCharacter(Vector2 movement, float speedModifier)
    {
        transform.position = ConvertMovementToPosition(movement * speedModifier);
    }

    private float GetMovementSpeed(Vector2 movement)
    {
        float acceleration = _maxAccelerationByCurrentSpeed.Evaluate(_currentSpeed);
        _currentSpeed = Mathf.Clamp((acceleration + _currentSpeed) * movement.magnitude, 0, _maxMoveSpeed);

        return _currentSpeed * Time.deltaTime;
    }

    #region Movement - Calculate Position

    private Vector2 ConvertInputToDirection(Vector2 input)
    {
        float direction = _cameraObject.transform.eulerAngles.y - _camStartRot;
        return input.Rotate(-direction);
    }

    private Vector3 ConvertMovementToPosition(Vector2 movement)
    {
        Vector3 newPos = transform.position;
        newPos.x += movement.x;
        newPos.z += movement.y;
        return newPos;
    }

    #endregion

    #region Movement - Rotation

    private void CheckCharacterRotation(Vector3 movement, CharacterStates currentState)
    {
        switch (currentState)
        {
            case CharacterStates.NonCombat:
                if (movement.magnitude > 0)
                    _playerRotation.RotateBodyFromMovement(movement);
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
        Vector2 moveSpeed = movement * _currentSpeed;

        float animSpeed = _speedToAnimationCurve.Evaluate(moveSpeed.magnitude);
        _animController.SetFloat("MoveSpeed", animSpeed);
    }

    #endregion
}