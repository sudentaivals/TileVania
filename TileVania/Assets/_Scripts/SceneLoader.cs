using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : SingletonInstance<SceneLoader>
{
    [SerializeField] GameObject _canvas;
    [SerializeField] Image _progressBar;

    public void RestartScene()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private async void LoadScene(int sceneIndex)
    {
        EventBus.Publish(GameplayEventType.LevelLoading, this, new System.EventArgs());
        var scene = SceneManager.LoadSceneAsync(sceneIndex);
        scene.allowSceneActivation = false;

        _canvas.SetActive(true);

        do
        {
            _progressBar.fillAmount = scene.progress;
        } while (scene.progress < 0.9f);
        _progressBar.fillAmount = 1.0f;
        await Task.Delay(250);
        scene.allowSceneActivation = true;
        _canvas.SetActive(false);
    }

    public void LoadNextScene()
    {
        var currentIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentIndex >= SceneManager.sceneCount - 1)
        {
            LoadMainMenu();
        }
        else
        {
            LoadScene(currentIndex+1);
        }
    }

    public void LoadMainMenu()
    {
        LoadScene(0);
    }

}
