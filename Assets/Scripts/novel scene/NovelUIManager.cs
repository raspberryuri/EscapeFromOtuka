using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace NovelSystems
{
    public class NovelUIManager : MonoBehaviour, ICommandExecutor
    {
        [Header("UI 参照")]
        [SerializeField] private GameObject logPanel;
        [SerializeField] private RubyTextMeshProUGUI context;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private GameObject OpenbackLog;
        [SerializeField] private GameObject ClosebackLog;

        [Header("画像管理")]
        [SerializeField] private GameObject backgroundObject;
        [SerializeField] private GameObject eventObject;
        [SerializeField] private GameObject imagePrefab;

        // 会話履歴
        private List<string> logHistory = new List<string>();

        // 画像管理用辞書
        private Dictionary<string, GameObject> textToParentObject;
        private Dictionary<string, GameObject> textToSpriteObject;

        private void Awake()
        {
            // 画像管理用辞書初期化
            textToParentObject = new Dictionary<string, GameObject>
            {
                { "background", backgroundObject },
                { "event", eventObject }
            };
            textToSpriteObject = new Dictionary<string, GameObject>();
        }

        private void Start()
        {
            // 必要に応じて初期化処理
        }

        // ログ追加
        public void AddLog(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                logHistory.Add(message);
            }
        }

        public void OpenLogWindow()
        {
            logPanel.SetActive(true);
            context.text = string.Join("\n", logHistory); // 自前ログを表示
            InputManager.Instance?.SwitchActionMap(GameMode.NotInput);
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }

        public void CloseLogWindow()
        {
            logPanel.SetActive(false);
            InputManager.Instance?.SwitchActionMap(GameMode.NovelGame);
        }

        public void SetCursorState(bool visible, bool lockCursor)
        {
            Cursor.visible = visible;
            Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        }

        // 画像を配置するメソッド
        public void PutImage(string identifier, string parentObjectName)
        {
            Sprite sprite = ResolveImageSprite(identifier);
            if (sprite == null || !textToParentObject.TryGetValue(parentObjectName, out var parentObject))
            {
                Debug.LogWarning($"Invalid image identifier ({identifier}) or parent name ({parentObjectName}).");
                return;
            }

            // 既に同じ名前の画像があれば削除してから追加
            if (textToSpriteObject.TryGetValue(identifier, out var existing))
            {
                Destroy(existing);
                textToSpriteObject.Remove(identifier);
            }

            GameObject item = Instantiate(imagePrefab, Vector2.zero, Quaternion.identity, parentObject.transform);
            item.name = identifier;

            Image img = item.GetComponent<Image>();
            img.sprite = sprite;

            RectTransform rect = item.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.localScale = Vector3.one;

            textToSpriteObject[identifier] = item;
        }

        private Sprite ResolveImageSprite(string identifier)
        {
            var dataManager = NovelManager.Data;
            if (dataManager == null) return null;

            string[] parts = identifier.Split('_');
            if (parts.Length == 2)
            {
                if (parts[0].ToLower() == "background")
                {
                    return dataManager.GetBackgroundSpriteByName(parts[1]);
                }
                else
                {
                    CharacterData charData = dataManager.GetCharacterData(parts[0]);
                    if (charData == null) return null;
                    switch (parts[1].ToLower())
                    {
                        case "default":
                        case "def":
                            return charData.def;
                        case "smile":
                        case "smi":
                            return charData.smi;
                        case "surprise":
                        case "sup":
                            return charData.sup;
                        case "sad":
                            return charData.sad;
                        case "angry":
                        case "ang":
                            return charData.ang;
                        default:
                            return null;
                    }
                }
            }
            return null;
        }

        // 画像削除
        public void RemoveImage(string identifier)
        {
            if (textToSpriteObject.TryGetValue(identifier, out var obj))
            {
                Destroy(obj);
                textToSpriteObject.Remove(identifier);
            }
        }

        // 画像全部削除
        public void RemoveAllImages()
        {
            ClearChildren(backgroundObject.transform);
            ClearChildren(eventObject.transform);
            textToSpriteObject.Clear();
        }

        private void ClearChildren(Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }

        // ICommandExecutorの実装部分

        void ICommandExecutor.PutImage(string name, string spriteName)
        {
            PutImage(name, spriteName);
        }

        void ICommandExecutor.RemoveImage(string name)
        {
            RemoveImage(name);
        }

        void ICommandExecutor.RemoveAllImages()
        {
            RemoveAllImages();
        }
    }
}
