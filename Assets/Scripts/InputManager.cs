using UnityEngine;
using UnityEngine.InputSystem;

public enum GameMode
{
    MainGame,
    InputField,
    NovelGame,
    NotInput
}

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [Header("Input System アセット")]
    [SerializeField] private InputActionAsset inputActions;

    private InputActionMap currentMap;
    public GameMode CurrentMode { get; private set; } = GameMode.MainGame;

    // === デバッグ用 Inspector 表示 ===
    [SerializeField, Tooltip("現在有効なアクションマップ名（デバッグ用）")]
    private string currentMapName;
    [SerializeField, Tooltip("現在有効なアクション数（デバッグ用）")]
    private int currentActionCount;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        // Inspector 更新用
        if (currentMap != null)
        {
            currentMapName = currentMap.name;
            currentActionCount = currentMap.actions.Count;
        }
        else
        {
            currentMapName = "(なし)";
            currentActionCount = 0;
        }
    }

    /// <summary>
    /// モード切り替え＋アクションマップ更新
    /// </summary>
    public void SwitchActionMap(GameMode mode)
    {
        CurrentMode = mode;
        string mapName = mode switch
        {
            GameMode.MainGame => "MainGame",
            GameMode.InputField => "InputField",
            GameMode.NovelGame => "NovelGame",
            GameMode.NotInput => null,  // すべて無効
            _ => null
        };

        // 既存マップを無効化
        if (currentMap != null && currentMap.enabled)
            currentMap.Disable();

        if (string.IsNullOrEmpty(mapName))
        {
            currentMap = null;
            Debug.Log($"[InputManager] モード {mode} ですべての入力無効");
            return;
        }

        // 新マップを有効化
        currentMap = inputActions.FindActionMap(mapName, true);
        if (currentMap != null)
        {
            currentMap.Enable();
            Debug.Log($"[InputManager] モード {mode} に切替（マップ:{mapName}）");
        }
        else
        {
            Debug.LogWarning($"[InputManager] アクションマップ '{mapName}' が見つかりません");
        }
    }

    public InputActionMap GetCurrentActionMap()
    {
        return currentMap;
    }
}
