using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace NovelGame
{
    public class NovelManager : MonoBehaviour
    {
        public static NovelManager Instance { get; private set; }

        [Header("Manager参照")]
        public UserScriptManager userScriptManager;
        public MainTextController mainTextController;
        public ImageManager imageManager;

        [Header("ノベルパートのUIオブジェクト")]
        [SerializeField] private GameObject novelUIRoot; // ノベルUIまとめ

        private bool isNovelActive = true;
        [System.NonSerialized] public int lineNumber;

        private InputAction clickAction;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            lineNumber = 0;

            // ★ ノベルモードに切り替え
            InputManager.Instance?.SwitchActionMap(GameMode.NovelGame);

            // ★ 現在のアクションマップがノベル用か確認
            if (InputManager.Instance != null && InputManager.Instance.CurrentMode == GameMode.NovelGame)
            {
                var map = InputManager.Instance.GetCurrentActionMap();
                if (map != null)
                {
                    clickAction = map.FindAction("Click");
                    if (clickAction != null)
                    {
                        clickAction.performed += OnClickPerformed;
                        clickAction.Enable();
                    }
                    else
                    {
                        Debug.LogWarning("Click アクションが見つかりません");
                    }
                }
            }
        }

        private void Start()
        {
            AudioManager.Instance?.StopBGM();
        }

        private void OnDestroy()
        {
            if (clickAction != null)
            {
                clickAction.performed -= OnClickPerformed;
                clickAction.Disable();
            }
        }

        /// <summary>
        /// ノベルパートの表示を切り替える（ON/OFF）
        /// </summary>
        public void SetNovelActive(bool active)
        {
            isNovelActive = active;

            if (novelUIRoot != null)
            {
                novelUIRoot.SetActive(active);
            }
            else
            {
                Debug.LogWarning("novelUIRoot が設定されていません");
            }

            // ONならノベルモード、OFFならメインモードに切り替え
            if (active)
            {
                InputManager.Instance?.SwitchActionMap(GameMode.NovelGame);
            }
            else
            {
                InputManager.Instance?.SwitchActionMap(GameMode.MainGame);
            }
        }

        public void TextLineIncrement()
        {
            if (lineNumber < UserScriptManager.LineCount)
                lineNumber++;
        }

        private void OnClickPerformed(InputAction.CallbackContext ctx)
        {
            // ★ 現在のアクションマップがノベル用ならだけ反応
            if (InputManager.Instance != null && InputManager.Instance.CurrentMode == GameMode.NovelGame)
            {
                mainTextController?.OnClickByManager();
            }
        }

        public static void SceneChanger(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
