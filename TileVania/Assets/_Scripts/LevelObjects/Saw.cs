using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Saw : MonoBehaviour
{
    [SerializeField] Transform _waypoints;
    [SerializeField] float _speed;
    [SerializeField] float _rotationSpeed;
    [SerializeField] bool _movingRight;

    private List<Vector3> _concreteWaypoints;
    private int _currentWaypointIndex;

    private void Start()
    {
        GenerateWaypoints();
        _currentWaypointIndex = _movingRight ? 0 : _concreteWaypoints.Count - 1;
    }

    private void GenerateWaypoints()
    {
        _concreteWaypoints = new List<Vector3>();
        foreach (Transform tr in _waypoints)
        {
            _concreteWaypoints.Add(tr.position);
        }
    }

    private void Update()
    {
        transform.Rotate(0, 0, _movingRight ? _rotationSpeed * Time.deltaTime : -_rotationSpeed * Time.deltaTime);
        transform.position = Vector2.MoveTowards(transform.position, _concreteWaypoints[_currentWaypointIndex], _speed * Time.deltaTime);
        if (transform.position == _concreteWaypoints[_currentWaypointIndex])
        {
            if (_movingRight)
            {
                _currentWaypointIndex++;
                if (_currentWaypointIndex == _concreteWaypoints.Count)
                {
                    _currentWaypointIndex = 0;
                }
            }
            else
            {
                _currentWaypointIndex--;
                if(_currentWaypointIndex < 0)
                {
                    _currentWaypointIndex = _concreteWaypoints.Count - 1;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (Transform wp in _waypoints)
        {
            Gizmos.DrawWireSphere(wp.position, 0.1f);
        }

    }

}
