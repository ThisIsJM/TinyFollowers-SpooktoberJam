using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class OptionsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    public GameObject pauseMenu;
    [Header("Audio Slider")]
    public Slider sfxSlider;
    public Slider bgmSlider;

    [Header("Resolution Dropdown")]
    public TMPro.TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions;

    private bool isFullScreen = true;

    private void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;

            if (!options.Contains(option))
            {
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }
          
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        sfxSlider.value = AudioManager.instance.sfxValue;
        bgmSlider.value = AudioManager.instance.bgmValue;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, isFullScreen);
    }

    public void SetFullScreen(bool toggle)
    {
        isFullScreen = toggle;
        Screen.fullScreen = isFullScreen;
    }

    public void SetSfxVolume(float volume)
    {
        audioMixer.SetFloat("SFX",volume);
        AudioManager.instance.sfxValue = volume;
    }

    public void SetBgmVolume(float volume)
    {
        audioMixer.SetFloat("BGM", volume);
        AudioManager.instance.bgmValue = volume;
    }

    public void ExitOptions()
    {
        if (LevelManager.instance.InALevelScene(SceneManager.GetActiveScene().path))
        {
            pauseMenu.SetActive(true);
        }
        this.gameObject.SetActive(false);
    }
}
