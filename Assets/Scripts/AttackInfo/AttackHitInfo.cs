using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackHitInfo", menuName = "AttackInfo/HitInfo")]
public class AttackHitInfo : ScriptableObject
{
    private float _priority;
    public float Priority => _priority;
    public void SetPriority(float priority)
        { this._priority = priority; }

    [SerializeField]
    private AttackTypes _attackType;
    public AttackTypes AttackType => _attackType;

    [SerializeField]
    private float _colliderScale;
    public float ColliderScale => _colliderScale;


}