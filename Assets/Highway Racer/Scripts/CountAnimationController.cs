﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CountAnimationController : MonoBehaviour
{
    private Text text;
    public string originalText;

    public float originalValue = 0f;
    public float targetValue = 0f;

    public bool actNow = false;
    public bool endedAnimation = false;

    private AudioSource countingAudioSource;

    void Awake()
    {
        text = GetComponent<Text>();
        originalText = text.text;
    }

    public void GetNumber()
    {
        originalValue = float.Parse(text.text, System.Globalization.NumberStyles.Number);
        text.text = "0";
    }

    public void Count()
    {
        if (GameObject.Find(HR_HighwayRacerProperties.Instance.countingPointsAudioClip.name))
            countingAudioSource = GameObject.Find(HR_HighwayRacerProperties.Instance.countingPointsAudioClip.name)
                .GetComponent<AudioSource>();
        else
            countingAudioSource = RCC_CreateAudioSource.NewAudioSource(Camera.main.gameObject,
                HR_HighwayRacerProperties.Instance.countingPointsAudioClip.name, 0f, 0f, 1f,
                HR_HighwayRacerProperties.Instance.countingPointsAudioClip, true, true, true);

        countingAudioSource.ignoreListenerPause = true;
        countingAudioSource.ignoreListenerVolume = true;

        actNow = true;
    }

    void Update()
    {
        if (!actNow || endedAnimation)
            return;

        if (countingAudioSource && !countingAudioSource.isPlaying)
            countingAudioSource.Play();

        targetValue = Mathf.MoveTowards(targetValue, originalValue, Time.unscaledDeltaTime * 350f);

        text.text = targetValue.ToString("F0");

        if ((originalValue - targetValue) <= .05f)
        {
            if (countingAudioSource && countingAudioSource.isPlaying)
                countingAudioSource.Stop();

            text.text = originalValue.ToString("F0");

            if (GetComponentInParent<ButtonsAnimation>())
                GetComponentInParent<ButtonsAnimation>().endedAnimation = true;

            endedAnimation = true;
        }

        if (endedAnimation)
            enabled = false;
    }
}