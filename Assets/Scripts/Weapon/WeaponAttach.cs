using System.Collections.Generic;
using UnityEngine;

public class WeaponAttach : MonoBehaviour
{
    [SerializeField]
    private Object _weaponPrefab;

    private GameObject _weaponInstance;

    [SerializeField]
    private WeaponAttachPoint[] _attachPoints;
    private Dictionary<CharacterStates, GameObject> _attachPointsByState;

    [SerializeField]
    private CharacterStates _initialState;
    public CharacterStates CurrentState { get; private set; }

    void Awake()
    {
        _attachPointsByState = _attachPoints.AsDictionary();
        _weaponInstance = CreateWeapon();
        CurrentState = _initialState;
        AttachWeapon();
    }

    [ContextMenu("Create Weapon")]
    private GameObject CreateWeapon()
        => Instantiate(_weaponPrefab) as GameObject;

    private void AttachWeapon()
    {
        _weaponInstance.transform.SetParent(_attachPointsByState[CurrentState].transform, false);
    }

    public void SetState(CharacterStates state)
    {
        CurrentState = state;
        AttachWeapon();
    }
}

[System.Serializable]
public struct WeaponAttachPoint
{
    public CharacterStates State;
    public GameObject AttachPoint;
}