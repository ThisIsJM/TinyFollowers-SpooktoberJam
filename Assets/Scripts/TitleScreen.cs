using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : MonoBehaviour
{
   public void Play()
   {
        StartCoroutine(GameManager.instance.LoadScene("Chapter Menu"));
   }

    public void SettingsMenu()
    {
        GameManager.instance.OpenOptionsMenu();
    }

    public void ClickButtonSFX()
    {
        GameManager.instance.ClickButtonSFX();
    }


}
