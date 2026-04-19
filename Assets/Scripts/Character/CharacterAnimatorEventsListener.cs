using UnityEngine;

public class CharacterAnimatorEventsListener : MonoBehaviour
{
    public delegate void CharacterAnimatorEventDelegate();

    public CharacterAnimatorEventDelegate ResetAttackDelegate;

    public delegate void GuardInfoEventDelegate(AttackGuardChangeInfo changeInfo);
    public GuardInfoEventDelegate ResetChangeGuardDelegate;

    public void ResetAttack()
    {
        ResetAttackDelegate.TryInvoke();
    }

    public void ResetChangeGuard(AnimationEvent animEvent)
    {
        AttackGuardChangeInfo changeInfo = (AttackGuardChangeInfo)animEvent.objectReferenceParameter;
        changeInfo.SetPriority(animEvent.animatorClipInfo.weight);

        if (changeInfo == null)
            throw new System.Exception();

        ResetChangeGuardDelegate.TryInvoke(changeInfo);
    }    
}