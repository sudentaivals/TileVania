using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    [SerializeField] [Range(0, 1)] float _highlightVolume;
    [SerializeField] AudioClip _highlightClip;

    [SerializeField] [Range(0, 1)] float _clickVolume;
    [SerializeField] AudioClip _clickClip;
    public void PlayHighlightSound()
    {
        if (!gameObject.GetComponent<Button>().interactable) return;
        EventBus.Publish(GameplayEventType.PlaySound, this, new PlaySoundEventArgs(_highlightVolume, _highlightClip));
    }
    public void PlayClickSound()
    {
        EventBus.Publish(GameplayEventType.PlaySound, this, new PlaySoundEventArgs(_clickVolume, _clickClip));
    }

    public void RestartLevel()
    {
        SceneLoader.Instance.RestartScene();
    }

    public void LoadNextLevel()
    {
        SceneLoader.Instance.LoadNextScene();
    }

    public void LoadMainMenu()
    {
        SceneLoader.Instance.LoadMainMenu();
    }

    public void ResumeGame()
    {
        GameManager.Instance.TriggerGameState(GameState.Play);
    }

    public void PauseGame()
    {
        GameManager.Instance.TriggerGameState(GameState.Pause);
    }

}
