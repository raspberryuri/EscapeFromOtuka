using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MainGame
{
    public class GameUIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private Image TimerImage;
        [SerializeField] private Image AimImage;
        [SerializeField] private Sprite ActiveAim;
        [SerializeField] private Sprite UnactiveAim;
        [SerializeField] private CanvasGroup InputFieldPanel;
        [SerializeField] private CanvasGroup GameCanvas;

        // 新規追加：マウス感度調整UIのCanvasGroupとスライダー
        [SerializeField] private CanvasGroup MouseSensitivityPanel;
        [SerializeField] private Slider MouseSensitivitySlider;

        public static bool Isaimactive = false;
        public delegate void MouseSensitivityChangedHandler(float newSensitivity);
        public event MouseSensitivityChangedHandler OnMouseSensitivityChangedEvent;

        // 現在のマウス感度（デフォルト1.0f）
        private float currentMouseSensitivity = 1.0f;

        void Start()
        {
            if (MouseSensitivityPanel != null)
                SetCanvasGroupActive(MouseSensitivityPanel, false);

            if (MouseSensitivitySlider != null)
            {
                MouseSensitivitySlider.minValue = 0.1f;
                MouseSensitivitySlider.maxValue = 5.0f;
                MouseSensitivitySlider.value = currentMouseSensitivity;

                MouseSensitivitySlider.onValueChanged.AddListener(OnMouseSensitivityChanged);
            }
        }

        void Update()
        {
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

            Cursor.lockState = active ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = active;
        }

        public void OnInputArea()
        {
            AimImage.sprite = (Isaimactive) ? ActiveAim : UnactiveAim;
        }

        // マウス感度パネルの表示切替
        public void ToggleMouseSensitivityPanel(bool active)
        {
            if (MouseSensitivityPanel == null) return;

            SetCanvasGroupActive(MouseSensitivityPanel, active);
            GameMode type = active ? GameMode.InputField : GameMode.MainGame; 
            Time.timeScale = (active ? 0 : 1);
            InputManager.Instance.SwitchActionMap(type);
            // 表示状態によってカーソルロックを切り替え
            Cursor.lockState = active ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = active;
        }

        private void OnMouseSensitivityChanged(float value)
        {
            currentMouseSensitivity = value;
            Inputoperation.mouseSensitivity = value;

            // 感度変更を通知
            OnMouseSensitivityChangedEvent?.Invoke(value);
        }

        private void SetCanvasGroupActive(CanvasGroup cg, bool active)
        {
            cg.alpha = active ? 1 : 0;
            cg.interactable = active;
            cg.blocksRaycasts = active;
        }

        // 外部から現在の感度を取得できるように
        public float GetCurrentMouseSensitivity()
        {
            return currentMouseSensitivity;
        }
    }
}
