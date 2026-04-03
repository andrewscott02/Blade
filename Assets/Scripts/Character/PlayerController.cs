using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : CharacterController
{
    [SerializeField]
    private CharacterStates _initialState = CharacterStates.NonCombat;
    public CharacterStates CurrentState { get; private set; }

    private PlayerMovement _playerMovement;
    private PlayerCamera _playerCamera;

    [SerializeField]
    private InputActionReference _moveInput;
    [SerializeField]
    private InputActionReference _cameraInput;

    private void Awake()
    {
        CurrentState = _initialState;
        _playerMovement = GetComponent<PlayerMovement>();
        _playerCamera = GetComponent<PlayerCamera>();
    }

    private void Update()
    {
        CheckCameraInput();
        CheckMovementInput();
    }

    private void CheckCameraInput()
    {
        Vector2 inputValue = _cameraInput.action.ReadValue<Vector2>();
        _playerCamera.RotateCam(inputValue);
    }

    private void CheckMovementInput()
    {
        Vector2 inputValue = _moveInput.action.ReadValue<Vector2>();
        _playerMovement.Move(inputValue, CurrentState);
    }
}