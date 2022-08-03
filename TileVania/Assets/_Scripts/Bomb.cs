using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] GameObject _explosionObject;
    [SerializeField] float _rotation;
    [SerializeField] float _hitboxRadius;
    [SerializeField] Vector2 _hitboxOffset;
    [SerializeField] LayerMask _targets;

    private Collider2D[] _targetsInRadius = new Collider2D[1];

    private bool IsTouchingTargets => Physics2D.OverlapCircleNonAlloc((Vector2)transform.position + _hitboxOffset, _hitboxRadius, _targetsInRadius, _targets) > 0;
    public void Explosion()
    {
        var explosion = Instantiate(_explosionObject);
        explosion.transform.position = transform.position;
        Destroy(gameObject);
    }

    private void Update()
    {
        transform.Rotate(0, 0, _rotation * Time.deltaTime);
        if (IsTouchingTargets)
        {
            Explosion();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(transform.position + (Vector3)_hitboxOffset, _hitboxRadius);
    }


}
