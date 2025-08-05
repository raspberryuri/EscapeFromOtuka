using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MainGame
{
    /// <summary>
    /// プレイヤー制御クラス（体の左右回転 + カメラの上下回転を分離）
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 10f;
        [SerializeField] private float _lookSpeed = 50f;

        [SerializeField] private Transform _playerBody;   // プレイヤー体（左右回転用）
        [SerializeField] private Transform _playerCamera; // カメラ（上下回転用）

        private PlayerInput _playerInput;

        private const int MaxRayDistance = 2;

        private Vector2 _currentMoveInputValue = Vector2.zero;
        private Vector2 _currentLookInputValue = Vector2.zero;

        private float _xRotation = 0f;  // カメラの上下回転角度

        private const string ACTION_MOVE = "Move";
        private const string ACTION_LOOK = "Look";
        private const string ACTION_FIRE = "Fire";

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;

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
            // 左右回転はプレイヤー体（ヨー回転）
            _playerBody.Rotate(Vector3.up * _currentLookInputValue.x * _lookSpeed * Time.deltaTime);

            // 上下回転はカメラ（ピッチ回転）
            _xRotation -= _currentLookInputValue.y * _lookSpeed * Time.deltaTime;
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
