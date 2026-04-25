using System.Collections.Generic;
using UnityEngine;

public class IKControl : MonoBehaviour
{
    private Animator animator;

    [SerializeField]
    private AvatarIKGoal _ikGoal;

    private bool _ikActive = true;
    private CharacterStates _currentState = CharacterStates.NonCombat;

    [SerializeField]
    private List<IKControlTarget> _ikTargets;

    void Start()
    {
        animator = GetComponent<Animator>();

        CharacterManager character = GetComponentInParent<CharacterManager>();
        if (character != null)
        {
            character.CharacterStateChange += SetCurrentState;
        }
    }

    internal void SetActive(bool active)
    {
        _ikActive = active;
    }

    private void SetCurrentState(CharacterStates currentState)
    {
        _currentState = currentState;
    }

    void OnAnimatorIK()
    {
        if (animator)
        {
            if (_ikActive && TryGetTargetFromState(_currentState, out IKControlTarget target))
            {
                SetIKTarget(target);
            }
            else
            {
                UnsetIKTarget();
            }
        }
    }

    private bool TryGetTargetFromState(CharacterStates state, out IKControlTarget outTarget)
    {
        outTarget = new();
        foreach (IKControlTarget target in _ikTargets)
        {
            if (target.State == state)
            {
                outTarget = target;
                return true;
            }
        }

        return false;
    }

    private void SetIKTarget(IKControlTarget target)
    {
        animator.SetIKPositionWeight(_ikGoal, target.IKValuePosition);
        animator.SetIKRotationWeight(_ikGoal, target.IKValueRotation);
        animator.SetIKPosition(_ikGoal, target.IKTarget.position);
        animator.SetIKRotation(_ikGoal, target.IKTarget.rotation);
    }

    private void UnsetIKTarget()
    {
        animator.SetIKPositionWeight(_ikGoal, 0);
        animator.SetIKRotationWeight(_ikGoal, 0);
        animator.SetLookAtWeight(0);
    }
}

[System.Serializable]
public struct IKControlTarget
{
    public CharacterStates State;
    public Transform IKTarget;
    public float IKValuePosition;
    public float IKValueRotation;
}