using UnityEngine;

public class TargetController : CharacterManager
{
    private CharacterMovement _characterMovement;

    [SerializeField]
    private Vector2 guardPos;

    private void Start()
    {
        LockOn();

        InvokeRepeating("Attack", 5f, 5f);
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
}
