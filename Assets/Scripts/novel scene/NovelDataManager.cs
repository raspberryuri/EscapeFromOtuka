using System.Collections.Generic;
using UnityEngine;

namespace NovelSystems
{
    public class NovelDataManager
    {
        private NovelData _novelData;

        public NovelDataManager(NovelData data)
        {
            _novelData = data;
        }

        // ----------------------------
        // シナリオテキスト取得
        // ----------------------------

        /// <summary>
        /// 名前でシナリオテキストを取得（存在しなければ null）
        /// </summary>
        public TextAsset GetScenarioTextByName(string name)
        {
            foreach (var entry in _novelData.scenarioTexts)
            {
                if (entry.name == name)
                    return entry.textAsset;
            }
            return null;
        }

        /// <summary>
        /// インデックスでシナリオテキストを取得（範囲外なら null）
        /// </summary>
        public TextAsset GetScenarioTextByIndex(int index)
        {
            if (_novelData == null || _novelData.scenarioTexts == null || index < 0 || index >= _novelData.scenarioTexts.Count)
                return null;

            return _novelData.scenarioTexts[index].textAsset;
        }

        // ----------------------------
        // 背景画像取得
        // ----------------------------

        /// <summary>
        /// 名前で背景画像を取得（存在しなければ null）
        /// </summary>
        public Sprite GetBackgroundSpriteByName(string name)
        {
            foreach (var entry in _novelData.backgroundImages)
            {
                if (entry.name == name)
                    return entry.sprite;
            }
            return null;
        }

        /// <summary>
        /// インデックスで背景画像を取得（範囲外なら null）
        /// </summary>
        public Sprite GetBackgroundSpriteByIndex(int index)
        {
            if (_novelData == null || _novelData.backgroundImages == null || index < 0 || index >= _novelData.backgroundImages.Count)
                return null;

            return _novelData.backgroundImages[index].sprite;
        }

        /// <summary>
        /// キャラクター名でCharacterDataを取得（存在しなければ null）
        /// </summary>
        public CharacterData GetCharacterData(string characterName)
        {
            if (_novelData == null || _novelData.characters == null)
                return null;

            foreach (var character in _novelData.characters)
            {
                if (character.characterName == characterName)
                    return character;
            }
            return null;
        }

        /// <summary>
        /// インデックスでキャラクターを取得（範囲外なら null）
        /// </summary>
        public CharacterData GetCharacterDataByIndex(int index)
        {
            if (_novelData == null || _novelData.characters == null || index < 0 || index >= _novelData.characters.Length)
                return null;

            return _novelData.characters[index];
        }
    }
}
