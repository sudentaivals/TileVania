using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthSystem : MonoBehaviour
{
    [Header("Killers")]
    [SerializeField] Transform _killChecker;
    [SerializeField] float _killBoxWidth;
    [SerializeField] float _killBoxHeight;
    [SerializeField] Vector2 _killBoxOffset;
    [SerializeField] LayerMask _killLayers;
    [Header("SFX")]
    [SerializeField] AudioClip _deathSfx;
    [SerializeField] [Range(0f, 1f)] float _deathSfxVolume;

    private readonly Collider2D[] _enemyHits = new Collider2D[1];

    private PlayerController _pc;
    private void DeathTrigger()
    {
        EventBus.Publish(GameplayEventType.PlaySound, this, new PlaySoundEventArgs(_deathSfxVolume, _deathSfx));
        GameManager.Instance.TriggerGameState(GameState.Lose);
    }

    void Start()
    {
        _pc = GetComponent<PlayerController>();
    }

    private void FixedUpdate()
    {
        if (!_pc.IsAlive) return;
        if(Physics2D.OverlapBoxNonAlloc(_killChecker.position + (Vector3)_killBoxOffset, new Vector2(_killBoxWidth, _killBoxHeight), transform.eulerAngles.z, _enemyHits, _killLayers) > 0)
        {
            DeathTrigger();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(_killChecker.position + (Vector3)_killBoxOffset, new Vector3(_killBoxWidth, _killBoxHeight, 0));
    }
}
