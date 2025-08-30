using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

namespace MainGame
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 10f;
        [SerializeField] private float _baseLookSpeed = 50f; // Š´“x‚È‚µ‚ÌŠî–{‘¬“x

        [SerializeField] private float _gravity = -19.62f;
        [SerializeField] private Transform _playerBody;
        [SerializeField] private Transform _playerCamera;

        private CharacterController _Controller;

        private const int MaxRayDistance = 2;

        private Vector3 _playerVelocity;
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
            _Controller = GetComponent<CharacterController>();

            

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

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
            bool isGrounded = _Controller.isGrounded;
            if (isGrounded && _playerVelocity.y < 0)
            {
                _playerVelocity.y = -2f;
            }

            Vector3 moveDir = new Vector3(_currentMoveInputValue.x, 0, _currentMoveInputValue.y);
            Vector3 horizontalVelocity = (_playerBody.rotation * moveDir) * _moveSpeed;

            _playerVelocity.y += _gravity * Time.deltaTime;

            Vector3 finalVelocity = horizontalVelocity + new Vector3(0, _playerVelocity.y, 0);
            _Controller.Move(finalVelocity * Time.deltaTime);

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
