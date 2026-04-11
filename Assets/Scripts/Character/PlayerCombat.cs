using System.Collections;
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
    private float _resetToBaseGuardDelay = 0.75f;
    [SerializeField]
    private float _resetToBaseGuardAfterAttackingDelay = 1.75f;

    private const string _upperBodyLayerName = "UpperBody";
    private int _upperBodyLayerIndex;

    private bool debug_guardThresholdMet;

    private void Start()
    {
        _upperBodyLayerIndex = _animator.GetLayerIndex(_upperBodyLayerName);
        _canResetBaseGuard = false;
        _canChangeGuard = true;
        _canAttack = true;

        SetAnimationState(CharacterStates.NonCombat);

        _animatorEvents.ResetChangeGuardDelegate += ResetCanChangeGuard;
        _animatorEvents.ResetAttackDelegate += ResetCanAttack;
    }

    internal void SetGuard(Vector2 input)
    {
        //TODO: Maybe queue up a desired guard direction for when guarding becomes available?
        if (!_canChangeGuard)
            return;

        _guardDirection = DetermineGuardDirection(input);
        AnimateGuardDirection();
    }

    private void AnimateGuardDirection()
    {
        _animator.SetFloat("GuardX", _guardDirection.x, _animDampenGuard, Time.deltaTime);
        _animator.SetFloat("GuardY", _guardDirection.y, _animDampenGuard, Time.deltaTime);
    }

    private Vector2 DetermineGuardDirection(Vector2 input)
    {
        if (input.magnitude >= _minGuardChangeThreshold)
        {
            debug_guardThresholdMet = true;

            StopResetGuardToBaseCoroutine();
            //TryStartResetGuardToBaseCoroutine(_resetToBaseGuardDelay);

            return input.normalized;
        }

        debug_guardThresholdMet = false;
        //TODO: Not working as expected?
        TryStartResetGuardToBaseCoroutine(_resetToBaseGuardDelay);
        return ResetGuardPos();
    }

    private Vector2 ResetGuardPos()
        => _canResetBaseGuard
        ? Vector2.zero
        : _guardDirection;

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

        AnimateGuardDirection();
        StopResetGuardToBaseCoroutine();
        _canResetBaseGuard = false;
        _canChangeGuard = false;
        _canAttack = false;

        _animator.SetTrigger("Attack");

        _guardDirection = -_guardDirection;
    }

    private void ResetCanChangeGuard()
    {
        _canChangeGuard = true;
        AnimateGuardDirection();

        StopResetGuardToBaseCoroutine();
        TryStartResetGuardToBaseCoroutine(_resetToBaseGuardAfterAttackingDelay);
    }

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