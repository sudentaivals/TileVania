using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] float _speed = 3f;
    [SerializeField] LayerMask _groundMask;
    [Header("WallCheck")]
    [SerializeField] Transform _wallChecker;
    [SerializeField] float _wallCheckerRadius;
    [SerializeField] Vector2 _wallCheckerOffset;
    [Header("Abyss check")]
    [SerializeField] Transform _abyssChecker;
    [SerializeField] float _abyssCheckerRadius;
    [SerializeField] Vector2 _abyssCheckerOffset;

    private readonly Collider2D[] _groundHits = new Collider2D[1];
    private readonly Collider2D[] _abyssHits = new Collider2D[1];

    private Rigidbody2D _rigidBody;

    private bool IsWallNextToEnemy => Physics2D.OverlapCircleNonAlloc(_wallChecker.transform.position + (Vector3)_wallCheckerOffset, _wallCheckerRadius, _groundHits, _groundMask) > 0;
    private bool IsAbyssNextToEnemy => Physics2D.OverlapCircleNonAlloc(_abyssChecker.transform.position + (Vector3)_abyssCheckerOffset, _abyssCheckerRadius, _abyssHits, _groundMask) == 0;

    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _rigidBody.velocity = new Vector2(_speed, 0);
    }

    private void FixedUpdate()
    {
        if (IsWallNextToEnemy || IsAbyssNextToEnemy)
        {
            Flip();
        }
    }

    private void Flip()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        _rigidBody.velocity *= -1;
        _abyssCheckerOffset = new Vector2(_abyssCheckerOffset.x * -1, _abyssCheckerOffset.y);
        _wallCheckerOffset = new Vector2(_wallCheckerOffset.x * -1, _wallCheckerOffset.y);

    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position + (Vector3)_wallCheckerOffset, _wallCheckerRadius);
        Gizmos.DrawWireSphere(transform.position + (Vector3)_abyssCheckerOffset, _abyssCheckerRadius);

    }

}
