using UnityEngine;
using UnityEngine.InputSystem;

namespace MainGame
{
    [RequireComponent(typeof(Rigidbody))]
    public class Inputoperation : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private float lookSpeed = 1.0f; 
        [SerializeField] private Transform cameraTS;

        public static float mouseSensitivity = 1.0f;

        [SerializeField]
        private GameObject mouseSensitivityCanvas;  // ← 感度調整用UIのCanvas

        private Rigidbody rb;
        private float movementX;
        private float movementY;

        private float rotationX = 0f;
        private float rotationY = 0f;

        private bool isSensitivityUIActive = false;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            Cursor.lockState = CursorLockMode.Locked;

            if (mouseSensitivityCanvas != null)
            {
                mouseSensitivityCanvas.SetActive(false); // 最初は非表示
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.performed || context.started)
            {
                Vector2 movementVector = context.ReadValue<Vector2>();
                movementX = movementVector.x;
                movementY = movementVector.y;
            }
            else if (context.canceled)
            {
                movementX = 0f;
                movementY = 0f;
            }
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            Debug.Log("OnLookメソッドが呼ばれました！ マウスの入力値: " + context.ReadValue<Vector2>());
            if (isSensitivityUIActive) return; // UI表示中は視点操作無効化も可

            Vector2 lookInput = context.ReadValue<Vector2>();
            rotationX += lookInput.x * lookSpeed * mouseSensitivity * Time.deltaTime * 100f; // deltaTimeを使う場合、値を大きく調整
            rotationY += lookInput.y * lookSpeed * mouseSensitivity * Time.deltaTime * 100f;
            rotationY = Mathf.Clamp(rotationY, -90f, 90f);

            cameraTS.localRotation = Quaternion.Euler(-rotationY, 0f, 0f);
            transform.rotation = Quaternion.Euler(0f, rotationX, 0f);
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

                // UI表示中はカーソルを解放、非表示時はロック
                Cursor.lockState = isSensitivityUIActive ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = isSensitivityUIActive;
            }
        }

        private void FixedUpdate()
        {
            if (isSensitivityUIActive) return; // UI表示中は移動停止も可

            // カメラの前方ベクトルと右方ベクトルを取得
            Vector3 camForward = cameraTS.forward;
            Vector3 camRight = cameraTS.right;

            // Y軸の成分を0にして、水平なベクトルにする
            camForward.y = 0;
            camRight.y = 0;

            // ベクトルの長さを1に戻す（正規化）
            camForward.Normalize();
            camRight.Normalize();

            // 水平化されたベクトルを使って移動方向を計算
            Vector3 moveDirection = (camForward * movementY + camRight * movementX).normalized;

            // Rigidbodyで移動させる（AddForceよりもVelocityを直接変える方が操作性が良い場合が多い）
            Vector3 targetVelocity = moveDirection * speed;
            targetVelocity.y = rb.velocity.y; // Y軸の速度は現在の重力などによる速度を維持
            rb.velocity = targetVelocity;
        }
    }
}
