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

    [SerializeField]
    private InputActionReference _moveInput;
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

        _nonCombatCam.Prioritize();
    }

    private void Update()
    {
        CheckCameraInput();
        CheckMovementInput();
        CheckLockOnInput();
        RotateCameraToTarget();
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

    private void CheckLockOnInput()
    {
        if (_lockOnInput.action.triggered)
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
            _playerLockOn.SetTarget(target);
            CurrentState = CharacterStates.Combat;
            _combatCam.Prioritize();
        }
    }

    private void LockOff()
    {
        CurrentState = CharacterStates.NonCombat;
        _nonCombatCam.Prioritize();
    }

    private void RotateCameraToTarget()
    {
        if (CurrentState == CharacterStates.Combat)
        {
            if (_playerLockOn.CurrentTarget == null)
                throw new System.Exception("Current target cannot be null in combat mode");

            _playerCamera.LookAtTarget(_playerLockOn.CurrentTarget);
        }
    }
}