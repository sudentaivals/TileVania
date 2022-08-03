using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaypointMovement : MonoBehaviour
{
    [SerializeField] float _speed;
    [SerializeField] float _acceleration;
    [SerializeField]List<Vector2> _path = new List<Vector2>();
    [SerializeField] UnityEvent _onPathEnds;
    private bool _isMoving;

    private int _currentWaypointIndex = 0;
    public void SetWaypoints(List<Vector2> waypoints)
    {
        _path = waypoints;
        _isMoving = true;
    }

    private void Update()
    {
        if (_isMoving)
        {
            transform.position = Vector2.MoveTowards(transform.position, _path[_currentWaypointIndex], _speed * Time.deltaTime);
            _speed += _acceleration * Time.deltaTime;
            if ((Vector2)transform.position == _path[_currentWaypointIndex])
            {
                _currentWaypointIndex++;
                if (_currentWaypointIndex == _path.Count)
                {
                    _isMoving = false;
                    _onPathEnds?.Invoke();
                }
            }
        }
    }
}
