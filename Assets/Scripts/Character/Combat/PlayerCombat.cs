using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private CharacterAnimatorEventsListener _animatorEvents;

    [SerializeField]
    private float _minGuardChangeThreshold = 0.25f;

    private Vector2 _guardDirection = Vector2.down;

    private bool _canResetBaseGuard;

    private bool _resetGuardCoroutineRunning;
    private bool _canChangeGuard;
    private bool _canAttack;

    [SerializeField]
    private float _animDampenGuard = 0.05f;
    [SerializeField]
    private float _resetToBaseGuardAfterAttackingDelay = 1.75f;

    private const string _upperBodyLayerName = "UpperBody";
    private int _upperBodyLayerIndex;

    private GuardDirectionController _guardController;

    private void Start()
    {
        _upperBodyLayerIndex = _animator.GetLayerIndex(_upperBodyLayerName);
        _canResetBaseGuard = false;
        _canChangeGuard = true;
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

    internal void Attack()
    {
        //TODO: Maybe queue up a combo attack when it's available?
        if (!_canAttack)
            return;

        AnimateGuardDirection(0);
        StopResetGuardToBaseCoroutine();
        _canResetBaseGuard = false;
        _canChangeGuard = false;
        _canAttack = false;

        _animator.SetTrigger("Attack");
    }

    private List<AttackGuardChangeInfo> changeInfoQueue;

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
        ResetCanChangeGuard(changeInfo);
    }

    private static AttackGuardChangeInfo GetHighestPriority(List<AttackGuardChangeInfo> changeInfoList)
    {
        AttackGuardChangeInfo highestPriorityItem = null;
        int highestPriorityIndex = -99999999;

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

    private void ResetCanChangeGuard(AttackGuardChangeInfo changeInfo)
    {
        _canChangeGuard = true;
        _guardDirection = GetGuardChangeDirection(changeInfo);
        AnimateGuardDirection(0);

        StopResetGuardToBaseCoroutine();
        TryStartResetGuardToBaseCoroutine(_resetToBaseGuardAfterAttackingDelay);
    }

    private Vector2 GetGuardChangeDirection(AttackGuardChangeInfo changeInfo)
        => changeInfo.GuardSwitchType switch
        {
            GuardSwitchType.OverrideDirection => changeInfo.GuardDirection,
            GuardSwitchType.OppositeDirection => -_guardDirection,
            _ => throw new System.NotImplementedException()
        };

    private IEnumerator ResetCanResetGuardToBase(float delay)
    {
        _resetGuardCoroutineRunning = true;
        _canResetBaseGuard = false;
        yield return new WaitForSeconds(delay);
        _canResetBaseGuard = true;
        _resetGuardCoroutineRunning = false;
    }

    private void TryStartResetGuardToBaseCoroutine(float delay)
    {
        if (_resetGuardCoroutineRunning)
            return;

        StartCoroutine(ResetCanResetGuardToBase(delay));
    }

    private void StopResetGuardToBaseCoroutine()
    {
        _resetGuardCoroutineRunning = false;
        StopCoroutine(ResetCanResetGuardToBase(0));
    }

    private void ResetCanAttack()
    {
        _canAttack = true;
    }
}