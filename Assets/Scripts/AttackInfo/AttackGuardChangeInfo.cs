using UnityEngine;

[CreateAssetMenu(fileName = "NewGuardChangeInfo", menuName = "AttackInfo/GuardChangeInfo")]
public class AttackGuardChangeInfo : ScriptableObject
{
    private float _priority;
    public float Priority => _priority;
    public void SetPriority(float priority)
        { this._priority = priority; }

    [SerializeField]
    private GuardSwitchType _guardSwitchType;
    public GuardSwitchType GuardSwitchType => _guardSwitchType;

    [SerializeField]
    private Vector2 _guardDirection;
    public Vector2 GuardDirection => _guardDirection;

    [SerializeField]
    private float _canResetGuardDelay;
    public float CanResetGuardDelay => _canResetGuardDelay;

    [SerializeField]
    private float _canResetGuardToBaseDelay;
    public float CanResetGuardToBaseDelay => _canResetGuardToBaseDelay;
}