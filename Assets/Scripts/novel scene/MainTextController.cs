using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace NovelGame
{
    [System.Serializable]
    public class CharacterColor
    {
        public string characterName;
        public Color color;
    }

    public class MainTextController : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI _mainTextObject;
        [SerializeField] private TextMeshProUGUI _nameObject;

        [Header("文字送り")]
        [SerializeField] private float _feedTime = 0.05f;

        [Header("待機時間（秒）")]
        [SerializeField] private float _defaultWaitTime = 0f;
        public static float WaitDuration = 0f;

        [Header("キャラクターごとの色設定")]
        [SerializeField] private List<CharacterColor> characterColors = new List<CharacterColor>();

        private int _displayedSentenceLength;
        private float _time;
        private bool _isTyping = false;
        private bool _isWaiting = false; // ★wait中フラグ

        public static string TextLog = "";
        private string Text = "";
        private string[] parts;

        // 現在の文字送りSE
        private enAudioClip currentTypingSE = enAudioClip.UI_Talk_narration;

        private void Start()
        {
            StartTextNovel();
        }

        public void StartTextNovel()
        {
            _time = 0f;
            Debug.Log("Start novel");
            StartCoroutine(HandleLine());
        }

        private void Update()
        {
            if (_isTyping && !_isWaiting)
            {
                _time += Time.deltaTime;
                if (_time >= _feedTime)
                {
                    _time -= _feedTime;
                    if (_displayedSentenceLength < _mainTextObject.text.Length)
                    {
                        _displayedSentenceLength++;
                        _mainTextObject.maxVisibleCharacters = _displayedSentenceLength;

                        // ★毎文字で音を鳴らす
                        AudioManager.Instance?.OneShotSE(currentTypingSE);
                    }
                    else
                    {
                        _isTyping = false;
                    }
                }
            }
        }

        public void OnClickByManager()
        {
            // wait中は無視
            if (_isWaiting) return;

            if (_isTyping)
            {
                _displayedSentenceLength = _mainTextObject.text.Length;
                _mainTextObject.maxVisibleCharacters = _displayedSentenceLength;
                _isTyping = false;
                return;
            }

            if (CanGoToTheNextLine())
            {
                AudioManager.Instance?.OneShotSE(enAudioClip.UI_NextText);
                GoToTheNextLine();
            }
        }

        public bool CanGoToTheNextLine()
        {
            return (_displayedSentenceLength >= _mainTextObject.text.Length);
        }

        public void GoToTheNextLine()
        {
            // 最終行チェック
            if (NovelManager.Instance.userScriptManager.IsLastLine())
            {
                OnScenarioEnd();
                return;
            }

            _time = 0f;
            _mainTextObject.maxVisibleCharacters = 0;
            _displayedSentenceLength = 0;
            NovelManager.Instance.lineNumber++;
            StartCoroutine(HandleLine());
        }

        /// <summary>
        /// シナリオが終了した時に呼ばれる関数
        /// </summary>
        private void OnScenarioEnd()
        {
            Debug.Log("シナリオが終了しました");

            // NovelManagerのノベル表示OFF
            if (NovelManager.Instance != null)
            {
                NovelManager.Instance.SetNovelActive(false);
            }
        }

        public void ResetAll()
        {
            // 表示中テキストをクリア
            _mainTextObject.text = "";
            _mainTextObject.maxVisibleCharacters = 0;
            _nameObject.text = "";

            // 状態フラグを初期化
            _displayedSentenceLength = 0;
            _time = 0f;
            _isTyping = false;
            _isWaiting = false;

            // グローバルログやウェイトも初期化
            TextLog = "";
            WaitDuration = 0f;

            // 再生中の文字送りSE設定も初期化
            currentTypingSE = enAudioClip.UI_Talk_narration;

            // もし文字送り中のコルーチンがある場合は止めておく
            StopAllCoroutines();
        }


        private IEnumerator HandleLine()
        {
            string sentence = NovelManager.Instance.userScriptManager.GetCurrentSentence();

            if (NovelManager.Instance.userScriptManager.IsStatement(sentence))
            {
                _isTyping = false;
                _mainTextObject.text = ""; // 命令は表示しない

                NovelManager.Instance.userScriptManager.ExecuteStatement(sentence);

                // &waitコマンド対応
                float waitTime = WaitDuration > 0f ? WaitDuration : _defaultWaitTime;
                if (waitTime > 0f)
                {
                    _isWaiting = true;
                    float timer = 0f;
                    while (timer < waitTime)
                    {
                        timer += Time.deltaTime;
                        yield return null;
                    }
                    _isWaiting = false;
                    WaitDuration = 0f;
                }

                GoToTheNextLine();
            }
            else
            {
                // 現在の文章を処理
                Text = sentence;
                parts = Text.Split(new char[] { ':' }, 2);

                if (parts.Length == 2)
                {
                    string name = parts[0].Trim();
                    string message = parts[1].Trim();

                    // キャラクターの色を取得
                    Color color = Color.white;
                    foreach (var cc in characterColors)
                    {
                        if (cc.characterName == name)
                        {
                            color = cc.color;
                            if (color.a == 0f) color.a = 1f;
                            break;
                        }
                    }

                    _mainTextObject.color = color;
                    _mainTextObject.text = message;
                    _nameObject.color = color;
                    _nameObject.text = name;

                    // ログには名前も残す（カラータグ付き）
                    string colorHex = ColorUtility.ToHtmlStringRGB(color);
                    TextLog += $"<color=#{colorHex}>{name}:{message}</color>\n";

                    // ★キャラごとに文字送りSEを設定
                    switch (name)
                    {
                        case "ミヤビ": currentTypingSE = enAudioClip.UI_Talk_miyabi; break;
                        case "主人公": currentTypingSE = enAudioClip.UI_Talk_shujinkou; break;
                        case "???": currentTypingSE = enAudioClip.UI_Talk_nazo; break;
                        case "":
                        case " ": currentTypingSE = enAudioClip.UI_Talk_narration; break;
                        default: currentTypingSE = enAudioClip.UI_Talk_narration; break;
                    }
                }
                else
                {
                    _mainTextObject.color = Color.white;
                    _mainTextObject.text = sentence.Trim();
                    _nameObject.color = Color.white;
                    _nameObject.text = string.Empty;
                    TextLog += sentence + "\n";

                    // 名前がない場合はナレーション音
                    currentTypingSE = enAudioClip.UI_Talk_narration;
                }

                _mainTextObject.maxVisibleCharacters = 0;
                _isTyping = true;
            }
        }
    }
}
