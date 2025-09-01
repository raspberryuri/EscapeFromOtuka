using UnityEngine;
using UnityEngine.InputSystem;

namespace MainGame
{
    // CharacterControllerコンポーネントを必須にする
    [RequireComponent(typeof(CharacterController))]
    public class NewPlayerOperation : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float gravity = -19.62f; // CharacterControllerは自前で重力計算が必要

        [Header("Look Settings")]
        [SerializeField] private float lookSpeed = 1.0f;
        [SerializeField] private Transform playerCamera;

        [Header("UI Settings")]
        [SerializeField] private GameObject mouseSensitivityCanvas; // 感度調整用UI

        [Header("Shooting Settings")]
        [SerializeField] private float maxRayDistance = 2f;

        // Public property for sensitivity
        public float mouseSensitivity = 1.0f;

        // Private member variables
        private CharacterController controller;
        private Vector3 playerVelocity; // Y軸の速度（重力など）を保持
        private float rotationX = 0f; // Player body rotation (Y-axis)
        private float rotationY = 0f; // Camera rotation (X-axis)

        private Vector2 movementInput = Vector2.zero;
        private Vector2 lookInput = Vector2.zero;

        private bool isSensitivityUIActive = false;
        private GameUIManager uiManager;

        private void Start()
        {
            controller = GetComponent<CharacterController>();

            // 初期カーソル設定
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // 感度調整UIを初期状態では非表示にする
            if (mouseSensitivityCanvas != null)
            {
                mouseSensitivityCanvas.SetActive(false);
            }

            // GameUIManagerとの連携
            uiManager = FindObjectOfType<GameUIManager>();
            if (uiManager != null)
            {
                mouseSensitivity = uiManager.GetCurrentMouseSensitivity();
                uiManager.OnMouseSensitivityChangedEvent += OnSensitivityChanged;
            }
        }

        private void OnDestroy()
        {
            if (uiManager != null)
            {
                uiManager.OnMouseSensitivityChangedEvent -= OnSensitivityChanged;
            }
        }

        private void OnSensitivityChanged(float newSensitivity)
        {
            mouseSensitivity = newSensitivity;
        }

        private void Update()
        {
            // UI表示中はプレイヤーの操作を停止
            if (isSensitivityUIActive) return;

            HandleMovement();
            HandleLook();
            HandleAimRaycast();
        }

        /// <summary>
        /// プレイヤーの移動処理 (CharacterController.Moveを使用)
        /// </summary>
        private void HandleMovement()
        {
            // 地面に設置しているかチェック
            if (controller.isGrounded && playerVelocity.y < 0)
            {
                playerVelocity.y = -2f; // 少しだけ下向きの力をかけて地面に吸着させる
            }

            // カメラの向きを基準に水平な移動方向を計算
            Vector3 camForward = playerCamera.forward;
            Vector3 camRight = playerCamera.right;
            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDirection = (camForward * movementInput.y + camRight * movementInput.x);

            // 移動を適用
            controller.Move(moveDirection * moveSpeed * Time.deltaTime);

            // 重力を適用
            playerVelocity.y += gravity * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
        }

        /// <summary>
        /// 視点操作の処理
        /// </summary>
        private void HandleLook()
        {
            rotationX += lookInput.x * lookSpeed * mouseSensitivity * Time.deltaTime * 100f;
            rotationY += lookInput.y * lookSpeed * mouseSensitivity * Time.deltaTime * 100f;
            rotationY = Mathf.Clamp(rotationY, -90f, 90f);

            transform.rotation = Quaternion.Euler(0f, rotationX, 0f);
            playerCamera.localRotation = Quaternion.Euler(-rotationY, 0f, 0f);
        }

        /// <summary>
        /// 照準がターゲットに合っているか判定するRaycast
        /// </summary>
        private void HandleAimRaycast()
        {
            Ray ray = new Ray(playerCamera.position, playerCamera.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance))
            {
                GameUIManager.Isaimactive = hit.collider.TryGetComponent(out TargetHit target);
            }
            else
            {
                GameUIManager.Isaimactive = false;
            }
        }

        #region Input System Callbacks

        public void OnMove(InputAction.CallbackContext context)
        {
            movementInput = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            lookInput = context.ReadValue<Vector2>();
            Debug.Log("Look Input: " + context.ReadValue<Vector2>());
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            Ray ray = new Ray(playerCamera.position, playerCamera.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance))
            {
                if (hit.collider.TryGetComponent(out TargetHit target))
                {
                    target.TriggerHit();
                }
            }
        }

        public void OnToggleSensitivityUI(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                isSensitivityUIActive = !isSensitivityUIActive;

                if (mouseSensitivityCanvas != null)
                {
                    mouseSensitivityCanvas.SetActive(isSensitivityUIActive);
                }

                Cursor.lockState = isSensitivityUIActive ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = isSensitivityUIActive;
            }
        }

        #endregion
    }
}