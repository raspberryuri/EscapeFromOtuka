using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NovelGame
{
    public class ImageManager : MonoBehaviour
    {
        [Header("背景画像")]
        [SerializeField] Sprite[] _backgrounds;

        [Header("キャラ画像（表情順：デフォルト、笑う、驚く）")]
        [SerializeField] Sprite[] _Miyabi;
        [SerializeField] Sprite[] _Sontyo;
        [SerializeField] Sprite[] _Unknown;

        [Header("背景オブジェクト")]
        [SerializeField] GameObject _backgroundObject;

        [Header("キャラオブジェクト")]
        [SerializeField] GameObject _eventObject;

        [Header("イメージプレハブ")]
        [SerializeField] GameObject _imagePrefab;

        Dictionary<string, Sprite> _textToSprite;
        Dictionary<string, GameObject> _textToParentObject;
        Dictionary<string, GameObject> _textToSpriteObject;

        void Awake()
        {
            _textToSprite = new Dictionary<string, Sprite>();
            _textToParentObject = new Dictionary<string, GameObject>();
            _textToSpriteObject = new Dictionary<string, GameObject>();

            // 背景画像の登録（例：background0, background1...）
            for (int i = 0; i < _backgrounds.Length; i++)
            {
                _textToSprite.Add($"background{i + 1}", _backgrounds[i]);
            }

            // キャラ画像の登録（命名規則：miyabi_default, sontyo_smile, unknown_surprise）
            AddCharacterExpressions("miyabi", _Miyabi);
            AddCharacterExpressions("sontyo", _Sontyo);
            AddCharacterExpressions("unknown", _Unknown);

            // 親オブジェクトの登録
            _textToParentObject.Add("background", _backgroundObject);
            _textToParentObject.Add("event", _eventObject);
        }

        // キャラクターの表情を辞書に登録
        void AddCharacterExpressions(string name, Sprite[] expressions)
        {
            if (expressions.Length >= 3)
            {
                _textToSprite[$"{name}_default"] = expressions[0];
                _textToSprite[$"{name}_smile"] = expressions[1];
                _textToSprite[$"{name}_surprise"] = expressions[2];
            }
        }

        // 画像を配置する
        public void PutImage(string imageName, string parentObjectName)
        {
            if (!_textToSprite.TryGetValue(imageName, out var image) ||
                !_textToParentObject.TryGetValue(parentObjectName, out var parentObject))
            {
                Debug.LogWarning($"Invalid image name ({imageName}) or parent name ({parentObjectName}).");
                return;
            }

            GameObject item = Instantiate(_imagePrefab, Vector2.zero, Quaternion.identity, parentObject.transform);
            item.name = imageName;

            Image img = item.GetComponent<Image>();
            img.sprite = image;

            //RectTransform の設定
            RectTransform rect = item.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.localScale = Vector3.one;

            _textToSpriteObject[imageName] = item;
        }

        // 画像を削除する
        public void RemoveImage(string imageName)
        {
            if (_textToSpriteObject.TryGetValue(imageName, out var obj))
            {
                Destroy(obj);
                _textToSpriteObject.Remove(imageName);
            }
        }

        // すべての画像を削除する
        public void RemoveAllImages()
        {
            ClearChildren(_backgroundObject.transform);
            ClearChildren(_eventObject.transform);

            _textToSpriteObject.Clear(); // 辞書もクリア
        }

        // 指定したTransformの全子オブジェクトを削除
        private void ClearChildren(Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }




    }
}
