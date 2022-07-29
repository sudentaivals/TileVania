using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelInformation : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _levelName;
    [SerializeField] TextMeshProUGUI _deathsText;
    [SerializeField] TextMeshProUGUI _timerText;

    public void UpdateDeaths(int deaths)
    {
        _deathsText.text = deaths.ToString();
    }

    public void Clear()
    {
        _deathsText.text = "-";
        _levelName.text = "LEVEL $";
        _timerText.text = "--:--:--";
    }

    public void UpdateTime(float time)
    {
        if(time <= 0)
        {
            _timerText.text = "--:--:--";
        }
        else
        {
            int minutes = (int)time / 60;
            int seconds = (int)time - minutes * 60;
            int milliseconds = (int)((time - minutes * 60 - seconds) * 100);
            _timerText.text = minutes.ToString("D2") + ":" + seconds.ToString("D2") + ":" + milliseconds.ToString("D2");
        }
    }

    public void UpdateTitle(string newTitle)
    {
        _levelName.text = newTitle;
    }

}
