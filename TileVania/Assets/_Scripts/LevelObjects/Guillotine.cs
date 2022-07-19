using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guillotine : MonoBehaviour
{
    [SerializeField] float _force;
    [SerializeField] float _chainHeight = 3;
    [SerializeField] HingeJoint2D _guillotineHj;
    [SerializeField] GameObject _chain;
    [SerializeField] float _timer;
    [SerializeField] float _minAngle;
    [SerializeField] float _maxAngle;

    private float _currentTimer;

    private readonly float _chainWidth = 0.37f;
    void Start()
    {
        _chain.GetComponent<SpriteRenderer>().size = new Vector2(_chainWidth, _chainHeight);
        _guillotineHj.connectedAnchor = new Vector2(0, -_chainHeight);
        JointAngleLimits2D limits = new JointAngleLimits2D { max = _maxAngle, min = _minAngle };
        _chain.GetComponent<HingeJoint2D>().limits = limits;
        
    }

    private void Update()
    {
        _currentTimer += Time.deltaTime;
        if(_currentTimer >= _timer)
        {
            _force *= -1;
            _currentTimer = 0;
        }
    }

    private void FixedUpdate()
    {
        _guillotineHj.attachedRigidbody.AddRelativeForce(new Vector2(_force, 0));
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;


        var point = transform.position + new Vector3(0, -(_chainHeight + 1), 0);
        Gizmos.DrawLine(transform.position, point.RotateAroundPoint(transform.position, new Vector3(0, 0, _chain.GetComponent<HingeJoint2D>().limits.min)));
        Gizmos.DrawLine(transform.position, point.RotateAroundPoint(transform.position, new Vector3(0, 0, _chain.GetComponent<HingeJoint2D>().limits.max)));
        Gizmos.DrawWireCube(transform.position + new Vector3(0, -_chainHeight / 2), new Vector3(_chainWidth, _chainHeight));
        Gizmos.DrawWireSphere(transform.position - new Vector3(0, _chainHeight + 1f), 1f); 
    }

}
