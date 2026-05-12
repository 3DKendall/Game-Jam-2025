using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public Transform sun;

    [Header("Cycle Settings")]
    public float timeOfDay = 700f;
    public float cycleDuration = 1440f; // Full in-game day in minutes (24 hours * 60 minutes)
    public float dayStartTime = 480f; // 8:00 AM in in-game minutes
    public float dayEndTime = 1080f; // 6:00 PM in in-game minutes
    [Space]
    public float cycleSpeed = 0.8f; // Speed factor to achieve 30 real-time minutes for a full day cycle

    [Header("Lighting Settings")]
    public float dayTimeSunIntensity = 1f;
    public float nightTimeSunIntensity = 0;
    [Space]
    public float dayTimeAmbientIntensity = 1;
    public float nightTimeAmbientIntensity = 0.15f;
    [Space]
    [Space]
    public float intensityChangeSpeed = 1f;
    public Material skybox;
    public Color dayTimeColor;
    public Color nightTimeColor;

    [HideInInspector] public bool isNightTime;

    private Light sunLight;

    private void Start()
    {
        sunLight = sun.GetComponentInChildren<Light>();
        sunLight.intensity = isNightTime ? nightTimeSunIntensity : dayTimeSunIntensity;
    }

    private void Update()
    {
        float targetIntensity = isNightTime ? nightTimeSunIntensity : dayTimeSunIntensity;
        float targetAmbientIntensity = isNightTime ? nightTimeAmbientIntensity : dayTimeAmbientIntensity;

        sunLight.intensity = Mathf.Lerp(sunLight.intensity, targetIntensity, intensityChangeSpeed * Time.deltaTime);
        RenderSettings.ambientIntensity = Mathf.Lerp(RenderSettings.ambientIntensity, targetAmbientIntensity, intensityChangeSpeed * Time.deltaTime);

        if (skybox != null)
        {
            RenderSettings.skybox.SetColor("_Tint", Color.Lerp(dayTimeColor, nightTimeColor, intensityChangeSpeed * Time.deltaTime));
        }

        if (timeOfDay > cycleDuration)
            timeOfDay = 0;

        timeOfDay += (timeOfDay > dayStartTime && timeOfDay < dayEndTime ? cycleSpeed : cycleSpeed * 2) * Time.deltaTime;

        UpdateLighting();
    }

    public void UpdateLighting()
    {
        sun.localRotation = Quaternion.Euler((timeOfDay * 360 / cycleDuration), 0, 0);
        isNightTime = timeOfDay < dayStartTime || timeOfDay > dayEndTime;
    }
}
