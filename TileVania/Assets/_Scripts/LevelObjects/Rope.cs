using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    [SerializeField] Rigidbody2D _hook;
    [SerializeField] int _numberOfLinks;
    [SerializeField] GameObject _ropeSegment;
    [SerializeField] GameObject _lastSegment;

    private void Start()
    {
        GenerateRope();
    }

    private void GenerateRope()
    {
        var prevBod = _hook;
        for (int i = 0; i < _numberOfLinks; i++)
        {
            var newSegment = Instantiate(_ropeSegment);
            newSegment.transform.parent = gameObject.transform;
            newSegment.transform.position = transform.position;
            newSegment.name += $" {i}";
            newSegment.GetComponent<HingeJoint2D>().connectedBody = prevBod;

            prevBod = newSegment.GetComponent<Rigidbody2D>();
        }
        var lastSegment = Instantiate(_lastSegment);
        lastSegment.transform.parent = gameObject.transform;
        lastSegment.transform.position = transform.position;
        lastSegment.GetComponent<HingeJoint2D>().connectedBody = prevBod;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        for (int i = 0; i < _numberOfLinks; i++)
        {
            Gizmos.DrawWireCube(transform.position + (new Vector3(0, -0.5f) * i) + new Vector3(0, -0.25f), new Vector3(0.2f, 0.5f));
        }
        var maxAngle = _ropeSegment.GetComponent<HingeJoint2D>().limits.max;
        var minAngle = _ropeSegment.GetComponent<HingeJoint2D>().limits.min;

        Gizmos.DrawLine(transform.position, (transform.position + new Vector3(0, -0.25f * _numberOfLinks)).RotateAroundPoint(transform.position, new Vector3(0, 0, maxAngle)));
        Gizmos.DrawLine(transform.position, (transform.position + new Vector3(0, -0.25f * _numberOfLinks)).RotateAroundPoint(transform.position, new Vector3(0, 0, minAngle)));

    }
}
