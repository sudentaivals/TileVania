using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] CircleCollider2D _collider;
    [SerializeField] float _knockbackRadius;
    [SerializeField] float _explosionRadius;
    [SerializeField] float _knockbackForce;
    [SerializeField] LayerMask _affectedObjects;
    [SerializeField] AudioClip _explosionSfx;
    [SerializeField][Range(0f, 1f)] float _explosionSfxVolume = 1f;


    private void SetExplosionRadius()
    {
        var newScale = _explosionRadius / 0.5f;
        transform.localScale = new Vector3(newScale, newScale);
    }

    private void Awake()
    {
        SetExplosionRadius();
        _collider.enabled = true;
    }
    void Start()
    {
        EventBus.Publish(GameplayEventType.PlaySound, this, new PlaySoundEventArgs(_explosionSfxVolume, _explosionSfx));
        var affectedObjects = Physics2D.OverlapCircleAll(transform.position, _knockbackRadius, _affectedObjects);
        //knockback
        foreach (var affected in affectedObjects)
        {
            affected.GetComponent<PlayerController>().PushForConcreteTime(0.1f);
            var direction = (affected.transform.position - transform.position).normalized;
            var force = Mathf.Lerp(_knockbackForce, _knockbackForce * 0.05f, (affected.transform.position - transform.position).magnitude / _knockbackRadius);
            affected.attachedRigidbody.AddForce(direction * force, ForceMode2D.Impulse);
        }
        StartCoroutine(DisableCollider());
    }

    private IEnumerator DisableCollider()
    {
        yield return new WaitForSeconds(0.1f);
        _collider.enabled = false;
    }


    private void DestroyObject()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;

        Gizmos.DrawWireSphere(transform.position, _explosionRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _knockbackRadius);
    }

}
