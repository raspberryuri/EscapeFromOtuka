using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace NovelSystems
{
    [RequireComponent(typeof(NovelTextController))]
    [RequireComponent(typeof(NovelUIManager))]
    public class NovelManager : MonoBehaviour
    {
        public static NovelManager Instance { get; private set; }

        [Header("Manager参照")]
        public NovelTextController mainTextController;
        public NovelUIManager uiManager;

        [Header("ノベルパートのUIオブジェクト")]
        [SerializeField] private GameObject novelUIRoot;

        [Header("シナリオデータ")]
        [SerializeField] private NovelData novelData;

        private NovelDataManager _manager;
        public static NovelDataManager Data => Instance?._manager;

        public static NovelUIManager UI => Instance?.uiManager;
        public static NovelTextController TextController => Instance?.mainTextController;

        private bool isNovelActive = true;
        [System.NonSerialized] public int lineNumber;

        private InputAction clickAction;

        private void Start()
        {
            if (!InitializeSingleton()) return;

            InitializeDataManager();
            InitializeInput();
        }

        #region Initialization

        private bool InitializeSingleton()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                lineNumber = 0;
                return true;
            }
            else
            {
                Destroy(gameObject);
                return false;
            }
        }

        private void InitializeDataManager()
        {
            if (novelData != null)
            {
                _manager = new NovelDataManager(novelData);
            }
            else
            {
                Debug.LogWarning("NovelData が設定されていません。データマネージャーは初期化されません。");
            }
        }

        private void InitializeInput()
        {
            if (InputManager.Instance == null)
            {
                Debug.LogWarning("InputManager がシーンに存在しません。クリック入力は無効です。");
                return;
            }


            InputManager.Instance.SwitchActionMap(GameMode.NovelGame);

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

        #endregion

        private void OnDestroy()
        {
            if (clickAction != null)
            {
                clickAction.performed -= OnClickPerformed;
                clickAction.Disable();
            }
        }

        public void SetNovelActive(bool active)
        {
            isNovelActive = active;

            if (novelUIRoot != null)
                novelUIRoot.SetActive(active);
            else
                Debug.LogWarning("novelUIRoot が設定されていません");

            if (InputManager.Instance != null)
            {
                var targetMode = active ? GameMode.NovelGame : GameMode.MainGame;
                if (InputManager.Instance.SwitchActionMap(targetMode))
                {
                    Debug.LogWarning($"ActionMap {targetMode} に切り替えできませんでした");
                }
            }
        }

        private void OnClickPerformed(InputAction.CallbackContext ctx)
        {
            if (InputManager.Instance != null && InputManager.Instance.CurrentMode == GameMode.NovelGame)
            {
                mainTextController?.OnClick();
            }
        }

        #region Public API

        /// <summary>シーン切り替え</summary>
        public static void SceneChanger(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        /// <summary>シナリオを名前で選択し再生する</summary>
        public void PlayScenario(string scenarioName)
        {
            var scenario = _manager?.GetScenarioTextByName(scenarioName);
            Debug.Log(scenario.name);
            if (scenario == null)
            {
                Debug.LogError($"シナリオ '{scenarioName}' が見つかりません。");
                return;
            }

            SetNovelActive(true);
            lineNumber = 0;
            TextController?.Initialize(scenario, uiManager);
        }

        /// <summary>シナリオをインデックスで選択し再生する</summary>
        public void PlayScenario(int index)
        {
            var scenario = _manager?.GetScenarioTextByIndex(index);
            if (scenario == null)
            {
                Debug.LogError($"シナリオインデックス {index} が無効です。");
                return;
            }

            SetNovelActive(true);
            lineNumber = 0;
            TextController?.Initialize(scenario, uiManager);
        }

        #endregion

        #region Static UI Helper

        public static void PutImage(string identifier, string parentObjectName)
            => UI?.PutImage(identifier, parentObjectName);

        public static void RemoveImage(string identifier)
            => UI?.RemoveImage(identifier);

        public static void RemoveAllImages()
            => UI?.RemoveAllImages();

        public static void OpenLogWindow()
            => UI?.OpenLogWindow();

        public static void CloseLogWindow()
            => UI?.CloseLogWindow();

        public static void ResetTextDisplay()
            => TextController?.ResetAll();

        public static void ClickText()
            => TextController?.OnClick();

        #endregion
    }
}
