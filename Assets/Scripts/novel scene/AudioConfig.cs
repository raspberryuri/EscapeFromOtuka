using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioConfig : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioSource seAudioSource;
    [SerializeField] AudioSource bgmAudioSource;
    [SerializeField] Slider seSlider;
    [SerializeField] Slider bgmSlider;

    private void Start()
    {
        audioMixer.SetFloat("BGM", -80f);
        bgmSlider.onValueChanged.AddListener((value) =>
        {
            value = Mathf.Clamp01(value);

            float decibel = 20f * Mathf.Log10(value);
            decibel = Mathf.Clamp(decibel, -80f,0f);
            audioMixer.SetFloat("BGM", decibel);
        });

        seSlider.onValueChanged.AddListener((value) =>
        {
            value = Mathf.Clamp01(value);

            float decibel = 20f * Mathf.Log10(value);
            decibel = Mathf.Clamp(decibel, -80f,0f);
            audioMixer.SetFloat("SE", decibel);
        });
    }



    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            seAudioSource.Play();
        }
    }
}
