using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.InputSystem;

namespace Upft
{
    /// <summary>
    /// �v���C���[����N���X
    /// </summary>
    public class UpftPlayerController : MonoBehaviour
    {
        /// <summary>
        /// �ړ����x
        /// </summary>
        [SerializeField] private float _moveSpeed = 10f;
        
        /// <summary>
        /// �J������]���x
        /// </summary>
        [SerializeField] private float _lookSpeed = 50f;
       
        /// <summary>
        /// Player Input
        /// </summary>
        private PlayerInput _playerInput = null;

        /// <summary>
        /// ���݂̈ړ����͒l
        /// </summary>
        private Vector2 _currentMoveInputValue = Vector2.zero;

        /// <summary>
        /// ���݂̃J������]���͒l
        /// </summary>
        private Vector2 _currentLookInputValue = Vector2.zero;

        /// <summary>
        /// �O��̃J�����̌���
        /// </summary>
        private Vector3 _preRotation = Vector3.zero;

        /// <summary>
        /// Input Actions - Move
        /// </summary>
        private const string ACTION_MOVE = "Move";

        /// <summary>
        /// Input Actions - Look
        /// </summary>
        private const string ACTION_LOOK = "Look";

        /// <summary>
        /// Input Actions - Fire
        /// </summary>
        private const string ACTION_FIRE = "Fire";

        /// <summary>
        /// Device - �Q�[���p�b�h
        /// </summary>
        private const string DEVICE_GAMEPAD = "Gamepad";

        /// <summary>
        /// Start
        /// </summary>
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;

            if (TryGetComponent(out _playerInput))
            {
                _playerInput.actions[ACTION_LOOK].started += OnLook;
                _playerInput.actions[ACTION_LOOK].performed += OnLook;
                _playerInput.actions[ACTION_LOOK].canceled += OnLook;

                _playerInput.actions[ACTION_FIRE].started += _ => OnFire();
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        private void Update()
        {
            look();
        }

        private void move()
        {
            var moveForward = Quaternion.Euler(0, transform.eulerAngles.y, 0) * new Vector3(_currentMoveInputValue.x, 0, _currentMoveInputValue.y);
            transform.position += moveForward * _moveSpeed * Time.deltaTime;
        }

        private void look()
        {
            _preRotation.y += _currentLookInputValue.x * _lookSpeed * Time.deltaTime;
            _preRotation.x -= _currentLookInputValue.y * _lookSpeed * Time.deltaTime;
            _preRotation.x = Mathf.Clamp(_preRotation.x, -89, 89);
            transform.localEulerAngles = _preRotation;
        }


        /// <summary>
        /// �J������]����
        /// </summary>
        public void OnLook(InputAction.CallbackContext context)
        {
            _currentLookInputValue = context.ReadValue<Vector2>();
        }

        /// <summary>
        /// �ˌ�����
        /// </summary>
        public void OnFire()
        {
            Ray ray = new Ray(this.transform.position, this.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 100))
            {
                if (hit.collider.TryGetComponent(out TargetHit target))
                {
                    target.SwitchTo2DView(this.gameObject);
                }
            }
        }


        
    }
}