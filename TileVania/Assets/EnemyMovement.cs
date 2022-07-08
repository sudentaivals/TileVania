using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] float _speed = 3f;
    [SerializeField] CircleCollider2D _wallChecker;
    [SerializeField] CircleCollider2D _abyssChecker;
    [SerializeField] LayerMask _groundMask;

    private Rigidbody2D _rigidBody;

    private bool IsWallNextToEnemy => _wallChecker.IsTouchingLayers(_groundMask);

    private bool IsAbyssNextToEnemy => !_abyssChecker.IsTouchingLayers(_groundMask);

    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _rigidBody.velocity = new Vector2(_speed, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsWallNextToEnemy || IsAbyssNextToEnemy) Flip();
    }

    private void Flip()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        _rigidBody.velocity *= -1;
    }

}
