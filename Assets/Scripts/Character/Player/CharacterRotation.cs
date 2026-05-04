using UnityEngine;

public class CharacterRotation
{
    private GameObject _characterBody;

    public CharacterRotation(GameObject characterBody)
    {
        _characterBody = characterBody;
    }

    public void RotateBodyFromMovement(Vector2 direction2)
    {
        Vector3 direction3 = new(direction2.x, 0, direction2.y);

        RotateBody(Quaternion.LookRotation(direction3));
    }

    private void RotateBody(Quaternion desiredRotation)
    {
        _characterBody.transform.rotation = desiredRotation;
    }
}