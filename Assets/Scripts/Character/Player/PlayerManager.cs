using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : CharacterManager
{
    private PlayerMovement _playerMovement;
    private PlayerCamera _playerCamera;

    [SerializeField]
    private InputActionReference _moveInput;
    [SerializeField]
    private InputActionReference _sprintInput;
    [SerializeField]
    private InputActionReference _cameraInput;
    [SerializeField]
    private InputActionReference _lockOnInput;
    [SerializeField]
    private InputActionReference _attackInput;
    [SerializeField]
    private InputActionReference _attackAltInput;

    [SerializeField]
    private CinemachineCamera _nonCombatCam;
    [SerializeField]
    private CinemachineCamera _combatCam;

    protected override void Awake()
    {
        base.Awake();
        _playerMovement = GetComponentInChildren<PlayerMovement>();
        _playerCamera = GetComponentInChildren<PlayerCamera>();

        AssignInputs();
    }

    private void Start()
    {
        _nonCombatCam.Prioritize();
    }

    private void Update()
    {
        CheckCameraInput();
        CheckMovementInput();
        RotateCameraToTarget();
    }

    private void AssignInputs()
    {
        _sprintInput.action.performed += CheckSprintInput;
        _sprintInput.action.canceled += CheckSprintCancelInput;
        _lockOnInput.action.performed += CheckLockOnInput;
        _attackInput.action.performed += CheckAttackInput;
        _attackAltInput.action.performed += CheckAttackAltInput;
    }

    private void CheckCameraInput()
    {
        Vector2 inputValue = _cameraInput.action.ReadValue<Vector2>();

        switch (CurrentState)
        {
            case CharacterStates.NonCombat:
                _playerCamera.RotateCam(inputValue);
                break;
            case CharacterStates.Combat:
                _combat.SetGuard(inputValue);
                break;
            case CharacterStates.Dead:
                break;
            default:
                throw new System.Exception("State not defined");
        }
    }

    private void CheckMovementInput()
    {
        Vector2 inputValue = _moveInput.action.ReadValue<Vector2>();
        _playerMovement.Move(inputValue, CurrentState);
    }

    private void CheckSprintInput(InputAction.CallbackContext context)
    {
        _playerMovement.SetSprinting(context.action.triggered);
    }

    private void CheckSprintCancelInput(InputAction.CallbackContext context)
    {
        _playerMovement.SetSprinting(context.action.triggered);
    }

    private void CheckLockOnInput(InputAction.CallbackContext context)
    {
        if (context.action.triggered)
        {
            switch (CurrentState)
            {
                case CharacterStates.NonCombat:
                    LockOn();
                    break;
                case CharacterStates.Combat:
                    LockOff();
                    break;
                case CharacterStates.Dead:
                    break;
                default:
                    throw new System.Exception("State not defined");
            }
        }
    }

    private void LockOn()
    {
        if (_lockOn.TryGetLockOnTarget(out LockOnTarget target))
        {
            CurrentState = CharacterStates.Combat;

            _playerMovement.SetSprinting(false);
            _lockOn.SetTarget(target);
            _combat.SetAnimationState(CurrentState);

            _combatCam.Prioritize();
        }
    }

    private void LockOff()
    {
        CurrentState = CharacterStates.NonCombat;

        _lockOn.UnlockTarget();
        _combat.SetAnimationState(CurrentState);

        _nonCombatCam.Prioritize();
    }

    private void RotateCameraToTarget()
    {
        if (CurrentState == CharacterStates.Combat)
        {
            if (_lockOn.CurrentTarget == null)
                throw new System.Exception("Current target cannot be null in combat mode");

            _playerCamera.LookAtTarget(_lockOn.CurrentTarget);
            _playerMovement.RotateCharacterToTarget(_lockOn.CurrentTarget.transform.position);
        }
    }

    private void CheckAttackInput(InputAction.CallbackContext context)
    {
        if (context.action.triggered)
        {
            AttemptAttack(AttackTypes.Default);
        }
    }

    private void CheckAttackAltInput(InputAction.CallbackContext context)
    {
        if (context.action.triggered)
        {
            AttemptAttack(AttackTypes.Alt);
        }
    }

    private void AttemptAttack(AttackTypes attackType)
    {
        switch (CurrentState)
        {
            case CharacterStates.NonCombat:
                break;
            case CharacterStates.Combat:
                _combat.Attack(attackType);
                break;
            case CharacterStates.Dead:
                break;
            default:
                throw new System.Exception("State not defined");
        }
    }
}