using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSegment : MonoBehaviour
{
    [SerializeField] GameObject _connectedAbove;
    [SerializeField] GameObject _connectedBelow;

    public GameObject Above => _connectedAbove;

    public GameObject Below => _connectedBelow;
    

    // Start is called before the first frame update
    void Start()
    {
        _connectedAbove = GetComponent<HingeJoint2D>().connectedBody.gameObject;
        var aboveSegment = _connectedAbove.GetComponent<RopeSegment>();
        if (aboveSegment != null)
        {
            aboveSegment._connectedBelow = gameObject;
            float spriteBottom = _connectedAbove.GetComponent<SpriteRenderer>().sprite.bounds.size.y;
            GetComponent<HingeJoint2D>().connectedAnchor = new Vector2(0, spriteBottom * -1);
        }
        else
        {
            GetComponent<HingeJoint2D>().connectedAnchor = Vector2.zero;
        }
    }

}
