using System.Collections;
using UnityEngine;

public class GuardDirectionController : MonoBehaviour
{
    private Animator _animator;

    [SerializeField]
    private float _minGuardChangeThreshold = 0.25f;

    private Vector2 _guardDirection = Vector2.down;
    internal Vector2 GuardDirection => _guardDirection;

    private bool _resetGuardCoroutineRunning;
    private bool _canResetBaseGuard;
    private bool _canChangeGuard;

    [SerializeField]
    private float _animDampenGuard = 0.05f;
    [SerializeField]
    private float _resetToBaseGuardAfterAttackingDelay = 1.75f;

    internal void Init(Animator animator)
    {
        _animator = animator;

        _resetGuardCoroutineRunning = false;
        _canResetBaseGuard = false;
        _canChangeGuard = true;
    }

    internal void SetCanResetGuard(bool canReset)
    {
        _canResetBaseGuard = canReset;
    }

    internal void SetGuard(Vector2 input)
    {
        //TODO: Maybe queue up a desired guard direction for when guarding becomes available?
        if (!_canChangeGuard)
            return;

        _guardDirection = DetermineGuardDirection(input);
        AnimateGuardDirection(_animDampenGuard);
    }

    internal void AnimateGuardDirection(float dampenGuard)
    {
        _animator.SetFloat("GuardX", _guardDirection.x, dampenGuard, Time.deltaTime);
        _animator.SetFloat("GuardY", _guardDirection.y, dampenGuard, Time.deltaTime);
    }

    private Vector2 DetermineGuardDirection(Vector2 input)
    {
        if (input.magnitude >= _minGuardChangeThreshold)
        {
            return input.normalized;
        }

        return ResetGuardPos();
    }

    private Vector2 ResetGuardPos()
        => _canResetBaseGuard
        ? Vector2.zero
        : _guardDirection;

    internal void ResetCanChangeGuard(AttackGuardChangeInfo changeInfo)
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

    public void StopResetGuardToBaseCoroutine()
    {
        _resetGuardCoroutineRunning = false;
        StopCoroutine(ResetCanResetGuardToBase(0));
    }
}