using UnityEngine;

public class CharacterAnimatorEventsListener : MonoBehaviour
{
    public delegate void CharacterAnimatorEventDelegate();

    public CharacterAnimatorEventDelegate ResetAttackDelegate;
    public CharacterAnimatorEventDelegate ResetChangeGuardDelegate;

    public void ResetAttack()
    {
        ResetAttackDelegate.TryInvoke();
    }

    public void ResetChangeGuard()
    {
        ResetChangeGuardDelegate.TryInvoke();
    }
}