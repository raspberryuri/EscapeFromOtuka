using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace MainGame
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("正解リスト（インデックスに対応）")]
        [SerializeField] private List<string> correctAnswers = new List<string> { "Apple", "Banana", "Cherry" };

        [Header("UI 要素")]
        [SerializeField] private TMP_InputField inputField;

        [Header("制限時間")]
        [SerializeField] private float maxTimeSec = 60f;
        private float timer = 0f;

        [Header("スコア管理（3問）")]
        public bool[] correctFlags = new bool[3]; // 0:高床,1:竪穴迷路,2:竪穴ツボ

        private GameUIManager gameUIManager;

        public static int InputPanelID = 0; //0:高床, 1:竪穴迷路, 2:竪穴ツボ

        private bool isClearing = false; // クリア遷移中フラグ

        // ====== プロパティ ======
        public bool IsTimeUp => timer >= maxTimeSec;
        public int Score => CountTrueFlags();
        public int MaxScore => correctFlags.Length;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            gameUIManager = GetComponent<GameUIManager>();
            NovelSystems.NovelAudioManager.Instance.PlayBGM(NovelSystems.enAudioClip.GameBGM_MainBGM);
            InputManager.Instance?.SwitchActionMap(GameMode.MainGame);
            NovelSystems.NovelManager.Instance.returnActionMap = GameMode.MainGame;
            NovelSystems.NovelManager.Instance.SetHomeAndLogButtonsActive(false);
        }

        private void Update()
        {
            if (!IsTimeUp)
            {
                timer += Time.deltaTime;
            }
            else
            {
                SceneManager.LoadScene(4);//ゲームオーバーシナリオへ
            }
        }

        // ====== タイマー操作 ======
        public float GetRemainingTimeRate()
        {
            return 1f - (timer / maxTimeSec);
        }

        public void ResetTimer()
        {
            timer = 0f;
        }

        // ====== スコア操作 ======
        private int CountTrueFlags()
        {
            int count = 0;
            foreach (bool flag in correctFlags)
                if (flag) count++;
            return count;
        }

        public void SetCorrect(int index)
        {
            if (index < 0 || index >= correctFlags.Length) return;

            if (correctFlags[index]) return;

            correctFlags[index] = true;

            if (Score >= MaxScore && !isClearing)
            {
                StartCoroutine(DelayedClearTransition());
            }
        }

        private IEnumerator DelayedClearTransition()
        {
            isClearing = true;

            // 入力モードを NovelInput に切り替え
            InputManager.Instance?.SwitchActionMap(GameMode.NovelGame);

            // 1秒待つ
            yield return new WaitForSeconds(1f);

            // シーン切り替え（クリアシーン）
            SceneManager.LoadScene(3);
        }

        public void ResetScore()
        {
            for (int i = 0; i < correctFlags.Length; i++)
                correctFlags[i] = false;
        }

        // ====== 入力モード操作 ======
        public void ChangeInputMode(bool isActive)
        {
            AudioManager.Instance.OneShotSE(enAudioClip.UI_OpenInput);

            Time.timeScale = isActive ? 0 : 1;
            InputManager.Instance.SwitchActionMap(isActive ? GameMode.InputField : GameMode.MainGame);
            gameUIManager.OnInputField(isActive, InputPanelID);
        }

        public void OnTextInputing()
        {
            AudioManager.Instance.OneShotSE(enAudioClip.UI_Talk_narration);
        }

        private void ToggleTouch(int index)
        {
            GameObject tateanaPanel = GameObject.Find($"InputPanel:{index}");
            if (tateanaPanel == null)
            {
                Debug.LogWarning($"InputPanel:{index} が見つかりません");
                return;
            }

            Transform touch = tateanaPanel.transform.Find("torch");
            Transform touchOn = tateanaPanel.transform.Find("torchOn");
            Transform InputImage = tateanaPanel.transform.Find("InputImage_" + index);

            if (touch != null) touch.gameObject.SetActive(false);
            if (touchOn != null) touchOn.gameObject.SetActive(true);
        }

        // ====== 回答チェック ======
        public void CheckAnswer()
        {
            if (inputField == null)
            {
                Debug.LogWarning("InputField が設定されていません");
                return;
            }

            string userInput = inputField.text.Trim();
            string correctAnswer = correctAnswers[InputPanelID];

            // 入力と答えをひらがなに統一して比較（英語は小文字化）
            string normalizedInput = ToHiragana(userInput).ToLowerInvariant();
            string normalizedAnswer = ToHiragana(correctAnswer).ToLowerInvariant();

            if (normalizedInput == normalizedAnswer)
            {
                HandleCorrectAnswer(InputPanelID);
            }
            else
            {
                HandleIncorrectAnswer(userInput, correctAnswer);
            }
        }

        public void OnReturnTitle()
        {
            ResetScore();
            ResetTimer();
            InputPanelID = 0;

            if (inputField != null) inputField.text = "";
            Time.timeScale = 1f;

            NovelSystems.NovelAudioManager.Instance.PlayBGM(NovelSystems.enAudioClip.BGM_default);

            SceneManager.LoadScene(0);
        }

        private void HandleCorrectAnswer(int index)
        {
            AudioManager.Instance.OneShotSE(enAudioClip.UI_InputHit);

            SetCorrect(index);
            EnterNovelMode();

            var novel = NovelSystems.NovelManager.Instance;
            novel.PlayScenario("CollectAnswer");

            ToggleTouch(index);
            inputField.text = "";
        }

        private void HandleIncorrectAnswer(string input, string correct)
        {
            AudioManager.Instance.OneShotSE(enAudioClip.UI_InputMiss);
            EnterNovelMode();

            Debug.Log($"不正解：入力='{input}' / 正解='{correct}'");

            var novel = NovelSystems.NovelManager.Instance;
            novel.PlayScenario("MissAnswer");
        }

        private void EnterNovelMode()
        {
            ChangeInputMode(false);
            InputManager.Instance?.SwitchActionMap(GameMode.NovelGame);
        }

        // ====== ひらがな統一変換 ======
        private string ToHiragana(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            // 全角に統一
            string normalized = input.Normalize(NormalizationForm.FormKC);

            var sb = new StringBuilder(normalized.Length);
            foreach (char c in normalized)
            {
                if (c >= 'ァ' && c <= 'ン')
                {
                    sb.Append((char)(c - 'ァ' + 'ぁ')); // カタカナ→ひらがな
                }
                else if (c == 'ヵ') sb.Append('か');
                else if (c == 'ヶ') sb.Append('け');
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}
