using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.IO;

public class SettingManager : MonoBehaviour
{
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Dropdown resolutionDropdown;
    [SerializeField] private Dropdown textureQualityDropdown;
    [SerializeField] private Dropdown antialiasingDropdown;
    [SerializeField] private Dropdown vSyncDropdown;
    [SerializeField] private Slider audioVolumeSliter;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Button applyButton;

    public Resolution[] resolutions;
    public GameSettings gameSettings;

    private void Start()
    {
        gameSettings = new GameSettings();

        fullscreenToggle.onValueChanged.AddListener(delegate { OnFullscreenToggle(); });
        resolutionDropdown.onValueChanged.AddListener(delegate { OnResolutionChange(); });
        textureQualityDropdown.onValueChanged.AddListener(delegate { OnTextureQualityChange(); });
        antialiasingDropdown.onValueChanged.AddListener(delegate { OnAntialiasingChange(); });
        vSyncDropdown.onValueChanged.AddListener(delegate { OnVSyncChange(); });
        audioVolumeSliter.onValueChanged.AddListener(delegate { OnVolumeChange(); });
        applyButton.onClick.AddListener(delegate { OnApplyButtonClick(); });

        resolutions = Screen.resolutions;
        foreach (Resolution resolution in resolutions)
        {
            resolutionDropdown.options.Add(new Dropdown.OptionData(resolution.ToString()));
        }

        if (File.Exists(Application.persistentDataPath + "/gamesettings.json") == true)
        {
            LoadSettings();
        }
    }

    public void OnFullscreenToggle()
    {
        Screen.fullScreen = gameSettings.fullscreen = fullscreenToggle.isOn;
    }

    public void OnResolutionChange()
    {
        Screen.SetResolution(resolutions[resolutionDropdown.value].width, resolutions[resolutionDropdown.value].height, Screen.fullScreen);
        gameSettings.resolutionIndex = resolutionDropdown.value;
    }

    public void OnTextureQualityChange()
    {
        QualitySettings.masterTextureLimit = gameSettings.textureQuality = textureQualityDropdown.value;
    }

    public void OnAntialiasingChange()
    {
        QualitySettings.antiAliasing = gameSettings.antialiasing = (int)Mathf.Pow(2, antialiasingDropdown.value);
    }

    public void OnVSyncChange()
    {
        QualitySettings.vSyncCount = gameSettings.vSync = vSyncDropdown.value;
    }

    public void OnVolumeChange()
    {
        audioMixer.SetFloat("volume", audioVolumeSliter.value);
        gameSettings.volume = audioVolumeSliter.value;
    }

    public void OnApplyButtonClick()
    {
        SaveSettings();
    }

    public void SaveSettings()
    {
        string jsonData = JsonUtility.ToJson(gameSettings, true);
        File.WriteAllText(Application.persistentDataPath + "/gamesettings.json", jsonData);
    }

    public void LoadSettings()
    {
        gameSettings = JsonUtility.FromJson<GameSettings>(File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));

        audioVolumeSliter.value = gameSettings.volume;
        antialiasingDropdown.value = gameSettings.antialiasing;
        vSyncDropdown.value = gameSettings.vSync;
        textureQualityDropdown.value = gameSettings.textureQuality;
        resolutionDropdown.value = gameSettings.resolutionIndex;
        fullscreenToggle.isOn = gameSettings.fullscreen;
        Screen.fullScreen = gameSettings.fullscreen;

        resolutionDropdown.RefreshShownValue();
    }
}
