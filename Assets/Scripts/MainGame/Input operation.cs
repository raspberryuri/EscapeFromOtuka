using UnityEngine;
using UnityEngine.InputSystem;

namespace MainGame
{
    [RequireComponent(typeof(Rigidbody))]
    public class Inputoperation : MonoBehaviour
    {
        [SerializeField] private float speed = 0f;
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
            return;
            if (isSensitivityUIActive) return; // UI表示中は視点操作無効化も可

            Vector2 lookInput = context.ReadValue<Vector2>();
            rotationX += lookInput.x * mouseSensitivity;
            rotationY += lookInput.y * mouseSensitivity;
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

            Vector3 movement = new Vector3(movementX, 0.0f, movementY);

            rb.AddForce(cameraTS.forward * movementY * speed);
            rb.AddForce(cameraTS.right * movementX * speed);
        }
    }
}
