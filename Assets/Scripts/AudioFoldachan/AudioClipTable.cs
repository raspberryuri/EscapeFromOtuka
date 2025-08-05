using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// AudioClipと対応するキーのエントリ
/// </summary>
[System.Serializable]
public class AudioClipEntry
{
    public enAudioClip key;       // SEの識別キー
    public AudioClip sound;    // 対応するAudioClip
}

/// <summary>
/// SE（効果音）用のキー定義列挙体
/// </summary>
public enum enAudioClip// 
{
    BGM_default = 100,
    UI_Slider = 2,
    UI_StartSE = 3,
    UI_Talk_miyabi = 4,
    UI_Talk_shujinkou = 5,
    UI_Talk_nazo = 6,
    UI_Talk_narration = 7,
    UI_NextText = 8,
    UI_OpenInput = 9,
    UI_InputHit = 10,          
    UI_InputMiss = 11,         

    Text_OpneDoor = 51,
    Text_StartAI = 52,
    Text_ErrorAI = 53,
    Text_ChangeTheWorld = 54,

    TextBGM_Gakko = 101,
    TextBGM_Nature = 102,
    GameBGM_MainBGM = 103,
}

/// <summary>
/// SE用のAudioClipテーブル
/// </summary>
[CreateAssetMenu(menuName = "GameSettings/AudioClipTable", fileName = "AudioClipTable")]
public class AudioClipTable : ScriptableObject
{
    // AudioClipEntryのリスト（初期化済み）
    public List<AudioClipEntry> audioClipEntries = new List<AudioClipEntry>();

    /// <summary>
    /// 整数キーからAudioClipを取得する
    /// </summary>
    /// <param name="key">enSEClipに対応する整数キー</param>
    /// <returns>一致するAudioClip、存在しない場合はnull</returns>
    public AudioClip GetClipByKey(int key)
    {
        foreach (var entry in audioClipEntries)
        {
            if ((int)entry.key == key)
            {
                return entry.sound;
            }
        }

        Debug.LogError($"AudioClip with key '{key}' not found.");
        return null;
    }
}
