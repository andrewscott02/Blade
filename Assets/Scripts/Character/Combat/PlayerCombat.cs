using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private CharacterAnimatorEventsListener _animatorEvents;

    private bool _canAttack;

    private const string _upperBodyLayerName = "UpperBody";
    private int _upperBodyLayerIndex;

    private GuardDirectionController _guardController;

    private List<AttackGuardChangeInfo> changeInfoQueue;

    private void Start()
    {
        _upperBodyLayerIndex = _animator.GetLayerIndex(_upperBodyLayerName);
        _canAttack = true;

        SetAnimationState(CharacterStates.NonCombat);

        _animatorEvents.ResetChangeGuardDelegate += RequestResetCanChangeGuard;
        _animatorEvents.ResetAttackDelegate += ResetCanAttack;

        _guardController = GetComponent<GuardDirectionController>();
        _guardController.Init(_animator);
    }

    internal void SetGuard(Vector2 input)
    {
        _guardController.SetGuard(input);
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
        //TODO: Maybe queue up a combo attack when it's available?
        if (!_canAttack)
            return;

        AnimateAttackDirection(0);
        _guardController.StopResetGuardCoroutine();
        _guardController.StopResetGuardToBaseCoroutine();
        _guardController.SetCanChangeGuard(false);
        _guardController.SetCanResetGuard(false);
        _canAttack = false;

        _animator.SetTrigger($"Attack-{attackType}");
    }

    internal void AnimateAttackDirection(float dampen)
    {
        _animator.SetFloat("AttackX", _guardController.GuardDirection.x, dampen, Time.deltaTime);
        _animator.SetFloat("AttackY", _guardController.GuardDirection.y, dampen, Time.deltaTime);
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

    private void ResetCanAttack()
    {
        _canAttack = true;
    }
}