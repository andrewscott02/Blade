using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : CharacterController
{
    private PlayerMovement _playerMovement;
    private PlayerCamera _playerCamera;

    [SerializeField]
    private InputActionReference _moveInput;
    [SerializeField]
    private InputActionReference _cameraInput;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _playerCamera = GetComponent<PlayerCamera>();
    }

    private void Update()
    {
        CheckCameraInput();
    }

    private void CheckCameraInput()
    {
        Vector2 inputValue = _cameraInput.action.ReadValue<Vector2>();
        _playerCamera.RotateCam(inputValue);
    }
}