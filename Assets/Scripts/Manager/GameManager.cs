using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("Screen Transition")]
    public GameObject screenTransition;
    public float transitionDelay;
    bool loadingNextLevel = false;
    Animator screenAnim;

    [Header("UI")]
    public TextMeshProUGUI levelName;
    public TextMeshProUGUI skipText;
    public GameObject backButton;
    public GameObject pauseMenu;
    public GameObject optionsMenu;

    [Header("Post Processing")]
    public ForwardRendererData feature;
    private bool isPaused = false;

    private int maxRestart = 3;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        

        if (SceneManager.GetActiveScene().name == "BaseLevel")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            SceneManager.LoadSceneAsync("BaseLevel", LoadSceneMode.Additive);
        }
        if (!LevelManager.instance.InALevelScene(SceneManager.GetActiveScene().path))
        {
           GameObject[] screenTexts = GameObject.FindGameObjectsWithTag("Base UI");

            foreach (GameObject ui in screenTexts)
            {
                ui.SetActive(false);
            }
        }
        else
        {
            levelName.text = SceneManager.GetActiveScene().name;

            if (LevelManager.instance.restartCount >= maxRestart)
            {
                skipText.gameObject.SetActive(true);
            }
            else
            {
                skipText.gameObject.SetActive(false);
            }
        }

        RenderToggle();
        ShowBackButton();
        screenTransition.SetActive(true);
        screenAnim = screenTransition.GetComponent<Animator>();
    }

    void RenderToggle()
    {
        if (LevelManager.instance.InALevelScene(SceneManager.GetActiveScene().path))
        {
            feature.rendererFeatures[1].SetActive(true);

            if (LevelManager.instance.FindLevelChapter(SceneManager.GetActiveScene().path) == 2)
            {
                feature.rendererFeatures[0].SetActive(true);

                if (AudioManager.instance.isPlaying("BGM") && !AudioManager.instance.isPlaying("BGM 2")) // Plays the BGM 2
                {
                    AudioManager.instance.StopPlaying("BGM");
                    AudioManager.instance.Play("BGM 2");
                }
            }
            else
            {
                if (!AudioManager.instance.isPlaying("BGM") && AudioManager.instance.isPlaying("BGM 2")) // Plays the BGM
                {
                    feature.rendererFeatures[0].SetActive(false);

                    AudioManager.instance.StopPlaying("BGM 2");
                    AudioManager.instance.Play("BGM");
                }
            }
        }
        else
        {
            feature.rendererFeatures[1].SetActive(false);
            feature.rendererFeatures[0].SetActive(false);
        }
    }

    void ShowBackButton()
    {
        if (SceneManager.GetActiveScene().name.Equals("Title Page"))
        {
            backButton.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && LevelManager.instance.InALevelScene(SceneManager.GetActiveScene().path))
        {
            Debug.Log(LevelManager.instance.restartCount);
            StartCoroutine(RestartLevel());
        }
        if ((Input.GetKeyDown(KeyCode.Escape) ||(Input.GetKeyDown(KeyCode.P))) && LevelManager.instance.InALevelScene(SceneManager.GetActiveScene().path))
        {
            if (!isPaused)
            {
                Pause();
            }
            else if (isPaused && optionsMenu.activeInHierarchy)
            {
                optionsMenu.SetActive(false);
                pauseMenu.SetActive(true);
            }
            else
            {
                Resume();
            }
        }
        if (Input.GetKeyDown(KeyCode.N) && LevelManager.instance.restartCount >= maxRestart)
        {
            Debug.Log("Restart Level");
            StartCoroutine(LoadNextLevel());
        }
    }

    public IEnumerator LoadNextLevel()
    {
        loadingNextLevel = true;
        screenAnim.SetTrigger("Close");
        yield return new WaitForSeconds(transitionDelay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.LoadSceneAsync("BaseLevel", LoadSceneMode.Additive);
    }

    public IEnumerator RestartLevel()
    {
        screenAnim.SetTrigger("Close");
        yield return new WaitForSeconds(transitionDelay);
        LevelManager.instance.restartCount++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadSceneAsync("BaseLevel", LoadSceneMode.Additive);
    }

    public IEnumerator LoadScene(string sceneName)
    {
        loadingNextLevel = true;
        screenAnim.SetTrigger("Close");
        yield return new WaitForSeconds(transitionDelay);
        SceneManager.LoadScene(sceneName);
        SceneManager.LoadSceneAsync("BaseLevel", LoadSceneMode.Additive);
    }

    public void StartLoadSceneCoroutine(string sceneName)
    {
        Debug.Log("Loading Main Menu");
        Time.timeScale = 1f;
        StartCoroutine(LoadScene(sceneName));
    }

    public bool IsNextLevelLoading()
    {
        return loadingNextLevel;
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        isPaused = true;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        isPaused = false;
    }

    public void OpenOptionsMenu()
    {
        optionsMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    public void GoBackScene()
    {
        if (LevelManager.instance.InALevelScene(SceneManager.GetActiveScene().path))
        {
            StartCoroutine(LoadScene("Level Menu"));
        }
        else if (SceneManager.GetActiveScene().name.Equals("Level Menu"))
        {
            StartCoroutine(LoadScene("Chapter Menu"));
        }
        else if (SceneManager.GetActiveScene().name.Equals("Chapter Menu") || SceneManager.GetActiveScene().name.Equals("Credits"))
        {
            StartCoroutine(LoadScene("Title Page"));
        }
        else
        {
            Debug.Log("No Options");
        }
    }

    public void ClickButtonSFX()
    {
        AudioManager.instance.Play("Button Click");
    }

}
