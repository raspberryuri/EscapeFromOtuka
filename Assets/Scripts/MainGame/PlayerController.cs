using UnityEngine;
using UnityEngine.InputSystem;

namespace MainGame
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [Header("移動設定")]
        [SerializeField] private float moveSpeed = 10f;           // ← インスペクターで設定可能
        [SerializeField] private Transform playerBody;
        [SerializeField] private Transform playerCamera;

        [Header("マウス感度")]
        [SerializeField] private float mouseSensitivity = 1.0f;   // ← インスペクターで設定可能
        [SerializeField] private GameObject mouseSensitivityCanvas;

        private Rigidbody rb;
        private Vector2 moveInput = Vector2.zero;
        private Vector2 lookInput = Vector2.zero;

        private float xRotation = 0f;
        private bool isSensitivityUIActive = false;

        private const int MaxRayDistance = 2;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
            rb.useGravity = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (mouseSensitivityCanvas != null)
                mouseSensitivityCanvas.SetActive(false);
        }

        private void Update()
        {
            if (!isSensitivityUIActive)
            {
                Look();
                AimCheck();
            }
        }

        private void FixedUpdate()
        {
            if (!isSensitivityUIActive)
            {
                Move();
            }
            else
            {
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
            }
        }

        // ==================
        // 入力イベント
        // ==================
        public void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            if (!isSensitivityUIActive)
            {
                lookInput = context.ReadValue<Vector2>();
            }
            else
            {
                lookInput = Vector2.zero;
            }
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            if (context.performed && !isSensitivityUIActive)
            {
                Ray ray = new Ray(playerCamera.position, playerCamera.forward);
                if (Physics.Raycast(ray, out RaycastHit hit, MaxRayDistance))
                {
                    if (hit.collider.TryGetComponent(out TargetHit target))
                    {
                        target.TriggerHit();
                    }
                }
            }
        }

        public void OnToggleSensitivityUI(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                isSensitivityUIActive = !isSensitivityUIActive;

                if (mouseSensitivityCanvas != null)
                    mouseSensitivityCanvas.SetActive(isSensitivityUIActive);

                Cursor.lockState = isSensitivityUIActive ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = isSensitivityUIActive;
            }
        }

        // ==================
        // 実際の処理
        // ==================
        private void Move()
        {
            Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y).normalized;
            Vector3 moveVelocity = (playerBody.rotation * moveDir) * moveSpeed;
            rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);
        }

        private void Look()
        {
            playerBody.Rotate(Vector3.up * lookInput.x * mouseSensitivity);

            xRotation -= lookInput.y * mouseSensitivity;
            xRotation = Mathf.Clamp(xRotation, -89f, 89f);

            playerCamera.localEulerAngles = new Vector3(xRotation, 0f, 0f);
        }

        private void AimCheck()
        {
            Ray ray = new Ray(playerCamera.position, playerCamera.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, MaxRayDistance))
            {
                GameUIManager.Isaimactive = hit.collider.TryGetComponent(out TargetHit target);
            }
            else
            {
                GameUIManager.Isaimactive = false;
            }
        }
    }
}
