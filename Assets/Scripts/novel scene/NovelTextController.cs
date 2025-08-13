using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;

namespace NovelSystems
{
    public class NovelTextController : MonoBehaviour
    {
        private ICommandExecutor _commandExecutor;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI mainTextObject;
        [SerializeField] private TextMeshProUGUI nameObject;

        [Header("文字送り設定")]
        [SerializeField] private float feedTime = 0.05f;

        [Header("キャラクター設定")]
        [SerializeField] private List<CharacterSetting> characterSettings = new List<CharacterSetting>();

        private List<string> lines;
        private int currentLineIndex = 0;

        private int displayedLength = 0;
        private float timer = 0f;
        private bool isTyping = false;
        private bool lineComplete = false;
        private bool isWaiting = false;
        private float waitDuration = 0f;

        private enAudioClip currentTypingSE = enAudioClip.UI_Talk_narration;
        private NovelUIManager _uiManager;

        public void Initialize(TextAsset scenarioText, ICommandExecutor commandExecutor, NovelUIManager uiManager)
        {
            _commandExecutor = commandExecutor;
            _uiManager = uiManager;

            lines = LoadLines(scenarioText);
            currentLineIndex = 0;

            StartCoroutine(ProcessCurrentLine());
        }


        private List<string> LoadLines(TextAsset textAsset)
        {
            List<string> result = new List<string>();
            using (StringReader reader = new StringReader(textAsset.text))
            {
                while (reader.Peek() != -1)
                    result.Add(reader.ReadLine());
            }
            return result;
        }

        private IEnumerator ProcessCurrentLine()
        {
            if (currentLineIndex >= lines.Count)
            {
                OnLastLineFinished(); // ← 最終行処理を追加
                OnScenarioEnd();
                yield break;
            }

            string line = lines[currentLineIndex];

            // コマンド行
            if (IsStatement(line))
            {
                ExecuteCommand(line);

                if (waitDuration > 0f)
                {
                    isWaiting = true;
                    float t = 0f;
                    while (t < waitDuration)
                    {
                        t += Time.deltaTime;
                        yield return null;
                    }
                    isWaiting = false;
                    waitDuration = 0f;
                }

                currentLineIndex++;
                StartCoroutine(ProcessCurrentLine());
                yield break;
            }

            // 通常テキスト
            string name = "";
            string message = line;
            if (line.Contains(":"))
            {
                var parts = line.Split(new char[] { ':' }, 2);
                name = parts[0].Trim();
                message = parts[1].Trim();
            }

            SetCharacter(name);

            // ログ追加
            // ログ追加
            if (_uiManager != null)
            {
                string logEntry;
                if (!string.IsNullOrEmpty(name))
                {
                    // キャラクター名は太字＆色付き
                    string colorCode = ColorUtility.ToHtmlStringRGB(nameObject.color);
                    logEntry = $"\n<b><color=#{colorCode}>{name}</color></b>：{message}";
                }
                else
                {
                    // ナレーションは灰色＋斜体
                    logEntry = $"\n<i><color=#FFFFFF>{message}</color></i>";
                }
                _uiManager.AddLog(logEntry);
            }


            mainTextObject.text = message;
            displayedLength = 0;
            isTyping = true;
            lineComplete = false;
            mainTextObject.maxVisibleCharacters = 0;

            while (displayedLength < message.Length)
            {
                if (!isTyping) break;

                timer += Time.deltaTime;
                if (timer >= feedTime)
                {
                    timer = 0f;
                    displayedLength++;
                    mainTextObject.maxVisibleCharacters = displayedLength;

                    NovelAudioManager.Instance?.OneShotSE(currentTypingSE);
                }
                yield return null;
            }

            displayedLength = message.Length;
            mainTextObject.maxVisibleCharacters = displayedLength;
            isTyping = false;
            lineComplete = true;

            yield break;
        }

        public void OnClick()
        {
            if (isWaiting) return;

            if (isTyping)
            {
                displayedLength = mainTextObject.text.Length;
                mainTextObject.maxVisibleCharacters = displayedLength;
                isTyping = false;
                lineComplete = true;
            }
            else if (lineComplete)
            {
                currentLineIndex++;
                NovelAudioManager.Instance.OneShotSE(enAudioClip.UI_Talk_Next);
                StartCoroutine(ProcessCurrentLine());
            }
        }

        private bool IsStatement(string line)
        {
            return !string.IsNullOrEmpty(line) && line.StartsWith("&");
        }

        private void ExecuteCommand(string line)
        {
            var words = line.Split(' ');
            switch (words[0])
            {
                case "&img":
                    if (words.Length >= 3)
                        _commandExecutor.PutImage(words[1], words[2]);
                    break;
                case "&rmimg":
                    if (words.Length >= 2)
                        _commandExecutor.RemoveImage(words[1]);
                    break;
                case "&rmALL":
                    _commandExecutor.RemoveAllImages();
                    break;
                case "&sceCh":
                    if (words.Length >= 2)
                    {
                        if (words[1] == "Title") NovelManager.Instance.OnHomeButtonClicked();
                        else SceneManager.LoadScene(words[1]);
                    }
                    break;
                case "&wait":
                    if (words.Length >= 2 && float.TryParse(words[1], out float sec))
                        waitDuration = sec;
                    break;
                case "&se":
                    if (words.Length >= 2 && int.TryParse(words[1], out int seId))
                        NovelAudioManager.Instance.OneShotSE((enAudioClip)(seId + 50));
                    break;
                case "&bgm":
                    if (words.Length >= 2)
                    {
                        if (words[1].ToLower() == "stop")
                            NovelAudioManager.Instance.StopBGM();
                        else if (int.TryParse(words[1], out int bgmId))
                            NovelAudioManager.Instance.PlayBGM((enAudioClip)(bgmId + 100));
                    }
                    break;
                default:
                    Debug.LogWarning($"Unknown command: {line}");
                    break;
            }
        }

        private void SetCharacter(string name)
        {
            Color color = Color.white;
            currentTypingSE = enAudioClip.UI_Talk_narration;

            foreach (var cs in characterSettings)
            {
                if (cs.characterName == name)
                {
                    color = cs.color;
                    if (color.a == 0f) color.a = 1f;
                    currentTypingSE = cs.typingSE;
                    break;
                }
            }

            nameObject.color = color;
            mainTextObject.color = color;
            nameObject.text = name;
        }

        private void OnScenarioEnd()
        {
            Debug.Log("Scenario ended");
        }

        /// <summary>
        /// 最終行に到達したときに呼ばれる関数
        /// </summary>
        protected virtual void OnLastLineFinished()
        {
            Debug.Log("TextAsset の最終行まで到達しました");
            NovelManager.Instance.SetNovelActive(false);
        }

        public void ResetAll()
        {
            StopAllCoroutines();
            mainTextObject.text = "";
            nameObject.text = "";
            displayedLength = 0;
            timer = 0f;
            isTyping = false;
            lineComplete = false;
            waitDuration = 0f;
            isWaiting = false;
        }
    }

    [System.Serializable]
    public class CharacterSetting
    {
        public string characterName;
        public Color color = Color.white;
        public enAudioClip typingSE = enAudioClip.UI_Talk_narration;
    }

    public interface ICommandExecutor
    {
        void PutImage(string name, string spriteName);
        void RemoveImage(string name);
        void RemoveAllImages();
    }
}
