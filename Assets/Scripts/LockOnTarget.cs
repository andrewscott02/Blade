using UnityEngine;

public class LockOnTarget : MonoBehaviour
{
    public delegate void BeingAttackedDelegate(AttackController attackerController, Vector2 attackDir);
    public BeingAttackedDelegate beingAttacked;

    internal void BeingAttacked(AttackController attackerController, Vector2 attackDir)
    {
        if (beingAttacked.GetInvocationList().Length > 0)
        {
            beingAttacked.Invoke(attackerController, attackDir);
        }
    }
}