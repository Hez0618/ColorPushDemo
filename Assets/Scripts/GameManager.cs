using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject pauseMenuUI;
    public Button menuToggleButton;
    public Button resumeButton;
    public Button settingsButton;
    public Button mainMenuButton;
    
    public Image fillImage;

    public float totalTime = 120f;
    private float elapsedTime = 0f;

    private bool isPaused = false;

    public bool isGameRunning = true;
    public bool isGameWon = false;

    private Vector3 playerStartPos;
    private Vector3[] boxStartPositions;
    private PushableBox[] allBoxes;
    private GameObject player;


    public Animator victoryPanelAnimator;

    public Image scoreLine1;
    public Image scoreLine2;
    public Image scoreLine3;

    public Image star1;
    public Image star2;
    public Image star3;


    public Button retryButton;
    public Button nextLevelButton;
    public Button backToMenuButton;

    private int tutorialPage = 0;
    public GameObject tutorialPanel;
    public CanvasGroup moveTutorialPanel;
    public CanvasGroup colorChangeTutorialPanel;
    public TextMeshProUGUI pageIndicatorText;
    public Button helpButton;

    public GameObject arrowLeftButton;
    public GameObject arrowRightButton;

    private bool isTutorialShowing = false;

    private int totalBoxCount;
    private int absorbedBoxCount = 0;

    public RectTransform fill;
    public RectTransform glowRect;
    public Image glowImage;
    private float pulseSpeed = 2f;   
    private float minAlpha = 0.1f;      
    private float maxAlpha = 1f;     


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {

        pauseMenuUI.SetActive(false);

        helpButton.onClick.AddListener(ToggleTutorialPanel);


        menuToggleButton.onClick.AddListener(TogglePauseMenu);
        resumeButton.onClick.AddListener(ResumeGame);
        mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        backToMenuButton.onClick.AddListener(ReturnToMainMenu);
        retryButton.onClick.AddListener(RestartLevel);
        nextLevelButton.onClick.AddListener(LoadNextLevel);

        settingsButton.onClick.AddListener(() => Debug.Log("settings"));

        player = GameObject.FindWithTag("Player");
        if (player != null)
            playerStartPos = player.transform.position;

        allBoxes = Object.FindObjectsByType<PushableBox>(FindObjectsSortMode.None);
        totalBoxCount = allBoxes.Length;
        absorbedBoxCount = 0;

        boxStartPositions = new Vector3[allBoxes.Length];
        for (int i = 0; i < allBoxes.Length; i++)
        {
            boxStartPositions[i] = allBoxes[i].transform.position;
        }

    }

    private void Update()
    {
        if (!isGameRunning||isGameWon) return;

        if (absorbedBoxCount >= totalBoxCount)
        {
            Debug.Log("胜利条件达成！");
            OnGameWin();
        }


        if (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(1f - elapsedTime / totalTime);
            fillImage.fillAmount = progress;
            glowRect.anchoredPosition = new Vector2(fill.rect.width*progress, 0f);
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
            Color c = glowImage.color;
            c.a = alpha;
            glowImage.color = c;
            UpdateScoreLines(progress);
            if (progress <= 0f)
            {
                GameOver();
            }
        }


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }


        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetLevel(); 
        }
    }

    private void UpdateScoreLines(float progress)
    {
        scoreLine1.color = GetAlphaColor(progress >= 0.75f);
        scoreLine2.color = GetAlphaColor(progress >= 0.5f); 
        scoreLine3.color = GetAlphaColor(progress >= 0.25f); 
    }

    private Color GetAlphaColor(bool active)
    {
        return new Color(1f, 1f, 1f, active ? 1f : 0.2f);
    }


    public void ToggleTutorialPanel()
    {
        isTutorialShowing = !isTutorialShowing;

        tutorialPanel.SetActive(isTutorialShowing);

        if (isTutorialShowing)
        {
            tutorialPage = 0;
            ShowCurrentTutorialPage();
        }
        else
        {

            moveTutorialPanel.alpha = 0f;
            moveTutorialPanel.blocksRaycasts = false;
            moveTutorialPanel.interactable = false;

            colorChangeTutorialPanel.alpha = 0f;
            colorChangeTutorialPanel.blocksRaycasts = false;
            colorChangeTutorialPanel.interactable = false;
        }
    }

    public void ShowCurrentTutorialPage()
    {
        bool isMovePage = (tutorialPage == 0);
        bool isColorPage = (tutorialPage == 1);


        moveTutorialPanel.alpha = isMovePage ? 1f : 0f;
        moveTutorialPanel.blocksRaycasts = isMovePage;
        moveTutorialPanel.interactable = isMovePage;

        colorChangeTutorialPanel.alpha = isColorPage ? 1f : 0f;
        colorChangeTutorialPanel.blocksRaycasts = isColorPage;
        colorChangeTutorialPanel.interactable = isColorPage;


        pageIndicatorText.text = $"{tutorialPage + 1} / 2";


        if (isMovePage)
        {
            Animator moveAnim = moveTutorialPanel.GetComponent<Animator>();
            if (moveAnim) moveAnim.Play("MoveTutorial", 0, 0f);
        }
        else if (isColorPage)
        {
            Animator colorAnim = colorChangeTutorialPanel.GetComponent<Animator>();
            if (colorAnim) colorAnim.Play("ToggleTutorial", 0, 0f);
        }


        arrowLeftButton.SetActive(tutorialPage == 1);
        arrowRightButton.SetActive(tutorialPage == 0);
    }

    public void NextTutorialPage()
    {
        tutorialPage = (tutorialPage + 1) % 2;
        ShowCurrentTutorialPage();
    }

    public void PrevTutorialPage()
    {
        tutorialPage = (tutorialPage + 1) % 2; 
        ShowCurrentTutorialPage();
    }

    public void OnGameWin()
    {
        isGameRunning = false;
        isGameWon = true;
        Debug.Log("Time Stopped!");
        int starsEarned = CalculateStars();
        StartCoroutine(DelayedVictorySequence(starsEarned));
    }

    private IEnumerator DelayedVictorySequence(int stars)
    {
        yield return new WaitForSeconds(1f); 

        victoryPanelAnimator.SetTrigger("Win");
        StartCoroutine(ShowStars(stars));
    }


    private int CalculateStars()
    {
        float progress = Mathf.Clamp01(1f - elapsedTime / totalTime);

        if (progress >= 0.75f) return 3;
        else if (progress >= 0.5f) return 2;
        else if (progress >= 0.25f) return 1;
        else return 0;
    }


    private IEnumerator ShowStars(int count)
    {
        Image[] stars = new Image[] { star1, star2, star3 };

        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].gameObject.SetActive(false); 
        }

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < count; i++)
        {
            stars[i].gameObject.SetActive(true);
            stars[i].color = new Color(1f, 1f, 1f, 0f);

            float duration = 0.3f;
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float alpha = Mathf.Lerp(0f, 1f, t / duration);
                stars[i].color = new Color(1f, 1f, 1f, alpha);
                yield return null;
            }

            yield return new WaitForSeconds(0.2f); 
        }
    }


    public void TogglePauseMenu()
    {
        isPaused = !isPaused;
        pauseMenuUI.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }


    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void GameOver()
    {
        isGameRunning = false;
        Debug.Log("GameOver！");

    }

    public void RestartLevel()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ResetLevel()
    {
        absorbedBoxCount = 0;
        if (player != null)
        {
            player.transform.position = playerStartPos;
            var rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;

            SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = Color.gray;
            }
        }


        for (int i = 0; i < allBoxes.Length; i++)
        {
            allBoxes[i].transform.position = boxStartPositions[i];
            var rb = allBoxes[i].GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }

    }

    public void LoadNextLevel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentIndex + 1 < SceneManager.sceneCountInBuildSettings)
        {
            SceneLoader.sceneToLoad = $"Level-{currentIndex}";
            SceneManager.LoadScene("LoadingScene");
        }
        else
        {
            //No Next Level
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void OnBoxAbsorbed()
    {
        absorbedBoxCount++;
        Debug.Log($"箱子已吸入: {absorbedBoxCount} / {totalBoxCount}");

        if (absorbedBoxCount >= totalBoxCount && isGameRunning && !isGameWon)
        {
            Debug.Log("所有箱子已吸入，触发胜利！");
            OnGameWin();
        }
    }


}


