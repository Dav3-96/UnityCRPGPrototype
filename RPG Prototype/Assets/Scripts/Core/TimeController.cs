using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    #region All Settings (Not broken down)
    [Tooltip("How much faster than real time the cycle will be, i.e 100 x 1 second will make speed 100x faster than real time")]
    [SerializeField] float timeMultiplier;

    [Tooltip("The time that the cycle will start at")]
    [SerializeField] float startHour;

    [Tooltip("The Directional Light object to use")]
    [SerializeField] Light sun;

    [Tooltip("Time that the sun will rise")]
    [Range(0f, 24f)]
    [SerializeField] float sunriseHour;

    [Tooltip("The time that the sun will set")]
    [Range(0f, 24f)]
    [SerializeField] float sunsetHour;

    private DateTime currentTime;
    private TimeSpan sunriseTime;
    private TimeSpan sunsetTime;

    [Tooltip("The colour of the ambient light during the day")]
    [SerializeField] private Color dayAmbientLight;

    [Tooltip("The colour of the ambient light during the night")]
    [SerializeField] private Color nightAmbientLight;

    [Tooltip("How quickly the sun will rise/set")]
    [SerializeField] private AnimationCurve lightChangeCurve;

    [Tooltip("The brightness of the sun light")]
    [SerializeField] private float maxSunLightIntensity;

    [Tooltip("The Directional Light object to use")]
    [SerializeField] private Light moonLight;

    [Tooltip("The brightness of the moon light")]
    [SerializeField] private float maxMoonLightIntensity;
    #endregion

    private void Start()
    {
        currentTime = DateTime.Now + TimeSpan.FromHours(startHour);

        sunriseTime = TimeSpan.FromHours(sunriseHour);
        sunsetTime = TimeSpan.FromHours(sunsetHour);
    }

    private void Update()
    {
        UpdateTimeOfDay();
        RotateSun();
        UpdateLightSettings();

    }

    void UpdateLightSettings()
    {
        float dotProduct = Vector3.Dot(sun.transform.forward, Vector3.down);
        sun.intensity = Mathf.Lerp(0, maxSunLightIntensity, lightChangeCurve.Evaluate(dotProduct));
        moonLight.intensity = Mathf.Lerp(maxMoonLightIntensity, 0, lightChangeCurve.Evaluate(dotProduct));
        RenderSettings.ambientLight = Color.Lerp(nightAmbientLight, dayAmbientLight, lightChangeCurve.Evaluate(dotProduct));
    }



    void UpdateTimeOfDay()
    {
        currentTime = currentTime.AddSeconds(Time.deltaTime * timeMultiplier);
    }

    void RotateSun()
    {
        float sunLightRotation;

        if (currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime)
        {
            TimeSpan sunriseToSunsetDuration = CalculateTimeDifference(sunriseTime, sunsetTime);
            TimeSpan timeSinceSunrise = CalculateTimeDifference(sunriseTime, currentTime.TimeOfDay);

            double percentage = timeSinceSunrise.TotalMinutes / sunriseToSunsetDuration.TotalMinutes;

            sunLightRotation = Mathf.Lerp(0, 180, (float)percentage);
        }
        else
        {
            TimeSpan sunsetToSunriseDuration = CalculateTimeDifference(sunsetTime, sunriseTime);
            TimeSpan timeSinceSunset = CalculateTimeDifference(sunsetTime, currentTime.TimeOfDay);

            double percentage = timeSinceSunset.TotalMinutes / sunsetToSunriseDuration.TotalMinutes;

            sunLightRotation = Mathf.Lerp(180, 360, (float)percentage);
        }

        sun.transform.rotation = Quaternion.AngleAxis(sunLightRotation, Vector3.right);
    }

    private TimeSpan CalculateTimeDifference(TimeSpan fromTime, TimeSpan toTime)
    {
        TimeSpan diff = toTime - fromTime;

        if (diff.TotalSeconds < 0) 
        {
            diff += TimeSpan.FromHours(24);
        }
        return diff;
    }
}
