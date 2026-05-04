using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField]
    protected CharacterStates _initialState = CharacterStates.NonCombat;
    protected CharacterStates _currentState;
    public CharacterStates CurrentState
    {
        get
        {
            return _currentState;
        }

        protected set
        {
            _currentState = value;
            if (CharacterStateChange?.GetInvocationList().Length > 0)
                CharacterStateChange(_currentState);
        }
    }

    public delegate void CharacterStateChangeDelegate(CharacterStates state);
    public CharacterStateChangeDelegate CharacterStateChange;

    protected CombatController _combat;
    protected LockOn _lockOn;

    protected virtual void Awake()
    {
        CurrentState = _initialState;

        _combat = GetComponentInChildren<CombatController>();
        _lockOn = GetComponentInChildren<LockOn>();
    }
}