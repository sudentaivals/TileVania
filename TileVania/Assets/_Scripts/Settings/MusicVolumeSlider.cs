using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicVolumeSlider : MonoBehaviour
{
    [SerializeField] Slider _slider;
    void Start()
    {
        _slider.value = PlayerPrefsController.MusicVolume;
        _slider.onValueChanged.AddListener((a) =>
        {
            PlayerPrefsController.SetMusicVolume(a);
            MusicManager.Instance.MusicVolume = a;
        });
    }

}
