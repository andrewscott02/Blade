using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerLockOn : MonoBehaviour
{
    [SerializeField]
    private float _lockOnRadius;

    [SerializeField]
    private LayerMask _layerMask;

    [SerializeField]
    private bool drawGizmos = false;

    public LockOnTarget CurrentTarget { get; private set; }

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
        Collider[] overlappingColliders = Physics.OverlapSphere(transform.position, _lockOnRadius);

        foreach (Collider col in overlappingColliders)
        {
            LockOnTarget target = col.GetComponent<LockOnTarget>();

            if (target != null)
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
    }
}