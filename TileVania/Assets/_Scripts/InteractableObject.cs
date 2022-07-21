using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class InteractableObject : MonoBehaviour
{
    [SerializeField] float _playerCheckerRadius;
    [SerializeField] LayerMask _playerMask;
    [SerializeField] Vector2 _playerCheckerOffset;
    [SerializeField] UnityEvent _onInterract;
    [Header("Sound")]
    [SerializeField][Range(0f, 1f)] float _sfxVolume = 1f;
    [SerializeField] AudioClip _clip;
    [SerializeField] Material _material;
    private Collider2D[] _playerCollision = new Collider2D[1];

    private Material _materialClone;
    protected bool IsPlayerInRange => Physics2D.OverlapCircleNonAlloc(transform.position, _playerCheckerRadius, _playerCollision, _playerMask) > 0;

    private void Awake()
    {
        _materialClone = new Material(_material);
        GetComponent<SpriteRenderer>().material = _materialClone;
    }

    protected virtual void Update()
    {
        if (IsPlayerInRange)
        {
            _materialClone.SetFloat("_Thickness", 5);
        }
        else
        {
            _materialClone.SetFloat("_Thickness", 0);
        }
    }


    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        Gizmos.DrawWireSphere(transform.position + (Vector3)_playerCheckerOffset, _playerCheckerRadius);
    }

    protected void Interract()
    {
        _onInterract?.Invoke();
        EventBus.Publish(GameplayEventType.PlaySound, this, new PlaySoundEventArgs(_sfxVolume, _clip));
    }
}
