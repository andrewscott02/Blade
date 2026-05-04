using UnityEngine;

public class WeaponPhysicsHandle : MonoBehaviour
{
    private Transform _target;
    private Rigidbody _rb;

    [SerializeField]
    private float _forceMultiplier = 10;

    [SerializeField]
    private float _movementThreshold = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _target = transform.parent ?? null;
        transform.parent = null;

        transform.position = _target.position;

        _rb = GetComponent<Rigidbody>();
    }

    //Vector2 movement;
    //Vector2 dir;
    //Vector2 force;

    //private void FixedUpdate()
    //{
    //    if (_target != null)
    //    {
    //        movement = _target.position - transform.position;

    //        if (movement.magnitude > _movementThreshold)
    //        {
    //            dir = movement.normalized;
    //            force = dir * _forceMultiplier * Time.fixedDeltaTime;
    //            _rb.AddForce(movement, ForceMode.VelocityChange);
    //            //_rb.AddRelativeForce
    //        }
    //    }
    //}
}
