using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
        [SerializeField] public GameMode returnActionMap = GameMode.NotInput;
        public static NovelDataManager Data => Instance?._manager;

        public static NovelUIManager UI => Instance?.uiManager;
        public static NovelTextController TextController => Instance?.mainTextController;

        private bool isNovelActive = true;
        [System.NonSerialized] public int lineNumber;

        private InputAction clickAction;

        private void Awake()
        {
            if (!InitializeSingleton()) return;

            InitializeDataManager();
            SetNovelActive(false);

            // ★ HomeButton を子階層から探してイベント登録
            Button[] buttons = GetComponentsInChildren<Button>(true);
            foreach (var btn in buttons)
            {
                if (btn.name == "HomeButton")
                {
                    btn.onClick.AddListener(OnHomeButtonClicked);
                    Debug.Log("HomeButton にクリックイベントを登録しました");
                    break;
                }
            }
        }

        public void OnHomeButtonClicked()
        {
            NovelAudioManager.Instance.StopBGM();
            uiManager.SetCursorState(true,false);
            Destroy(this.gameObject);
            SceneManager.LoadScene(0);
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
            uiManager.SetCursorState(active,!active);


            if (novelUIRoot != null)
                novelUIRoot.SetActive(active);
            else
                Debug.LogWarning("novelUIRoot が設定されていません");

            if (InputManager.Instance != null)
            {
                var targetMode = active ? GameMode.NovelGame : returnActionMap;
                if (InputManager.Instance.SwitchActionMap(targetMode))
                {
                    Debug.LogWarning($"ActionMap {targetMode} に切り替えできませんでした");
                }

                if (active)
                {
                    if (clickAction == null)
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
                        }
                    }
                    else
                    {
                        clickAction.Enable();
                    }
                }
                else
                {
                    clickAction?.Disable();
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

        public static void SceneChanger(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
        public void SetHomeAndLogButtonsActive(bool active)
        {
            Button[] buttons = GetComponentsInChildren<Button>(true);

            foreach (var btn in buttons)
            {
                if (btn.name == "HomeButton" || btn.name == "LogButton")
                {
                    btn.gameObject.SetActive(active);
                    Debug.Log($"{btn.name} の表示を {(active ? "ON" : "OFF")} にしました");
                }
            }
        }

        public void PlayScenario(string scenarioName)
        {
            var scenario = _manager?.GetScenarioTextByName(scenarioName);
            Debug.Log(scenario.name);
            if (scenario == null)
            {
                Debug.LogError($"シナリオ '{scenarioName}' が見つかりません。");
                return;
            }

            Debug.Log($"Scenario->{returnActionMap.ToString()}");
            SetNovelActive(true);

            lineNumber = 0;
            TextController?.Initialize(scenario, uiManager, uiManager);
        }

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
            TextController?.Initialize(scenario, uiManager, uiManager);
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
        {
            UI?.OpenLogWindow();
            Time.timeScale = 0.0f;
            InputManager.Instance.SwitchActionMap(GameMode.NotInput);
        }

        public static void CloseLogWindow()
        {
            UI?.CloseLogWindow();
            Time.timeScale = 1.0f;
            InputManager.Instance.SwitchActionMap(GameMode.NovelGame);
        }

        public static void ResetTextDisplay()
            => TextController?.ResetAll();

        public static void ClickText()
            => TextController?.OnClick();

        #endregion
    }
}
