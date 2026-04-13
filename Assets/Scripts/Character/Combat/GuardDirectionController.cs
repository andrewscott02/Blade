using UnityEngine;

public class GuardDirectionController : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private float _minGuardChangeThreshold = 0.25f;

    private Vector2 _guardDirection = Vector2.down;

    private bool _canResetBaseGuard;

    private bool _canChangeGuard;

    [SerializeField]
    private float _animDampenGuard = 0.05f;

    public void Init(Animator animator)
    {
        _animator = animator;

        _canResetBaseGuard = false;
        _canChangeGuard = true;
    }

    internal void SetGuard(Vector2 input)
    {
        //TODO: Maybe queue up a desired guard direction for when guarding becomes available?
        if (!_canChangeGuard)
            return;

        _guardDirection = DetermineGuardDirection(input);
        AnimateGuardDirection(_animDampenGuard);
    }

    private void AnimateGuardDirection(float dampenGuard)
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
}