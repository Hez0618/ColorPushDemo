using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Button startButton;
    public Button settingsButton;
    public Button quitButton;


    public Animator mainMenu_animator;
    public Animator Back_animator;

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

    public void OnClickLevelSelect()
    {
        mainMenu_animator.SetTrigger("ShowLevelSelect");
    }

    public void OnClickBackButton()
    {
        Back_animator.SetTrigger("BackToMenu");
    }

    public void OnClickLevelButton(int levelIndex)
    {
        SceneLoader.sceneToLoad = $"Level-{levelIndex}";
        SceneManager.LoadScene("LoadingScene");
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

