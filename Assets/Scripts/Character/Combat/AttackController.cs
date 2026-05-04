using System;
using UnityEditor.PackageManager;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    private Animator _animator;
    private GuardDirectionController _guardController;
    private LockOn _lockOn;
    private Weapon _weapon;

    [SerializeField]
    private float _attackSpeed = 1;
    [SerializeField]
    private bool _telegraphAttacks = true;

    public bool CanAttack { get; private set; }

    private Vector2 _attackDirection;

    internal void Init(Animator animator, CharacterAnimatorEventsListener animatorEvents, GuardDirectionController guardController)
    {
        _animator = animator;
        _guardController = guardController;
        _lockOn = GetComponent<LockOn>();
        _weapon = GetComponent<AttachPoint>().WeaponScript;

        _animator.SetFloat("AttackSpeed", _attackSpeed);
        _animator.SetBool("Telegraph", _telegraphAttacks);
        animatorEvents.AttackStartDelegate += StartAttackLogic;
        animatorEvents.ResetAttackDelegate += ResetCanAttack;

        CanAttack = true;
        _attackDirection = Vector2.zero;
    }

    internal void Attack(AttackTypes attackType)
    {
        //TODO: Maybe queue up a combo attack when it's available?
        if (!CanAttack)
            return;

        _attackDirection = _guardController.GuardDirection;
        AnimateAttackDirection(0);
        _guardController.StopResetGuardCoroutine();
        _guardController.StopResetGuardToBaseCoroutine();
        _guardController.SetCanChangeGuard(false);
        _guardController.SetCanResetGuard(false);
        CanAttack = false;

        _animator.SetTrigger($"Attack-{attackType}");
    }

    internal void AnimateAttackDirection(float dampen)
    {
        _animator.SetFloat("AttackX", _attackDirection.x, dampen, Time.deltaTime);
        _animator.SetFloat("AttackY", _attackDirection.y, dampen, Time.deltaTime);
    }

    private void StartAttackLogic(AttackHitInfo hitInfo)
    {
        _lockOn.Attacking(this, _attackDirection);
        _weapon.StartAttack(hitInfo);
    }

    private void ResetCanAttack()
    {
        CanAttack = true;

        _lockOn.StopAttacking();
        _weapon.EndAttack();
    }
}