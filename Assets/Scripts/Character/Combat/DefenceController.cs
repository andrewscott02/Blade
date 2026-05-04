using System.Collections;
using UnityEngine;

public class DefenceController : MonoBehaviour
{
    private GuardDirectionController _guardController;
    private Animator _animator;
    private LockOnTarget _lockOnTarget;

    public bool BeingAttacked { get; private set; } = false;
    public bool Defending { get; private set; } = false;


    [SerializeField]
    private float _defendAngleThreshold = 20;
    [SerializeField]
    private float _defendMagnitudeThreshold = 0.5f;

    private Vector2 _defendDirection = Vector2.down;
    internal Vector2 DefendDirection => _defendDirection;

    internal void Init(Animator animator)
    {
        _animator = animator;
        _lockOnTarget = GetComponent<LockOnTarget>();
        _lockOnTarget.beingAttacked += StartBeingAttacked;
        _lockOnTarget.stopBeingAttacked += StopBeingAttacked;
        _guardController = GetComponent<GuardDirectionController>();
    }

    private void StartBeingAttacked(AttackController attackerController, Vector2 attackDir)
    {
        BeingAttacked = true;
        EvaluateDefence(attackerController, attackDir);
    }

    private IEnumerator IEvaluateDefence(AttackController attackerController, Vector2 attackDir)
    {
        yield return null;
        EvaluateDefence(attackerController, attackDir);
    }

    private void EvaluateDefence(AttackController attackerController, Vector2 attackDir)
    {
        _defendDirection = attackDir;

        float angle = Mathf.Abs(Vector2.Angle(_guardController.GuardDirection, attackDir));

        if (angle <= _defendAngleThreshold && _guardController.GuardDirection.magnitude > _defendMagnitudeThreshold)
        {
            Defending = true;
            _animator.SetBool("Defending", Defending);
            _animator.SetFloat("DefendX", attackDir.x, 0, Time.deltaTime);
            _animator.SetFloat("DefendY", attackDir.y, 0, Time.deltaTime);
        }
        else
        {
            Defending = false;
            _animator.SetBool("Defending", Defending);
        }

        if (BeingAttacked)
            StartCoroutine(IEvaluateDefence(attackerController, attackDir));
        else
            StopAllCoroutines();
    }

    private void StopBeingAttacked()
    {
        BeingAttacked = false;
        Defending = false;
        _animator.SetBool("Defending", Defending);
        StopAllCoroutines();
    }
}