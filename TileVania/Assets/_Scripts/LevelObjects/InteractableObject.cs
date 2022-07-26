using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class InteractableObject : MonoBehaviour
{
    [SerializeField] float _playerCheckerRadius;
    [SerializeField] LayerMask _playerMask;
    [SerializeField] Vector2 _playerCheckerOffset;
    [SerializeField] UnityEvent _onInteract;
    [Header("Sound")]
    [SerializeField][Range(0f, 1f)] float _sfxVolume = 1f;
    [SerializeField] AudioClip _clip;
    [SerializeField] Material _material;
    private Collider2D[] _playerCollision = new Collider2D[1];

    protected bool _isActive = true;

    private Material _materialClone;
    protected bool IsPlayerInRange => Physics2D.OverlapCircleNonAlloc(transform.position, _playerCheckerRadius, _playerCollision, _playerMask) > 0;

    private void Awake()
    {
        _materialClone = new Material(_material);
        GetComponent<SpriteRenderer>().material = _materialClone;
    }

    private void OnEnable()
    {
        EventBus.Subscribe(GameplayEventType.GameOver, StopInteracting);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(GameplayEventType.GameOver, StopInteracting);
    }

    protected virtual void Update()
    {
        if (IsPlayerInRange && _isActive)
        {
            _materialClone.SetFloat("_Thickness", 5);
        }
        else
        {
            _materialClone.SetFloat("_Thickness", 0);
        }
    }

    private void StopInteracting(UnityEngine.Object sender, EventArgs args)
    {
        _isActive = false;
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        Gizmos.DrawWireSphere(transform.position + (Vector3)_playerCheckerOffset, _playerCheckerRadius);
    }

    protected void Interact()
    {
        if (!_isActive) return;
        _onInteract?.Invoke();
        EventBus.Publish(GameplayEventType.PlaySound, this, new PlaySoundEventArgs(_sfxVolume, _clip));
    }
}
