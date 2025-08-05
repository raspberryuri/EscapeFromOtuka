using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

namespace NovelSystems
{
    public class NovelTextController : MonoBehaviour
    {
        private ICommandExecutor _commandExecutor;
        private IAudioController _audioController;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI mainTextObject;
        [SerializeField] private TextMeshProUGUI nameObject;

        [Header("文字送り設定")]
        [SerializeField] private float feedTime = 0.05f;

        [Header("待機時間（秒）")]
        [SerializeField] private float defaultWaitTime = 0f;
        private float waitDuration = 0f;

        [Header("キャラクター色設定")]
        [SerializeField] private List<CharacterColor> characterColors = new List<CharacterColor>();

        private List<string> lines; // 読み込んだテキスト行
        private int currentLineIndex = 0;

        private int displayedLength = 0;
        private float timer = 0f;
        private bool isTyping = false;
        private bool isWaiting = false;

        private enAudioClip currentTypingSE = enAudioClip.UI_Talk_narration;

        public void Initialize(TextAsset scenarioText, ICommandExecutor commandExecutor, IAudioController audioController)
        {
            _commandExecutor = commandExecutor;
            _audioController = audioController;

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
                OnScenarioEnd();
                yield break;
            }

            string line = lines[currentLineIndex];

            if (IsStatement(line))
            {
                ExecuteCommand(line);
                yield return HandleWait();
                currentLineIndex++;
                StartCoroutine(ProcessCurrentLine());
            }
            else
            {
                // 表示用テキスト処理
                string name = "";
                string message = line;
                if (line.Contains(":"))
                {
                    var parts = line.Split(new char[] { ':' }, 2);
                    name = parts[0].Trim();
                    message = parts[1].Trim();
                }

                SetNameAndColor(name);
                mainTextObject.text = message;
                displayedLength = 0;
                isTyping = true;
                mainTextObject.maxVisibleCharacters = 0;

                while (isTyping)
                {
                    timer += Time.deltaTime;
                    if (timer >= feedTime)
                    {
                        timer = 0f;
                        displayedLength++;
                        mainTextObject.maxVisibleCharacters = Mathf.Min(displayedLength, message.Length);

                        // 文字送りSE
                        _audioController?.PlaySE((int)currentTypingSE);

                        if (displayedLength >= message.Length)
                            isTyping = false;
                    }
                    yield return null;
                }

                yield return HandleWait();

                currentLineIndex++;
                StartCoroutine(ProcessCurrentLine());
            }
        }

        private IEnumerator HandleWait()
        {
            float waitTime = waitDuration > 0f ? waitDuration : defaultWaitTime;
            waitDuration = 0f;
            if (waitTime > 0f)
            {
                isWaiting = true;
                float timer = 0f;
                while (timer < waitTime)
                {
                    timer += Time.deltaTime;
                    yield return null;
                }
                isWaiting = false;
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
                        _commandExecutor.ChangeScene(words[1]);
                    break;
                case "&wait":
                    if (words.Length >= 2 && float.TryParse(words[1], out float sec))
                        waitDuration = sec;
                    break;
                case "&se":
                    if (words.Length >= 2 && int.TryParse(words[1], out int seId))
                        _audioController.PlaySE(seId);
                    break;
                case "&bgm":
                    if (words.Length >= 2)
                    {
                        if (words[1].ToLower() == "stop")
                            _audioController.StopBGM();
                        else if (int.TryParse(words[1], out int bgmId))
                            _audioController.PlayBGM(bgmId);
                    }
                    break;
                default:
                    Debug.LogWarning($"Unknown command: {line}");
                    break;
            }
        }

        private void SetNameAndColor(string name)
        {
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
            nameObject.color = color;
            mainTextObject.color = color;
            nameObject.text = name;
        }

        private void OnScenarioEnd()
        {
            Debug.Log("Scenario ended");
            // ノベル終了時の処理をここに（UI非表示など）
        }

        public void OnClick()
        {
            if (isWaiting)
                return;

            if (isTyping)
            {
                // 文字送りスキップして全文表示
                displayedLength = mainTextObject.text.Length;
                mainTextObject.maxVisibleCharacters = displayedLength;
                isTyping = false;
            }
            else
            {
                if (currentLineIndex < lines.Count)
                {
                    StopAllCoroutines();
                    StartCoroutine(ProcessCurrentLine());
                }
                else
                {
                    OnScenarioEnd();
                }
            }
        }

        public void ResetAll()
        {
            StopAllCoroutines();
            mainTextObject.text = "";
            nameObject.text = "";
            displayedLength = 0;
            timer = 0f;
            isTyping = false;
            isWaiting = false;
            waitDuration = 0f;
        }
    }

    [System.Serializable]
    public class CharacterColor
    {
        public string characterName;
        public Color color;
    }
    // インターフェース：UIやシーン操作
    public interface ICommandExecutor
    {
        void PutImage(string name, string spriteName);
        void RemoveImage(string name);
        void RemoveAllImages();
        void ChangeScene(string sceneName);
    }

    // インターフェース：BGM・SE 操作
    public interface IAudioController
    {
        void PlaySE(int seId);
        void PlayBGM(int bgmId);
        void StopBGM();
    }
}
