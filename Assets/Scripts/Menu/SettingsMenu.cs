using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public TMP_Dropdown graphicsQuality;
    public Slider fieldOfView;
    public Toggle postProcessing;

    private void Start()
    {
        graphicsQuality.value = (int)Settings.graphicsQuality;
        fieldOfView.value = Settings.fov;
        postProcessing.isOn = Settings.postProcessing;

        ApplyChanges();
    }
    private void Update()
    {
        Settings.graphicsQuality = (Settings.GraphicsQuality)graphicsQuality.value;
        Settings.fov = fieldOfView.value;
        Settings.postProcessing = postProcessing.isOn;
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void ApplyChanges()
    {
        QualitySettings.SetQualityLevel((int)Settings.graphicsQuality);
    }
}
