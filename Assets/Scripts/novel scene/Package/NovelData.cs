using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NovelSystems
{
    [CreateAssetMenu(fileName = "NovelData", menuName = "Novel/NovelData")]
    public class NovelData : ScriptableObject
    {
        [Header("シナリオテキスト（名前付き複数登録）")]
        public List<NamedTextAsset> scenarioTexts = new List<NamedTextAsset>();

        [Header("背景画像（名前付き複数登録）")]
        public List<NamedSprite> backgroundImages = new List<NamedSprite>();

        [Header("キャラクターデータ（複数登録可）")]
        public CharacterData[] characters;
    }

    [System.Serializable]
    public class NamedTextAsset
    {
        public string name;
        public TextAsset textAsset;
    }

    [System.Serializable]
    public class NamedSprite
    {
        public string name;
        public Sprite sprite;
    }

    [System.Serializable]
    public class CharacterData
    {
        public string characterName;

        [Header("表情画像")]
        public Sprite def;
        public Sprite smi;
        public Sprite sup;
        public Sprite sad;
        public Sprite ang;
    }
}
