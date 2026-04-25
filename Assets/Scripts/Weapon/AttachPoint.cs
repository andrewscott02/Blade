using System.Collections.Generic;
using UnityEngine;

public class AttachPoint : MonoBehaviour
{
    [SerializeField]
    private Object _weaponPrefab;

    private GameObject _weaponInstance;

    [SerializeField]
    private AttachPointInfo[] _attachPoints;
    private Dictionary<CharacterStates, GameObject> _attachPointsByState;

    [SerializeField]
    private CharacterStates _initialState;
    public CharacterStates CurrentState { get; private set; }

    [SerializeField]
    private GameObject _ikHandleNonCombat;
    [SerializeField]
    private GameObject _ikHandleCombat;

    void Awake()
    {
        _attachPointsByState = _attachPoints.AsDictionary();
        _weaponInstance = CreateWeapon();
        CurrentState = _initialState;
        AttachWeapon();

        GetComponentInParent<CharacterManager>().CharacterStateChange += SetState;
    }

    [ContextMenu("Create Weapon")]
    private GameObject CreateWeapon()
    {
        GameObject weapon = Instantiate(_weaponPrefab) as GameObject;

        weapon.GetComponentInChildren<Weapon>().Init(_ikHandleNonCombat, _ikHandleCombat);

        return weapon;
    }

    private void AttachWeapon()
    {
        _weaponInstance.transform.SetParent(_attachPointsByState[CurrentState].transform, false);
    }

    private void SetState(CharacterStates state)
    {
        CurrentState = state;
        AttachWeapon();
    }
}