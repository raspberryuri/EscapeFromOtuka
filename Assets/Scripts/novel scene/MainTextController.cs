using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace NovelGame
{
    public class MainTextController : MonoBehaviour
    {
        public UIManager uimanager;
        [SerializeField] TextMeshProUGUI _mainTextObject;
        int _displayedSentenceLength;
        float _time;
        float _feedTime;

        string TextLog = "";
        string Text = "";
        string[] parts;
        //string[] lines = TextLog.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        // Start is called before the first frame update
        void Start()
        {
            _time = 0f;
            _feedTime = 0.05f;

            // 最初の行のテキストを表示、または命令を実行
            string statement = GameManager.Instance.userScriptManager.GetCurrentSentence();
            if (GameManager.Instance.userScriptManager.IsStatement(statement))
            {
                GameManager.Instance.userScriptManager.ExecuteStatement(statement);
                GoToTheNextLine();
            }
            DisplayText();
        }

        // Update is called once per frame
        void Update()
        {
            // 文章を１文字ずつ表示する
            _time += Time.deltaTime;
            if (_time >= _feedTime)
            {
                _time -= _feedTime;
                if (!CanGoToTheNextLine())
                {
                    _displayedSentenceLength++;
                    _mainTextObject.maxVisibleCharacters = _displayedSentenceLength;
                }
            }

            // クリックされたとき、次の行へ移動
            if (Input.GetMouseButtonUp(0))
            {
                uimanager.TextDisplayer(TextLog);
                if (CanGoToTheNextLine())
                {
                    Text = GameManager.Instance.userScriptManager.GetCurrentSentence() + "\n";
                    parts = Text.Split(':');
                    Debug.Log(parts.Length);
                    if(parts.Length == 2)
                    {
                        string name  = parts[0];
                        string message = parts[1];
                        string color = "white";

                        //if (lines.Length > 0)
                        //{
                        //    TextLog = string.Join("\n", lines.Take(lines.Length - 1)) + "\n";
                        //}
                        //int lastNewline = TextLog.LastIndexOf('\n');
                        //if (lastNewline != -1)
                        //{
                        //    // 最後の改行より前を残す
                        //    TextLog = TextLog.Substring(0, lastNewline + 1);

                        //    // 不要な改行を取りたい場合は +1 を外す
                        //    // TextLog = TextLog.Substring(0, lastNewline);
                        //}
                        switch (name)
                        {
                            case "Takeshita":
                                color  = "red"; 
                                break;
                            case "Fujisawa":
                                color = "blue";
                                break;
                            case "Usami":
                                color = "green";
                                break;

                            default:
                                color = "gray";
                                break;
                        }
                        TextLog += $"<color={color}>{name}:{message}</color>\n";
                    }else
                    {
                        TextLog += Text;
                    }
                    Debug.Log(Text);
                    GoToTheNextLine();
                    DisplayText();
                }
            }
        }

        // その行の、すべての文字が表示されていなければ、まだ次の行へ進むことはできない
        public bool CanGoToTheNextLine()
        {
            string sentence = GameManager.Instance.userScriptManager.GetCurrentSentence();
            return (_displayedSentenceLength > sentence.Length);
        }

        // 次の行へ移動
        public void GoToTheNextLine()
        {
            _displayedSentenceLength = 0;
            _time = 0f;
            _mainTextObject.maxVisibleCharacters = 0;

            GameManager.Instance.lineNumber++;

            string sentence = GameManager.Instance.userScriptManager.GetCurrentSentence();

            if (GameManager.Instance.userScriptManager.IsStatement(sentence))
            {
                GameManager.Instance.userScriptManager.ExecuteStatement(sentence);
                GoToTheNextLine();
            }
        }

        // テキストを表示
        public void DisplayText()
        {
            string sentence = GameManager.Instance.userScriptManager.GetCurrentSentence();
            _mainTextObject.text = sentence;

            FindObjectOfType<LogManager>().AddLog(sentence);
        }
    }
}
