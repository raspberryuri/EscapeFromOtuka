using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace NovelGame
{
    public enum ScenarioType
    {
        CorrectTakayuka = 0,
        CorrectMaze = 1,
        CorrectPot = 2,
        UnPassword = 3,
        GameOver = 4,
        Clear = 5,
        // ここまで既存
        Main = 7, // ★ メインシナリオ追加
    }

    public class UserScriptManager : MonoBehaviour
    {
        [Header("メインシナリオ")]
        [SerializeField] private TextAsset _textFile;

        [Header("各種イベント用シナリオ")]
        [SerializeField] private TextAsset _wrongAnswerTextFile;      // 不正解用
        [SerializeField] private TextAsset _gameOverTextFile;         // ゲームオーバー
        [SerializeField] private TextAsset _clearTextFile;            // ゲームクリア（汎用）

        [Header("正解後の詳細シナリオ")]
        [SerializeField] private TextAsset _correctTakayukaText;      // 高床倉庫 正解
        [SerializeField] private TextAsset _correctMazeText;          // 竪穴住居（迷路）正解
        [SerializeField] private TextAsset _correctPotText;           // 竪穴住居（ツボ）正解

        public static int LineCount;

        private List<string> _sentences = new List<string>();

        void OnEnable()
        {
            // ★初期状態では何もロードしない。必要に応じてLoadScenarioを呼ぶ
            LineCount = 0;
        }

        /// <summary>
        /// 指定の TextAsset から文章リストを読み込む
        /// </summary>
        private void LoadSentencesFromTextAsset(TextAsset textAsset)
        {
            _sentences.Clear();

            if (textAsset == null)
            {
                Debug.LogWarning("指定された TextAsset が null です。");
                return;
            }

            using (StringReader reader = new StringReader(textAsset.text))
            {
                while (reader.Peek() != -1)
                {
                    string line = reader.ReadLine();
                    _sentences.Add(line);
                }
            }

            LineCount = _sentences.Count;
        }

        /// <summary>
        /// 外部からシナリオを切り替える
        /// </summary>
        public void LoadScenario(ScenarioType type)
        {
            TextAsset scenario = type switch
            {
                ScenarioType.UnPassword => _wrongAnswerTextFile,
                ScenarioType.GameOver => _gameOverTextFile,
                ScenarioType.Clear => _clearTextFile,
                ScenarioType.CorrectTakayuka => _correctTakayukaText,
                ScenarioType.CorrectMaze => _correctMazeText,
                ScenarioType.CorrectPot => _correctPotText,
                ScenarioType.Main => _textFile, // ★ 追加
                _ => null
            };

            if (scenario == null)
            {
                Debug.LogError($"シナリオ '{type}' に対応する TextAsset が設定されていません。");
                return;
            }

            LoadSentencesFromTextAsset(scenario);
            NovelManager.Instance.lineNumber = 0;
        }

        public string GetCurrentSentence()
        {
            return _sentences[NovelManager.Instance.lineNumber];
        }

        /// <summary>
        /// 現在が最終行かどうかをチェック
        /// </summary>
        public bool IsLastLine()
        {
            return NovelManager.Instance.lineNumber >= _sentences.Count - 1;
        }

        /// <summary>
        /// 次の行があるかどうかをチェック
        /// </summary>
        public bool HasNextLine()
        {
            return NovelManager.Instance.lineNumber < _sentences.Count - 1;
        }

        /// <summary>
        /// 現在のシナリオの総行数を取得
        /// </summary>
        public int GetTotalLines()
        {
            return _sentences.Count;
        }

        public bool IsStatement(string sentence)
        {
            return sentence.Length > 0 && sentence[0] == '&';
        }
        public void ResetAll()
        {
            // 現在のシナリオを全クリア
            _sentences.Clear();
            LineCount = 0;

            // 現在行もリセット
            if (NovelManager.Instance != null)
            {
                NovelManager.Instance.lineNumber = 0;
            }
        }
        public void ExecuteStatement(string sentence)
        {
            string[] words = sentence.Split(' ');
            switch (words[0])
            {
                case "&img":
                    NovelManager.Instance.imageManager.PutImage(words[1], words[2]);
                    break;

                case "&rmimg":
                    NovelManager.Instance.imageManager.RemoveImage(words[1]);
                    break;

                case "&rmALL":
                    NovelManager.Instance.imageManager.RemoveAllImages();
                    break;

                case "&sceCh":
                    NovelManager.SceneChanger(words[1]);
                    break;

                case "&wait":
                    if (words.Length >= 2 && float.TryParse(words[1], out float sec))
                    {
                        MainTextController.WaitDuration = sec;
                    }
                    else
                    {
                        Debug.LogWarning("Invalid &wait command format.");
                    }
                    break;

                case "&se":  // SE再生コマンド
                    if (words.Length >= 2)
                    {
                        if (int.TryParse(words[1], out int clipId))
                        {
                            AudioManager.Instance?.OneShotSE_UI(50 + clipId);
                        }
                        else
                        {
                            Debug.LogWarning($"[ExecuteStatement] 無効なSE指定: {words[1]}");
                        }
                    }
                    break;

                case "&bgm":  // BGM再生コマンド
                    if (words.Length >= 2)
                    {
                        if (words[1].ToLower() == "stop")
                        {
                            AudioManager.Instance?.StopBGM();
                        }
                        else if (int.TryParse(words[1], out int bgmId))
                        {
                            AudioManager.Instance?.PlayBGM((enAudioClip)(100 + bgmId));
                        }
                        else
                        {
                            Debug.LogWarning($"[ExecuteStatement] 無効なBGM指定: {words[1]}");
                        }
                    }
                    break;
            }
        }

        // ==========================
        // 各種シナリオ取得関数
        // ==========================

        public TextAsset GetWrongAnswerText() => _wrongAnswerTextFile;
        public TextAsset GetGameOverText() => _gameOverTextFile;
        public TextAsset GetClearText() => _clearTextFile;

        /// <summary>
        /// 正解後のシナリオをIDで分岐
        /// 0: 高床, 1: 竪穴迷路, 2: 竪穴ツボ
        /// </summary>
        public TextAsset GetCorrectTextByID(int id)
        {
            return id switch
            {
                0 => _correctTakayukaText,
                1 => _correctMazeText,
                2 => _correctPotText,
                _ => _correctTakayukaText
            };
        }
    }
}
