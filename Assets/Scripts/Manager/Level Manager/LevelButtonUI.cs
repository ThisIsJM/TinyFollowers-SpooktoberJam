using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LevelButtonUI : MonoBehaviour
{
    private List<SceneLevel> sceneLevelList;

    public int firstPageThreshold;
    public Transform firstPage;
    public Transform secondPage;
    [Header("Level Button")]
    public Transform levelButton;

    [Header("Level Button Image")]
    public Sprite levelLocked;
    public Sprite levelUnlocked;

    private bool isChapterTwo = false;
    // Start is called before the first frame update
    void Start()
    {
        List<SceneLevel> placeHolderList = LevelManager.instance.GetSceneLevelList();
        sceneLevelList = new List<SceneLevel>();
        if (LevelManager.instance.currentChapter.Equals("Chapter One")) //Chapter One
        {
            GameObject.Find("Chapter Two").SetActive(false);
            for (int i = 0; i < placeHolderList.Count; i++)
            {
                if (placeHolderList[i].chapter == 1)
                { sceneLevelList.Add(placeHolderList[i]); }
            }
        }
        else //Chapter Two
        {
            isChapterTwo = true;
            GameObject.Find("Chapter One")?.SetActive(false);
            for (int i = 0; i < placeHolderList.Count; i++)
            {
                if (placeHolderList[i].chapter == 2)
                { sceneLevelList.Add(placeHolderList[i]); }
            }
        }

        LoadLevelButtons();
       
    }

    public void LoadLevelButtons()
    {
        //Instantiate the Button
        for (int i = 0; i < sceneLevelList.Count; i++)
        {
            SceneLevel levelData = sceneLevelList[i];

            Transform button = Instantiate(levelButton, transform);

            if (isChapterTwo)
            {
                button.GetComponentInChildren<TextMeshProUGUI>().text = (i + 16).ToString("D2");
            }
            else
            {
                button.GetComponentInChildren<TextMeshProUGUI>().text = (i + 1).ToString("D2");
            }

            if (i < firstPageThreshold)
            {
                button.SetParent(firstPage.transform);
            }
            else
            {
                button.SetParent(secondPage);
            }

            //Add Button Listener
            button.GetComponent<Button>().onClick.AddListener(delegate { LoadScene(levelData.sceneName); });

            //Check if sceneLevel has been unlocked
            if (!levelData.isUnlocked)
            {
                button.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
                button.GetComponent<Image>().sprite = levelLocked;
                button.GetComponent<Button>().interactable = false;
            }
        }
    }

    public void LoadScene(string levelName)
    {
        SceneManager.LoadScene(levelName);
        SceneManager.LoadSceneAsync("BaseLevel", LoadSceneMode.Additive);
    }
}
