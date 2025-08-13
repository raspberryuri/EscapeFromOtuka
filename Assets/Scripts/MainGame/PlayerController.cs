using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MainGame
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 10f;
        [SerializeField] private float _baseLookSpeed = 50f; // 感度なしの基本速度

        [SerializeField] private Transform _playerBody;
        [SerializeField] private Transform _playerCamera;

        private PlayerInput _playerInput;

        private const int MaxRayDistance = 2;

        private Vector2 _currentMoveInputValue = Vector2.zero;
        private Vector2 _currentLookInputValue = Vector2.zero;

        private float _xRotation = 0f;

        private const string ACTION_MOVE = "Move";
        private const string ACTION_LOOK = "Look";
        private const string ACTION_FIRE = "Fire";

        public float mouseSensitivity = 1.0f;

        private GameUIManager uiManager;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;

            uiManager = FindObjectOfType<GameUIManager>();
            if (uiManager != null)
            {
                // UIから現在の感度を受け取る
                mouseSensitivity = uiManager.GetCurrentMouseSensitivity();

                // イベント登録：UIのスライダー変更時に感度を更新する
                uiManager.OnMouseSensitivityChangedEvent += OnSensitivityChanged;
            }

            if (TryGetComponent(out _playerInput))
            {
                _playerInput.actions[ACTION_LOOK].started += OnLook;
                _playerInput.actions[ACTION_LOOK].performed += OnLook;
                _playerInput.actions[ACTION_LOOK].canceled += OnLook;

                _playerInput.actions[ACTION_MOVE].performed += OnMove;
                _playerInput.actions[ACTION_MOVE].canceled += OnMove;

                _playerInput.actions[ACTION_FIRE].started += _ => OnFire();
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
            Move();
            Look();

            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, MaxRayDistance))
            {
                GameUIManager.Isaimactive = hit.collider.TryGetComponent(out TargetHit target);
            }
            else
            {
                GameUIManager.Isaimactive = false;
            }
        }

        private void Move()
        {
            Vector3 moveDir = new Vector3(_currentMoveInputValue.x, 0, _currentMoveInputValue.y);
            Vector3 move = _playerBody.rotation * moveDir;
            transform.position += move * _moveSpeed * Time.deltaTime;
        }

        private void Look()
        {
            _playerBody.Rotate(Vector3.up * _currentLookInputValue.x * _baseLookSpeed * mouseSensitivity * Time.deltaTime);

            _xRotation -= _currentLookInputValue.y * _baseLookSpeed * mouseSensitivity * Time.deltaTime;
            _xRotation = Mathf.Clamp(_xRotation, -89f, 89f);

            _playerCamera.localEulerAngles = new Vector3(_xRotation, 0f, 0f);
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            _currentLookInputValue = context.ReadValue<Vector2>();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _currentMoveInputValue = context.ReadValue<Vector2>();
        }

        public void OnFire()
        {
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, MaxRayDistance))
            {
                if (hit.collider.TryGetComponent(out TargetHit target))
                {
                    target.TriggerHit();
                }
            }
        }
    }
}
