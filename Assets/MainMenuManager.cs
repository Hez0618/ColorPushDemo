using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Button startButton;
    public Button settingsButton;
    public Button quitButton;

    private void Start()
    {
        startButton.onClick.AddListener(StartGame);
        settingsButton.onClick.AddListener(OpenSettings);
        quitButton.onClick.AddListener(QuitGame);
    }

    void StartGame()
    {
        SceneLoader.sceneToLoad = "Level-1";
        SceneManager.LoadScene("LoadingScene");
    }

    void OpenSettings()
    {
        Debug.Log("settings");
    }

    void QuitGame()
    {
        Debug.Log("Exit");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

