using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MainGame
{
    public class GameUIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private Image TimerImage;       // タイマー用UI
        [SerializeField] private Image AimImage;         // インタラクティブUI
        [SerializeField] private Sprite ActiveAim;       // アクティブUI
        [SerializeField] private Sprite UnactiveAim;     // 非アクティブUI
        [SerializeField] private CanvasGroup InputFieldPanel;
        [SerializeField] private CanvasGroup GameCanvas;

        public static bool Isaimactive = false;

        void Update()
        {
            // タイマーUI更新
            if (GameManager.Instance != null)
            {
                TimerImage.fillAmount = Mathf.Clamp01(GameManager.Instance.GetRemainingTimeRate());
                UpdateScoreText();
            }

            OnInputArea();
        }

        private void UpdateScoreText()
        {
            if (scoreText != null && GameManager.Instance != null)
            {
                int current = GameManager.Instance.Score;
                int max = GameManager.Instance.MaxScore;
                scoreText.text = $"{max - current}/{max}";
            }
        }

        public void OnInputField(bool active)
        {
            Debug.Log($"InputField {(active ? "表示" : "非表示")} にしました");

            InputFieldPanel.interactable = active;
            InputFieldPanel.blocksRaycasts = active;
            InputFieldPanel.alpha = active ? 1 : 0;

            GameCanvas.interactable = !active;
            GameCanvas.blocksRaycasts = !active;
            GameCanvas.alpha = !active ? 1 : 0;

            // カーソル表示・ロック切り替え
            Cursor.lockState = active ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = active;
        }

        public void OnInputArea()
        {
            AimImage.sprite = (Isaimactive) ? ActiveAim : UnactiveAim;
        }
    }
}
