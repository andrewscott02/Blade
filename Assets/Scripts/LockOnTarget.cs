using UnityEngine;

public class LockOnTarget : MonoBehaviour
{
    public delegate void BeingAttackedDelegate(AttackController attackerController, Vector2 attackDir);
    public BeingAttackedDelegate beingAttacked;
    public delegate void StopBeingAttackedDelegate();
    public StopBeingAttackedDelegate stopBeingAttacked;

    internal void BeingAttacked(AttackController attackerController, Vector2 attackDir)
    {
        if (beingAttacked.GetInvocationList().Length > 0)
        {
            beingAttacked.Invoke(attackerController, InvertedAttackDirection(attackDir));
        }
    }

    private Vector2 InvertedAttackDirection(Vector2 attackDir)
    {
        Vector2 inverted = attackDir;
        inverted.x = -attackDir.x;
        return inverted;
    }

    internal void StopBeingAttacked()
    {
        if (stopBeingAttacked.GetInvocationList().Length > 0)
        {
            stopBeingAttacked.Invoke();
        }
    }
}