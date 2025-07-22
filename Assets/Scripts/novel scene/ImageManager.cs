using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NovelGame
{
    public class ImageManager : MonoBehaviour
    {
        [SerializeField] Sprite _background1;
        [SerializeField] Sprite _eventCG1;
        [SerializeField] Sprite _eventCG2;
        [SerializeField] GameObject _backgroundObject;
        [SerializeField] GameObject _eventObject;
        [SerializeField] GameObject _imagePrefab;

        // テキストファイルから、文字列でSpriteやGameObjectを扱えるようにするための辞書
        Dictionary<string, Sprite> _textToSprite;
        Dictionary<string, GameObject> _textToParentObject;

        // 操作したいPrefabを指定できるようにするための辞書
        Dictionary<string, GameObject> _textToSpriteObject;

        void Awake()
        {
            _textToSprite = new Dictionary<string, Sprite>();
            _textToSprite.Add("background1", _background1);
            _textToSprite.Add("eventCG1", _eventCG1);
            _textToSprite.Add("eventCG2", _eventCG2);

            _textToParentObject = new Dictionary<string, GameObject>();
            _textToParentObject.Add("backgroundObject", _backgroundObject);
            _textToParentObject.Add("eventObject", _eventObject);

            _textToSpriteObject = new Dictionary<string, GameObject>();
        }

        // 画像を配置する
        public void PutImage(string imageName, string parentObjectName)
        {
            if (!_textToSprite.TryGetValue(imageName, out var image) ||
                !_textToParentObject.TryGetValue(parentObjectName, out var parentObject))
            {
                Debug.LogWarning("Invalid image or parent name.");
                return;
            }

            GameObject item = Instantiate(_imagePrefab, Vector2.zero, Quaternion.identity, parentObject.transform);
            item.name = imageName;
            Image img = item.GetComponent<Image>();
            img.sprite = image;

            // RectTransform をリセットしてストレッチ有効にする
            RectTransform rect = item.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0, 1);
            rect.position = new Vector3(0, 0, 0);
            rect.offsetMin = Vector2.zero; // 左下
            rect.offsetMax = Vector2.zero; // 右上
            rect.localScale = Vector3.one;

            _textToSpriteObject[imageName] = item;
        }

        // 画像を削除する
        public void RemoveImage(string imageName)
        {
            Destroy(_textToSpriteObject[imageName]);
        }
    }
}
