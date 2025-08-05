using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioConfig : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider seSlider;
    [SerializeField] private Slider bgmSlider;

    // dB の範囲
    private const float MinDb = -40f;
    private const float MaxDb = 0f;

    private void Start()
    {
        // スライダーの範囲を 0～5 に設定（Inspector でも可）
        seSlider.minValue = 0f;
        seSlider.maxValue = 5f;
        bgmSlider.minValue = 0f;
        bgmSlider.maxValue = 5f;

        // 現在の AudioMixer 値を取得してスライダー初期値に反映
        if (audioMixer.GetFloat("BGM", out float bgmDb))
        {
            bgmSlider.value = DbToSliderValue(bgmDb);
        }
        if (audioMixer.GetFloat("SE", out float seDb))
        {
            seSlider.value = DbToSliderValue(seDb);
        }

        // BGM スライダー変更時
        bgmSlider.onValueChanged.AddListener((value) =>
        {
            float db = SliderValueToDb(value);
            audioMixer.SetFloat("BGM", db);
        });

        // SE スライダー変更時
        seSlider.onValueChanged.AddListener((value) =>
        {
            float db = SliderValueToDb(value);
            audioMixer.SetFloat("SE", db);
        });
    }

    private float SliderValueToDb(float sliderValue)
    {
        // sliderValue: 0~5 → dB: -80~20
        float t = Mathf.InverseLerp(0f, 5f, sliderValue);
        return Mathf.Lerp(MinDb, MaxDb, t);
    }

    private float DbToSliderValue(float db)
    {
        // dB: -80~20 → sliderValue: 0~5
        float t = Mathf.InverseLerp(MinDb, MaxDb, db);
        return Mathf.Lerp(0f, 5f, t);
    }

    private void Update()
    {
    }
}
