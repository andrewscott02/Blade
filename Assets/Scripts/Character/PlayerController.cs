using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : CharacterController
{
    [SerializeField]
    private CharacterStates _initialState = CharacterStates.NonCombat;
    public CharacterStates CurrentState { get; private set; }

    private PlayerMovement _playerMovement;
    private PlayerCamera _playerCamera;
    private PlayerLockOn _playerLockOn;
    private WeaponAttach _weaponAttach;

    [SerializeField]
    private InputActionReference _moveInput;
    [SerializeField]
    private InputActionReference _sprintInput;
    [SerializeField]
    private InputActionReference _cameraInput;
    [SerializeField]
    private InputActionReference _lockOnInput;

    [SerializeField]
    private CinemachineCamera _nonCombatCam;
    [SerializeField]
    private CinemachineCamera _combatCam;

    private void Awake()
    {
        CurrentState = _initialState;
        _playerMovement = GetComponent<PlayerMovement>();
        _playerCamera = GetComponent<PlayerCamera>();
        _playerLockOn = GetComponent<PlayerLockOn>();
        _weaponAttach = GetComponent<WeaponAttach>();

        _nonCombatCam.Prioritize();

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
                //TODO: Change guard
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
        if (_playerLockOn.TryGetLockOnTarget(out LockOnTarget target))
        {
            CurrentState = CharacterStates.Combat;

            _playerMovement.SetSprinting(false);
            _playerLockOn.SetTarget(target);
            _combatCam.Prioritize();
            _weaponAttach.SetState(CurrentState);
        }
    }

    private void LockOff()
    {
        CurrentState = CharacterStates.NonCombat;

        _playerLockOn.UnlockTarget();
        _nonCombatCam.Prioritize();
        _weaponAttach.SetState(CurrentState);
    }

    private void RotateCameraToTarget()
    {
        if (CurrentState == CharacterStates.Combat)
        {
            if (_playerLockOn.CurrentTarget == null)
                throw new System.Exception("Current target cannot be null in combat mode");

            _playerCamera.LookAtTarget(_playerLockOn.CurrentTarget);
            _playerMovement.RotateCharacterToTarget(_playerLockOn.CurrentTarget.transform.position);
        }
    }
}