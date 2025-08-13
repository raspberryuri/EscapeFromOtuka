using UnityEngine;
using System.Collections.Generic;



namespace NovelSystems
{
    /// <summary>
    /// AudioClipと対応するキーのエントリ
    /// </summary>
    [System.Serializable]
    public class NovelAudioClipEntry
    {
        public enAudioClip key;       // SEの識別キー
        public AudioClip sound;    // 対応するAudioClip
    }

    /// <summary>
    /// SE（効果音）用のキー定義列挙体
    /// </summary>
    public enum enAudioClip// 
    {
        UI_Talk_miyabi = 1,
        UI_Talk_shujinkou = 2,
        UI_Talk_nazo = 3,
        UI_Talk_narration = 4,
        UI_Talk_Next = 5,

        Text_OpneDoor = 51,
        Text_StartAI = 52,
        Text_ErrorAI = 53,
        Text_ChangeTheWorld = 54,

        BGM_default = 100,
        TextBGM_Gakko = 101,
        TextBGM_Nature = 102,
        GameBGM_MainBGM = 103,
    }

    /// <summary>
    /// SE用のAudioClipテーブル
    /// </summary>
    [CreateAssetMenu(menuName = "Novel/NovelAudioClipTable", fileName = "NovelAudioClipTable")]
    public class NovelAudioClipTable : ScriptableObject
    {
        // AudioClipEntryのリスト（初期化済み）
        public List<NovelAudioClipEntry> audioClipEntries = new List<NovelAudioClipEntry>();

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
}
