using UnityEngine;

public class PlayerMovement : CharacterMovement
{
    [SerializeField]
    private GameObject _characterContainer;

    private PlayerRotation _playerRotation;

    [SerializeField]
    private float _minMoveSpeed = 0.8f;
    [SerializeField]
    private float _maxMoveSpeed = 7f;
    [SerializeField]
    private float _maxSprintSpeed = 9f;
    [SerializeField]
    private float _maxCombatSpeed = 3.5f;
    [SerializeField]
    private AnimationCurve _maxAccelerationByCurrentSpeed;

    [SerializeField]
    private float _inputThreshold = 0.25f;
    private float _currentSpeed;
    private bool _sprinting;

    [SerializeField]
    private GameObject _cameraObject;
    private const float _camStartRot = 180;

    [SerializeField]
    private GameObject _characterBody;

    [Header("Animation")]
    [SerializeField]
    private Animator _animController;
    [SerializeField]
    private AnimationCurve _nonCombatSpeedToAnimationCurve;
    [SerializeField]
    private AnimationCurve _combatSpeedToAnimationCurve;
    [SerializeField]
    private float _animDampen = 0.1f;
    [SerializeField]
    private float _animDampenCombat = 0.05f;
    [SerializeField]
    private float _combatAnimThreshold = 0.5f;

    private void Awake()
    {
        _playerRotation = new(_characterBody);
        _sprinting = false;
    }

    public void Move(Vector2 input, CharacterStates currentState)
    {
        Vector2 movement = Vector2.zero;
        float speedModifier = 0;

        if (input.magnitude > _inputThreshold)
        {
            movement = ConvertInputToDirection(input);
            speedModifier = GetMovementSpeed(movement, currentState);
        }
        else
        {
            _currentSpeed = 0;
        }

        MoveCharacter(movement, speedModifier);
        CheckCharacterRotation(movement, currentState);
        AnimateMovement(movement, input, currentState);
    }

    public void SetSprinting(bool triggered)
    {
        _sprinting = triggered;
    }

    private void MoveCharacter(Vector2 movement, float speedModifier)
    {
        _characterContainer.transform.position = ConvertMovementToPosition(movement * speedModifier);
    }

    private float GetMovementSpeed(Vector2 movement, CharacterStates currentState)
    {
        float acceleration = _maxAccelerationByCurrentSpeed.Evaluate(_currentSpeed);
        float maxSpeed = GetMaxSpeed(currentState);
        _currentSpeed = Mathf.Clamp((acceleration + _currentSpeed) * movement.magnitude, _minMoveSpeed, maxSpeed);

        return _currentSpeed * Time.deltaTime;
    }

    private float GetMaxSpeed(CharacterStates currentState)
        => currentState switch
        {
            CharacterStates.NonCombat => _sprinting ? _maxSprintSpeed : _maxMoveSpeed,
            CharacterStates.Combat => _maxCombatSpeed,
            _ => throw new System.NotImplementedException()
        };


    #region Movement - Calculate Position

    private Vector2 ConvertInputToDirection(Vector2 input)
    {
        float direction = _cameraObject.transform.eulerAngles.y - _camStartRot;
        return input.Rotate(-direction);
    }

    private Vector3 ConvertMovementToPosition(Vector2 movement)
    {
        Vector3 newPos = _characterContainer.transform.position;
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

    public void RotateCharacterToTarget(Vector3 targetPosition)
    {
        Vector3 dir = targetPosition - _characterContainer.transform.position;
        Vector2 dir2D = new Vector2(dir.x, dir.z);

        _playerRotation.RotateBodyFromMovement(dir2D);
    }

    #endregion

    #region Movement - Animation

    private void AnimateMovement(Vector2 movement, Vector2 input, CharacterStates currentState)
    {
        switch (currentState)
        {
            case CharacterStates.NonCombat:
                AnimateMovementMagnitude(movement);
                break;
                case CharacterStates.Combat:
                AnimateMovementMagnitude(movement);
                AnimateMovementCombat(input);
                break;
                default:
                throw new System.NotImplementedException();
        }
    }

    private void AnimateMovementMagnitude(Vector2 movement)
    {
        Vector2 moveSpeed = movement * _currentSpeed;
        float animSpeed = _nonCombatSpeedToAnimationCurve.Evaluate(moveSpeed.magnitude);

        _animController.SetFloat("MoveMagnitude", animSpeed, _animDampen, Time.deltaTime);
    }

    private void AnimateMovementCombat(Vector2 input)
    {
        Vector2 moveSpeed = input * _currentSpeed;

        moveSpeed.x = moveSpeed.x > _combatAnimThreshold || moveSpeed.x < -_combatAnimThreshold ? moveSpeed.x : 0;
        moveSpeed.y = moveSpeed.y > _combatAnimThreshold || moveSpeed.y < -_combatAnimThreshold ? moveSpeed.y : 0;

        float animSpeedX = _combatSpeedToAnimationCurve.Evaluate(moveSpeed.x);
        float animSpeedY = _combatSpeedToAnimationCurve.Evaluate(moveSpeed.y);

        _animController.SetFloat("MoveX", animSpeedX, _animDampenCombat, Time.deltaTime);
        _animController.SetFloat("MoveZ", animSpeedY, _animDampenCombat, Time.deltaTime);
    }

    #endregion
}