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
    private bool _reverse = false;

    private List<Vector3> _concreteWaypoints;
    private int _currentWaypointIndex;

    private void Start()
    {
        GenerateWaypoints();
        _currentWaypointIndex = 0;
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
        transform.Rotate(0, 0, _reverse ? _rotationSpeed * Time.deltaTime : -_rotationSpeed * Time.deltaTime);
        transform.position = Vector2.MoveTowards(transform.position, _concreteWaypoints[_currentWaypointIndex], _speed * Time.deltaTime);
        if (transform.position == _concreteWaypoints[_currentWaypointIndex])
        {
            if (_reverse)
            {
                _currentWaypointIndex--;
                if (_currentWaypointIndex < 0)
                {
                    _currentWaypointIndex = 0;
                    _reverse = false;
                }
            }
            else
            {
                _currentWaypointIndex++;
                if (_currentWaypointIndex >= _concreteWaypoints.Count)
                {
                    _currentWaypointIndex = _concreteWaypoints.Count - 1;
                    _reverse = true;
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
