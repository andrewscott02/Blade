using UnityEngine;

public class PlayerRotation
{
    private GameObject _characterBody;

    public PlayerRotation(GameObject characterBody)
    {
        _characterBody = characterBody;
    }

    public void RotateBodyFromMovment(Vector2 movement)
    {
        RotateBody(movement);
    }

    private void RotateBody(Vector3 desiredRotation)
    {
        _characterBody.transform.rotation = Quaternion.Euler(desiredRotation);
    }
}