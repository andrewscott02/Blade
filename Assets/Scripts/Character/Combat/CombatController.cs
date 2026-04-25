using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private CharacterAnimatorEventsListener _animatorEvents;
    public CharacterAnimatorEventsListener AnimatorEventsListener => _animatorEvents;

    private const string _upperBodyLayerName = "UpperBody";
    private int _upperBodyLayerIndex;

    private GuardDirectionController _guardController;
    private AttackController _attackController;

    private List<AttackGuardChangeInfo> changeInfoQueue;

    private void Start()
    {
        _upperBodyLayerIndex = _animator.GetLayerIndex(_upperBodyLayerName);

        SetAnimationState(CharacterStates.NonCombat);

        _animatorEvents.ResetChangeGuardDelegate += RequestResetCanChangeGuard;

        _guardController = GetComponent<GuardDirectionController>();
        _guardController.Init(_animator);

        _attackController = GetComponent<AttackController>();
        _attackController.Init(_animator, _animatorEvents, _guardController);
    }

    internal void SetGuard(Vector2 input)
    {
        _guardController.SetGuard(input);
    }

    internal bool CanSwitchAnimationState(CharacterStates previousState)
    {
        switch (previousState)
        {
            case CharacterStates.NonCombat:
                return true;
            case CharacterStates.Combat:
                return _attackController.CanAttack;
            default:
                throw new System.NotImplementedException();
        }
    }

    internal void SetAnimationState(CharacterStates state)
    {
        switch (state)
        {
            case CharacterStates.NonCombat:
                _animator.SetLayerWeight(_upperBodyLayerIndex, 0);
                break;
            case CharacterStates.Combat:
                _animator.SetLayerWeight(_upperBodyLayerIndex, 1);
                break;
            default:
                throw new System.NotImplementedException();
        }
    }

    internal void Attack(AttackTypes attackType)
    {
        _attackController.Attack(attackType);
    }

    private void RequestResetCanChangeGuard(AttackGuardChangeInfo changeInfo)
    {
        if (changeInfoQueue == null || changeInfoQueue.Count == 0)
        {
            changeInfoQueue = new List<AttackGuardChangeInfo>();
            StartCoroutine(QueueResetCanChangeGuard());
        }

        changeInfoQueue.Add(changeInfo);
    }

    private IEnumerator QueueResetCanChangeGuard()
    {
        yield return new WaitForFixedUpdate();

        AttackGuardChangeInfo changeInfo = GetHighestPriority(changeInfoQueue);
        changeInfoQueue = null;
        _guardController.ResetCanChangeGuard(changeInfo);
    }

    private static AttackGuardChangeInfo GetHighestPriority(List<AttackGuardChangeInfo> changeInfoList)
    {
        AttackGuardChangeInfo highestPriorityItem = null;
        float highestPriorityIndex = -99999999;

        foreach (AttackGuardChangeInfo changeInfo in changeInfoList)
        {
            if (highestPriorityItem == null || changeInfo.Priority > highestPriorityIndex)
            {
                highestPriorityItem = changeInfo;
                highestPriorityIndex = changeInfo.Priority;
            }
        }

        return highestPriorityItem;
    }
}