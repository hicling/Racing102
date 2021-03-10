using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Dropdown resolutionDropdown;
    [SerializeField] private Dropdown textureQualityDropdown;
    [SerializeField] private Dropdown antialiasingDropdown;
    [SerializeField] private Dropdown vSyncDropdown;
    [SerializeField] private Slider audioVolumeSliter;

    public Resolution[] resolutions;
    public GameSettings gameSettings;

    private void OnEnable()
    {
        gameSettings = new GameSettings();

        fullscreenToggle.onValueChanged.AddListener(delegate { OnFullscreenToggle(); });
        resolutionDropdown.onValueChanged.AddListener(delegate { OnResolutionChange(); });
        textureQualityDropdown.onValueChanged.AddListener(delegate { OnTextureQualityChange(); });
        antialiasingDropdown.onValueChanged.AddListener(delegate { OnAntialiasingChange(); });
        vSyncDropdown.onValueChanged.AddListener(delegate { OnVSyncChange(); });

        resolutions = Screen.resolutions;
    }

    public void OnFullscreenToggle()
    {
        gameSettings.fullscreen = Screen.fullScreen = fullscreenToggle.isOn;
    }

    public void OnResolutionChange()
    {

    }

    public void OnTextureQualityChange()
    {
        gameSettings.textureQuality = QualitySettings.masterTextureLimit = textureQualityDropdown.value;
    }

    public void OnAntialiasingChange()
    {

    }

    public void OnVSyncChange()
    {

    }

    public void SaveSettings()
    {

    }

    public void LoadSettings()
    {

    }
}
