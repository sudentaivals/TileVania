using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectionCanvas : MonoBehaviour
{
    [SerializeField] Transform _scrollListContent;
    [SerializeField] GameObject _levelButton;
    [SerializeField] LevelInformation _levelInfo;
    [SerializeField] Button _startButton;

    private int _levelIndex;

    private List<LevelData> _dataList;
    void Start()
    {
        _dataList = new List<LevelData>();
        FillContent();
    }

    private void FillContent()
    {
        //first level - menu
        var levelCount = SceneManager.sceneCountInBuildSettings - 1;
        for (int i = 0; i < levelCount; i++)
        {
            //create button
            var levelButton = Instantiate(_levelButton);
            levelButton.transform.SetParent(_scrollListContent);
            levelButton.transform.localScale = new Vector3(1, 1, 1);
            string levelTitle = $"LEVEL {i+1}";
            levelButton.GetComponent<TextMeshProUGUI>().text = $"LEVEL {i+1}";
            //create button events
            var button = levelButton.GetComponent<Button>();
            var data = SaveSystem.GetLevelData(i + 1);
            _dataList.Add(data);
            if (i == 0)
            {
                button.interactable = true;
            }
            else
            {
                if (_dataList[i - 1].IsCompleted)
                {
                    button.interactable = true;
                }
                else
                {
                    button.interactable = false;
                }
            }
            int index = i + 1;
            button.onClick.AddListener(() =>
            {
                _levelIndex = index;
                ChangeStartButtonState(true);
                _levelInfo.UpdateTitle(levelTitle);
                _levelInfo.UpdateDeaths(data.DeathCount);
                if (data.IsCompleted)
                {
                    _levelInfo.UpdateTime(data.BestTime);
                }
            });

        }
    }

    public void StartGame()
    {
        SceneLoader.Instance.LoadScene(_levelIndex);
    }

    public void OpenCanvas()
    {
        foreach (Transform tr in transform)
        {
            tr.gameObject.SetActive(true);
        }
    }

    public void CloseCanvas()
    {
        foreach (Transform tr in transform)
        {
            tr.gameObject.SetActive(false);
        }
        ChangeStartButtonState(false);
        _levelInfo.Clear();
    }

    public void ChangeStartButtonState(bool state)
    {
        _startButton.interactable = state;
    }


}
