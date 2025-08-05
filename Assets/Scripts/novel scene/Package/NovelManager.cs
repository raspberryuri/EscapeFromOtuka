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

        // データマネージャ
        private NovelDataManager _manager;
        public static NovelDataManager Data => Instance?._manager;

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

            // データマネージャを初期化
            if (novelData != null)
            {
                _manager = new NovelDataManager(novelData);
            }
            else
            {
                Debug.LogWarning("NovelData が設定されていません。データマネージャは初期化されません。");
            }

            InputManager.Instance?.SwitchActionMap(GameMode.NovelGame);

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

            if (active)
            {
                InputManager.Instance?.SwitchActionMap(GameMode.NovelGame);
            }
            else
            {
                InputManager.Instance?.SwitchActionMap(GameMode.MainGame);
            }
        }

        private void OnClickPerformed(InputAction.CallbackContext ctx)
        {
            if (InputManager.Instance != null && InputManager.Instance.CurrentMode == GameMode.NovelGame)
            {
                mainTextController?.OnClick();
            }
        }

        public static void SceneChanger(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
