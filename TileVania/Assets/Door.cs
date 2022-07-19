using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] [Range(0, 1)] float _closeSfxVolume = 1;
    [SerializeField] AudioClip _clip;

    const string CLOSE_DOOR = "Close";
    const string OPEN_DOOR = "Open";

    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void CloseDoor()
    {
        EventBus.Publish(GameplayEventType.PlaySound, this, new PlaySoundEventArgs(_closeSfxVolume, _clip));
        _animator.Play(CLOSE_DOOR);
    }

    public void OpenDoor()
    {
        EventBus.Publish(GameplayEventType.PlaySound, this, new PlaySoundEventArgs(_closeSfxVolume, _clip));
        _animator.Play(OPEN_DOOR);
    }

    private void TurnColliderOff()
    {
        GetComponent<BoxCollider2D>().enabled = false;
    }

    private void TurnColliderOn()
    {
        GetComponent<BoxCollider2D>().enabled = true;
    }
}
