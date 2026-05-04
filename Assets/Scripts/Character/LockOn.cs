using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LockOn : MonoBehaviour
{
    [SerializeField]
    private float _lockOnRadius;

    [SerializeField]
    private LayerMask _layerMask;

    [SerializeField]
    private bool drawGizmos = false;

    public LockOnTarget CurrentTarget { get; private set; }

    [SerializeField]
    private Animator _animController;

    public bool TryGetLockOnTarget(out LockOnTarget target)
    {
        LockOnTarget[] targets = GetAllLockOnTargetsInRange().ToArray();

        if (targets.Length == 0)
        {
            target = null;
            return false;
        }

        target = EvaluateClosestTarget(targets);
        return true;
    }

    private IEnumerable<LockOnTarget> GetAllLockOnTargetsInRange()
    {
        //Collider[] overlappingColliders = Physics.OverlapSphere(transform.position, _lockOnRadius);

        //foreach (Collider col in overlappingColliders)
        //{
        //    LockOnTarget target = col.GetComponent<LockOnTarget>();

        //    if (target != null && target.gameObject != this.gameObject)
        //    {
        //        yield return target;
        //    }
        //}

        foreach (var target in FindObjectsByType<LockOnTarget>(FindObjectsSortMode.None))
        {
            if (target.gameObject != this.gameObject && Vector3.Distance(target.transform.position, transform.position) < _lockOnRadius)
            {
                yield return target;
            }
        }
    }

    private LockOnTarget EvaluateClosestTarget(LockOnTarget[] targets)
    {
        LockOnTarget bestMatch = null;

        foreach (LockOnTarget target in targets)
        {
            if (bestMatch == null || IsNewTargetCloser(bestMatch, target))
            {
                bestMatch = target;
            }
        }

        return bestMatch;
    }

    private bool IsNewTargetCloser(LockOnTarget oldTarget, LockOnTarget newTarget)
    {
        float distanceOld = Vector3.Distance(transform.position, oldTarget.transform.position);
        float distanceNew = Vector3.Distance(transform.position, newTarget.transform.position);

        return distanceOld > distanceNew;

        //TODO: Also check angle
        //Vector3.Angle
    }

    public void OnDrawGizmosSelected()
    {
        if (drawGizmos)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _lockOnRadius);
        }
    }

    internal void SetTarget(LockOnTarget target)
    {
        CurrentTarget = target;
        _animController.SetBool("Combat", true);
    }

    internal void UnlockTarget()
    {
        _animController.SetBool("Combat", false);
    }

    internal void Attacking(AttackController attackController, Vector2 attackDirection)
    {
        CurrentTarget.BeingAttacked(attackController, attackDirection);
    }
}