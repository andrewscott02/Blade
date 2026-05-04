using UnityEngine;

public class TargetController : CharacterManager
{
    private CharacterMovement _characterMovement;

    [SerializeField]
    private CharacterAnimatorEventsListener _animatorEvents;
    public CharacterAnimatorEventsListener AnimatorEventsListener => _animatorEvents;

    [SerializeField]
    private Vector2 guardPos;

    private void Start()
    {
        LockOn();

        InvokeRepeating("Attack", 2.5f, 2.5f);
        _animatorEvents.ResetAttackDelegate += DetermineNewAttackDir;
    }

    private void LockOn()
    {
        if (_lockOn.TryGetLockOnTarget(out LockOnTarget target))
        {
            CurrentState = CharacterStates.Combat;

            //_characterMovement.SetSprinting(false);
            _lockOn.SetTarget(target);
            _combat.SetAnimationState(CurrentState);

            //_combatCam.Prioritize();
        }
    }

    private void FixedUpdate()
    {
        _combat.SetGuard(guardPos);
    }

    private void Attack()
    {
        _combat.Attack(AttackTypes.Default);
    }

    private void DetermineNewAttackDir()
    {
        float x = Random.Range((float)-1, (float)1);
        float y = Random.Range((float)-1, (float)1);

        guardPos = new Vector2(x, y).normalized;
    }
}
