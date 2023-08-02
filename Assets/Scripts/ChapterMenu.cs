using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterMenu : MonoBehaviour
{
    public void LoadLevelMenu(string levelChapter)
    {
        LevelManager.instance.currentChapter = levelChapter;
        StartCoroutine(GameManager.instance.LoadScene("Level Menu"));
    }
}
