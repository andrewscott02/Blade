using System.Collections.Generic;
using UnityEngine;

public class AttachPoint : MonoBehaviour
{
    [SerializeField]
    private Object _weaponPrefab;

    public GameObject WeaponInstance { get; private set; }
    public Weapon WeaponScript { get; private set; }

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
        WeaponInstance = CreateWeapon();
        CurrentState = _initialState;
        AttachWeapon();

        GetComponentInParent<CharacterManager>().CharacterStateChange += SetState;
    }

    [ContextMenu("Create Weapon")]
    private GameObject CreateWeapon()
    {
        GameObject weaponGO = Instantiate(_weaponPrefab) as GameObject;
        WeaponScript = weaponGO.GetComponentInChildren<Weapon>();

        WeaponScript.Init(_ikHandleNonCombat, _ikHandleCombat);

        return weaponGO;
    }

    private void AttachWeapon()
    {
        WeaponInstance.transform.SetParent(_attachPointsByState[CurrentState].transform, false);
    }

    private void SetState(CharacterStates state)
    {
        CurrentState = state;
        AttachWeapon();
    }
}