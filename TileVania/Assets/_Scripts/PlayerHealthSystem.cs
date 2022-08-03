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
    [SerializeField] List<AudioClip>  _deathSfx;
    [SerializeField] [Range(0f, 1f)] float _deathSfxVolume;

    private readonly Collider2D[] _enemyHits = new Collider2D[1];

    private bool IsTouchEnemyOrTrap => Physics2D.OverlapBoxNonAlloc(_killChecker.position + (Vector3)_killBoxOffset, new Vector2(_killBoxWidth, _killBoxHeight), transform.eulerAngles.z, _enemyHits, _killLayers) > 0;

    private PlayerController _pc;
    private void DeathTrigger()
    {
        PublishDeathSound();
        GameManager.Instance.TriggerGameState(GameState.Lose);
    }

    private void PublishDeathSound()
    {
        int clipIndex = Random.Range(0, _deathSfx.Count);
        EventBus.Publish(GameplayEventType.PlaySound, this, new PlaySoundEventArgs(_deathSfxVolume, _deathSfx[clipIndex]));
    }

    void Start()
    {
        _pc = GetComponent<PlayerController>();
    }

    private void FixedUpdate()
    {
        if (!_pc.IsUnderControl) return;
        if (IsTouchEnemyOrTrap)
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
