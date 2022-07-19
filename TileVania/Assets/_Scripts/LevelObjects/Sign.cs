using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Sign : MonoBehaviour
{
    [SerializeField] float _signRadius = 1f;
    [SerializeField][TextArea] string _text = "blank";
    [SerializeField] LayerMask _playerMask;
    [SerializeField] TextMeshPro _tmp;
    private Collider2D[] _playerCollision = new Collider2D[1];
    private bool IsPlayerNear => Physics2D.OverlapCircleNonAlloc(transform.position, _signRadius, _playerCollision, _playerMask) > 0;

    void Start()
    {
        _tmp.text = _text;
    }

    void Update()
    {
        if (IsPlayerNear)
        {
            float distanceBetweenPlayerAndSign = ((Vector2)_playerCollision[0].transform.position - (Vector2)transform.position).magnitude;
            float alpha = Mathf.Lerp(1, 0.1f, distanceBetweenPlayerAndSign / _signRadius);
            _tmp.color = new Color(_tmp.color.r, _tmp.color.g, _tmp.color.g, alpha);
        }
        else
        {
            _tmp.color = new Color(_tmp.color.r, _tmp.color.g, _tmp.color.g, 0);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        Gizmos.DrawWireSphere(transform.position, _signRadius);
    }
}
