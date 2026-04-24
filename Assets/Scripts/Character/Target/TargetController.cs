using UnityEngine;

public class TargetController : CharacterManager
{
    private CharacterMovement _characterMovement;

    private void Start()
    {
        LockOn();
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
}
