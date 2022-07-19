using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundVolumeSlider : MonoBehaviour
{
    [SerializeField] Slider _slider;

    void Start()
    {
        _slider.value = PlayerPrefsController.SfxVolume;
        _slider.onValueChanged.AddListener((a) => 
        {
            PlayerPrefsController.SetSfxVolume(a);
            SoundManager.Instance.SoundVolume = a;
        });
    }

    
}
