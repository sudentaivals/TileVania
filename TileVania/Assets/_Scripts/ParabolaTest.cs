using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParabolaTest : MonoBehaviour
{
    [Header("Shooting area")]
    [SerializeField] float _boxWidth;
    [SerializeField] float _boxHeight;
    [SerializeField] LayerMask _targetLayer;
    [SerializeField] Vector2 _boxOffset;
    [Header("Shooting stats")]
    [SerializeField] float _cooldown = 1f;
    [SerializeField] float _parabolaHeight;
    [SerializeField] GameObject _bomb;
    [SerializeField] Transform _cannon;
    [SerializeField] Transform _shootingPosition;
    [Header("SFX and VFX")]
    [SerializeField] GameObject _shootingVFX;
    [SerializeField] AudioClip _launchSfx;
    [SerializeField][Range(0f, 1f)] float _launchSfxVolume;

    private float _currentCooldown = 0f;
    private Collider2D[] _targets = new Collider2D[1];
    private bool IsTargetInRange => Physics2D.OverlapBoxNonAlloc((Vector2)transform.position + _boxOffset, new Vector2(_boxWidth, _boxHeight), 0, _targets, _targetLayer) > 0;

    private bool IsShootReady => _currentCooldown <= 0;

    void Start()
    {
        
    }

    void Update()
    {
        if (IsShootReady)
        {
            if (IsTargetInRange)
            {
                Shoot();
                _currentCooldown = _cooldown;

            }
        }
        else
        {
            _currentCooldown -= Time.deltaTime;
        }
    }

    private void LookAtTarget(Vector2 target)
    {
        var cannonToTarget = target - (Vector2)_cannon.position;
        var angle = Mathf.Atan2(cannonToTarget.y, cannonToTarget.x);
        _cannon.localEulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * angle);
    }

    private void Shoot()
    {
        Vector2 p1 = _shootingPosition.transform.position;
        Vector2 p3 = _targets[0].transform.position;
        var p2 = new Vector2(Mathf.Lerp(p1.x, p3.x, 0.5f), Mathf.Max(p1.y, p3.y) + _parabolaHeight);
        LookAtTarget(p2);

        var vfx = Instantiate(_shootingVFX, _shootingPosition);
        Destroy(vfx.gameObject, 1f);
        EventBus.Publish(GameplayEventType.PlaySound, this, new PlaySoundEventArgs(_launchSfxVolume, _launchSfx));

        var bomb = Instantiate(_bomb);
        bomb.transform.position = _shootingPosition.transform.position;
        bomb.GetComponent<WaypointMovement>().SetWaypoints(ParabolaBuilder.GetParabolaPoints(p1, p2, p3, 15));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(transform.position + (Vector3)_boxOffset, new Vector3(_boxWidth, _boxHeight, 0));
    }
}
