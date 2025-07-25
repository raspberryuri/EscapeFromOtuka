using UnityEngine;
using UnityEngine.UI; // Legacy UI elements (Button, InputField)
using TMPro; // TextMeshProを使うため

public class WordJudge : MonoBehaviour
{
    [Header("判定するUI要素")]
    [SerializeField]
    private TMP_InputField _inputField; // 文字を入力するInputField

    [SerializeField]
    private Button _submitButton; // 判定を実行するボタン

    [SerializeField]
    private TextMeshProUGUI _resultText; // 「成功！」「失敗！」を表示するテキスト

    [Header("判定設定")]
    [SerializeField]
    private string _correctAnswer = "password"; // 正解の文字列（Inspectorから変更可能）

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start()
    {
        // ボタンがクリックされたら、ValidateInputメソッドを呼び出すように設定
        _submitButton.onClick.AddListener(ValidateInput);

        // InputFieldでEnterが押された時の処理を登録
        _inputField.onSubmit.AddListener((text) => ValidateInput());

        // 最初は結果テキストを非表示にしておく
        _resultText.gameObject.SetActive(false);
    }

    /// <summary>
    /// 入力された文字列を判定するメソッド
    /// </summary>
    public void ValidateInput()
    {
        // 結果テキストを表示状態にする
        _resultText.gameObject.SetActive(true);

        // InputFieldのテキストと正解の文字列を比較
        if (_inputField.text == _correctAnswer)
        {
            // 成功した場合
            _resultText.text = "Success!!";
            _resultText.color = Color.green; // 文字色を緑に
            Debug.Log("成功！");
        }
        else
        {
            // 失敗した場合
            _resultText.text = "Faled...";
            _resultText.color = Color.red; // 文字色を赤に
            Debug.Log("失敗！");
        }
        _inputField.gameObject.SetActive(false);
        _submitButton.gameObject.SetActive(false);
    }
}