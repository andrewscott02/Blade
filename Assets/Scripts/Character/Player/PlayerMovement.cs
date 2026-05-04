using UnityEngine;

public class PlayerMovement : CharacterMovement
{
    [SerializeField]
    private float _inputThreshold = 0.25f;

    [SerializeField]
    private GameObject _cameraObject;
    private const float _camStartRot = 180;

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

    private Vector2 ConvertInputToDirection(Vector2 input)
    {
        float direction = _cameraObject.transform.eulerAngles.y - _camStartRot;
        return input.Rotate(-direction);
    }
}