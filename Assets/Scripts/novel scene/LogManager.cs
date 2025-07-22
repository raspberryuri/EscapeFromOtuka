using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LogManager : MonoBehaviour
{
    [Header("UI 参照")]
    [SerializeField] private GameObject logPanel;       // ログ全体を包むパネル
    [SerializeField] private GameObject logItemPrefab;  // １行分のログ表示用プレハブ (Text or TMP)
    [SerializeField] private Transform content;         // Scroll View > Viewport > Content
    [SerializeField] private ScrollRect scrollRect;     // ScrollRect コンポーネント
    [SerializeField] private GameObject OpenbackLog;
    [SerializeField] private GameObject ClosebackLog;

    // 会話履歴をためるリスト
    private List<string> logHistory = new List<string>();

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



        // 表示前に内容クリア
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // ログを1つずつTextとして生成
        foreach (string msg in logHistory)
        {
            GameObject item = Instantiate(logItemPrefab, content);
            item.GetComponent<Text>().text = msg;
        }

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
    }
}

