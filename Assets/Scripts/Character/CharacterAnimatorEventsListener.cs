using UnityEngine;

public class CharacterAnimatorEventsListener : MonoBehaviour
{
    public delegate void CharacterAnimatorEventDelegate();
    public CharacterAnimatorEventDelegate ResetAttackDelegate;

    public delegate void GuardInfoEventDelegate(AttackGuardChangeInfo changeInfo);
    public GuardInfoEventDelegate ResetChangeGuardDelegate;
    public CharacterAnimatorEventDelegate ResetChangeGuardDelegateNoInfo;

    public delegate void HitInfoEventDelegate(AttackHitInfo hitInfo);
    public HitInfoEventDelegate AttackStartDelegate;
    public CharacterAnimatorEventDelegate AttackStartDelegateNoInfo;

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
        ResetChangeGuardDelegateNoInfo.TryInvoke();
    }

    public void StartAttack(AnimationEvent animEvent)
    {
        AttackHitInfo hitInfo = (AttackHitInfo)animEvent.objectReferenceParameter;
        hitInfo.SetPriority(animEvent.animatorClipInfo.weight);

        if (hitInfo == null)
            throw new System.Exception();

        AttackStartDelegate.TryInvoke(hitInfo);
        AttackStartDelegateNoInfo.TryInvoke();
    }
}