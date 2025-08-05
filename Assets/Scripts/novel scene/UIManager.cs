using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI 参照")]
    [SerializeField] private GameObject logPanel;       // ログ全体を包むパネル
    [SerializeField] private TextMeshProUGUI context;  
    [SerializeField] private NovelGame.NovelManager NovelManager;
    [SerializeField] private ScrollRect scrollRect;     // ScrollRect コンポーネント
    [SerializeField] private GameObject OpenbackLog;
    [SerializeField] private GameObject ClosebackLog;

    // 会話履歴をためるリスト
    private List<string> logHistory = new List<string>();


    private void Start()
    {
        NovelManager = GetComponent<NovelGame.NovelManager>();
    }
    /// <summary>
    /// 会話が発生した時に呼び出す。
    /// 画面上のログにも即時追加し、履歴にも残す。
    /// </summary>
    public void AddLog(string message)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            logHistory.Add(message);
        }

    }
    /// <summary>
    /// ログパネルを開くボタンに割り当てる。
    /// 履歴をいったんクリアしてから、全部再描画する。
    /// </summary>
    public void OpenLogWindow()
    {
        // 1) パネル表示
        logPanel.SetActive(true);
        
        context.text = NovelGame.MainTextController.TextLog;
        InputManager.Instance.SwitchActionMap(GameMode.NotInput);

        // スクロール最下部に
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    /// <summary>
    /// ログパネルを閉じるボタンに割り当てる。
    /// </summary>
    public void CloseLogWindow()
    {
        logPanel.SetActive(false);
        InputManager.Instance.SwitchActionMap(GameMode.NovelGame);
    }
}
