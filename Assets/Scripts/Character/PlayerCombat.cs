using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private float _minGuardChangeThreshold = 0.25f;

    private Vector2 _guardDirection = Vector2.down;

    [SerializeField]
    private float _animDampenGuard = 0.05f;

    private const string _upperBodyLayerName = "UpperBody";
    private int _upperBodyLayerIndex;

    private void Start()
    {
        _upperBodyLayerIndex = _animator.GetLayerIndex(_upperBodyLayerName);

        SetAnimationState(CharacterStates.NonCombat);
    }

    internal void SetGuard(Vector2 input)
    {
        if (input.magnitude < _minGuardChangeThreshold)
            return;

        _guardDirection = input.normalized;

        _animator.SetFloat("GuardX", _guardDirection.x, _animDampenGuard, Time.deltaTime);
        _animator.SetFloat("GuardY", _guardDirection.y, _animDampenGuard, Time.deltaTime);
    }

    internal void SetAnimationState(CharacterStates state)
    {
        switch (state)
        {
            case CharacterStates.NonCombat:
                _animator.SetLayerWeight(_upperBodyLayerIndex, 0);
                break;
            case CharacterStates.Combat:
                _animator.SetLayerWeight(_upperBodyLayerIndex, 1);
                break;
            default:
                throw new System.NotImplementedException();
        }
    }
}