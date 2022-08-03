using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParabolaTest : MonoBehaviour
{
    [SerializeField] float _boxWidth;
    [SerializeField] float _boxHeight;
    [SerializeField] LayerMask _targetLayer;
    [SerializeField] Vector2 _boxOffset;
    [SerializeField] float _cooldown = 1f;
    [SerializeField] float _parabolaHeight;
    [SerializeField] GameObject _bomb;
    private float _currentCooldown = 0f;
    private Collider2D[] _targets = new Collider2D[1];
    private bool IsTargetInRange => Physics2D.OverlapBoxNonAlloc((Vector2)transform.position + _boxOffset, new Vector2(_boxWidth, _boxHeight), 0, _targets, _targetLayer) > 0;

    private bool IsShootReady => _currentCooldown <= 0;

    void Start()
    {
        
    }

    void Update()
    {
        if (IsShootReady)
        {
            if (IsTargetInRange)
            {
                Shoot();
                _currentCooldown = _cooldown;

            }
        }
        else
        {
            _currentCooldown -= Time.deltaTime;
        }
    }

    private void Shoot()
    {
        var bomb = Instantiate(_bomb);
        bomb.transform.position = transform.position;
        var p1 = transform.position;
        var p3 = _targets[0].transform.position;
        var p2 = new Vector2(Mathf.Lerp(p1.x, p3.x, 0.5f), Mathf.Max(p1.y, p3.y) + _parabolaHeight);
        bomb.GetComponent<WaypointMovement>().SetWaypoints(ParabolaBuilder.GetParabolaPoints(p1, p2, p3, 15));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(transform.position + (Vector3)_boxOffset, new Vector3(_boxWidth, _boxHeight, 0));
    }
}
