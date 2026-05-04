using System.Collections;
using UnityEngine;

public class GuardDirectionController : MonoBehaviour
{
    private Animator _animator;
    private LockOnTarget _lockOnTarget;

    [SerializeField]
    private float _minGuardChangeThreshold = 0.25f;

    private Vector2 _guardDirection = Vector2.down;
    internal Vector2 GuardDirection => _guardDirection;

    [SerializeField]
    private float _defendAngleThreshold = 20;
    [SerializeField]
    private float _defendMagnitudeThreshold = 0.5f;

    private Vector2 _defendDirection = Vector2.down;
    internal Vector2 DefendDirection => _defendDirection;

    private bool _resetGuardCoroutineRunning;
    private bool _resetGuardToBaseCoroutineRunning;
    private bool _canResetBaseGuard;
    private bool _canChangeGuard;

    [SerializeField]
    private float _animDampenGuard = 0.05f;

    internal void Init(Animator animator)
    {
        _animator = animator;
        _lockOnTarget = GetComponent<LockOnTarget>();
        _lockOnTarget.beingAttacked += BeingAttacked;
        _lockOnTarget.stopBeingAttacked += StopBeingAttacked;

        _resetGuardCoroutineRunning = false;
        _resetGuardToBaseCoroutineRunning = false;
        _canResetBaseGuard = true;
        _canChangeGuard = true;
    }

    #region Changing Guard Direction

    internal void SetCanChangeGuard(bool canReset)
    {
        _canChangeGuard = canReset;
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
            return input;
        }

        return GetResetGuardPos();
    }

    private Vector2 GetResetGuardPos()
        => _canResetBaseGuard
        ? Vector2.zero
        : _guardDirection;

    internal void ResetCanChangeGuard(AttackGuardChangeInfo changeInfo)
    {
        _guardDirection = GetGuardChangeDirection(changeInfo);
        AnimateGuardDirection(0);

        StopResetGuardCoroutine();
        TryStartResetGuardCoroutine(changeInfo.CanResetGuardDelay);

        StopResetGuardToBaseCoroutine();
        TryStartResetGuardToBaseCoroutine(changeInfo.CanResetGuardToBaseDelay);
    }

    private Vector2 GetGuardChangeDirection(AttackGuardChangeInfo changeInfo)
        => changeInfo.GuardSwitchType switch
        {
            GuardSwitchType.OverrideDirection => changeInfo.GuardDirection,
            GuardSwitchType.OppositeDirection => -_guardDirection.normalized,
            _ => throw new System.NotImplementedException()
        };

    #endregion

    #region Reset Can Change Guard

    private IEnumerator ResetCanResetGuard(float delay)
    {
        _resetGuardCoroutineRunning = true;
        _canChangeGuard = false;
        yield return new WaitForSeconds(delay);
        _canChangeGuard = true;
    }

    private void TryStartResetGuardCoroutine(float delay)
    {
        if (_resetGuardCoroutineRunning)
            return;

        StartCoroutine(ResetCanResetGuard(delay));
    }

    public void StopResetGuardCoroutine()
    {
        _resetGuardCoroutineRunning = false;
        StopCoroutine(ResetCanResetGuard(0));
    }

    #endregion

    #region Reset Guard To Base

    private IEnumerator ResetCanResetGuardToBase(float delay)
    {
        _resetGuardToBaseCoroutineRunning = true;
        _canResetBaseGuard = false;
        yield return new WaitForSeconds(delay);
        _canResetBaseGuard = true;
        _resetGuardToBaseCoroutineRunning = false;
    }

    private void TryStartResetGuardToBaseCoroutine(float delay)
    {
        if (_resetGuardToBaseCoroutineRunning)
            return;

        StartCoroutine(ResetCanResetGuardToBase(delay));
    }

    public void StopResetGuardToBaseCoroutine()
    {
        _resetGuardToBaseCoroutineRunning = false;
        StopCoroutine(ResetCanResetGuardToBase(0));
    }

    #endregion

    private void BeingAttacked(AttackController attackerController, Vector2 attackDir)
    {
        _defendDirection = attackDir;

        float angle = Mathf.Abs(Vector2.Angle(_guardDirection, attackDir));

        if (angle <= _defendAngleThreshold && _guardDirection.magnitude > _defendMagnitudeThreshold)
        {
            _animator.SetBool("Defending", true);
            _animator.SetFloat("DefendX", attackDir.x, 0, Time.deltaTime);
            _animator.SetFloat("DefendY", attackDir.y, 0, Time.deltaTime);
        }
    }

    private void StopBeingAttacked()
    {
        _animator.SetBool("Defending", false);
    }
}