using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("クリップ管理")]
    [SerializeField] private AudioClipTable audioTable;

    [Header("オーディオミキサー設定")]
    [SerializeField] private AudioMixerGroup bgmMixerGroup;
    [SerializeField] private AudioMixerGroup seMixerGroup;

    private AudioSource BGM_source, SE_source;
    private static AudioManager instance;
    public static AudioManager Instance => instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        CreateAudioSources();
    }

    private void CreateAudioSources()
    {
        // BGM
        GameObject bgmObj = new GameObject("BGM_Box");
        bgmObj.transform.SetParent(transform);
        BGM_source = bgmObj.AddComponent<AudioSource>();
        BGM_source.loop = true;
        BGM_source.playOnAwake = false;
        if (bgmMixerGroup != null)
            BGM_source.outputAudioMixerGroup = bgmMixerGroup;

        // SE
        GameObject seObj = new GameObject("SE_Box");
        seObj.transform.SetParent(transform);
        SE_source = seObj.AddComponent<AudioSource>();
        SE_source.loop = false;
        SE_source.playOnAwake = false;
        if (seMixerGroup != null)
            SE_source.outputAudioMixerGroup = seMixerGroup;
    }

    /// <summary> SEを数値で指定して1回だけ再生する（UI用）</summary>
    public void OneShotSE_UI(int ClipNumber)
    {
        AudioClip clip = audioTable.GetClipByKey(ClipNumber);
        SE_source?.PlayOneShot(clip);
    }

    /// <summary> SEをenumで指定して1回だけ再生する（汎用）</summary>
    public void OneShotSE(enAudioClip ClipName)
    {
        AudioClip clip = audioTable.GetClipByKey((int)ClipName);
        SE_source?.PlayOneShot(clip);
    }

    /// <summary>BGMを再生する（指定したAudioClipに切り替え）</summary>
    public void PlayBGM(enAudioClip clipName)
    {
        AudioClip clip = audioTable.GetClipByKey((int)clipName);
        if (clip != null)
        {
            BGM_source.clip = clip;
            BGM_source.Play();
        }
        else
        {
            Debug.LogWarning($"[AudioManager] BGM '{clipName}' が AudioClipTable に見つかりません");
        }
    }

    /// <summary>BGMを停止する</summary>
    public void StopBGM()
    {
        if (BGM_source.isPlaying)
            BGM_source.Stop();
    }

    /// <summary>再生中のSEを停止する</summary>
    public void StopSE()
    {
        if (SE_source.isPlaying)
            SE_source.Stop();
    }
}
