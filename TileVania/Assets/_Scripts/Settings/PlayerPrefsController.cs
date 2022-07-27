using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsController
{
    const string SFX_VOLUME_KEY = "sfxVolume";
    const string MUSIC_VOLUME_KEY = "musicVolume";

    const float MIN_VOLUME = 0f;

    const float MAX_VOLUME = 1f;

    const float DEFAULT_VOLUME = 1f;

    public static float SfxVolume
    {
        get
        {
            if (!PlayerPrefs.HasKey(SFX_VOLUME_KEY))
            {
                PlayerPrefs.SetFloat(SFX_VOLUME_KEY, DEFAULT_VOLUME);
            }
            return PlayerPrefs.GetFloat(SFX_VOLUME_KEY);
        }
    }

    public static float MusicVolume
    {
        get
        {
            if (!PlayerPrefs.HasKey(MUSIC_VOLUME_KEY))
            {
                PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, DEFAULT_VOLUME);
            }
            return PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY);
        }
    }

    public static void SetSfxVolume(float volumeValue)
    {
        var checkedValue = Mathf.Clamp(volumeValue, MIN_VOLUME, MAX_VOLUME);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, checkedValue);
    }

    public static void IncreaseDeathCounter(string levelKey)
    {
        var newValue = GetDeathCounter(levelKey);
        PlayerPrefs.SetInt(levelKey, newValue + 1);
    }

    public static int GetDeathCounter(string levelKey)
    {
        return PlayerPrefs.GetInt(levelKey);
    }

    public static void SetMusicVolume(float volumeValue)
    {
        var checkedValue = Mathf.Clamp(volumeValue, MIN_VOLUME, MAX_VOLUME);
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, checkedValue);
    }

}

