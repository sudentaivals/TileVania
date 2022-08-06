using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class WaypointMovement : MonoBehaviour
{
    [SerializeField] float _speed;
    [SerializeField] float _acceleration;
    [SerializeField]List<Vector2> _path = new List<Vector2>();
    [SerializeField] UnityEvent _onPathEnds;
    [SerializeField] GameObject _lastWaypointPointer;

    private GameObject _pointer;

    private bool _isMoving;

    private int _currentWaypointIndex = 0;
    public void SetWaypoints(List<Vector2> waypoints)
    {
        _path = waypoints;
        _isMoving = true;
        if(_lastWaypointPointer != null)
        {
            _pointer = Instantiate(_lastWaypointPointer);
            _pointer.transform.position = _path.Last();
        }
    }

    private void OnDestroy()
    {
        if(_pointer != null)
        {
            Destroy(_pointer);
        }
    }

    private void Update()
    {
        _pointer.transform.Rotate(new Vector3(0, 0, 180 * Time.deltaTime));
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
