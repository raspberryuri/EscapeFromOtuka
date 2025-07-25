using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



namespace Upft
{
    /// <summary>
    /// 3D�r���[����2D�r���[�֑J�ڂ��AUI��\������N���X
    /// </summary>
    public class TargetHit : MonoBehaviour
    {
        [Header("UI�ݒ�")]
        public GameObject UipanelObject; // Inspector����ݒ肷��
        
        private Camera mainCamera;
        
        void Start()
        {
            // ���C���J�������擾
            mainCamera = Camera.main;
            

            // �N���b�N�Ώۂ̃I�u�W�F�N�g�̐F��ݒ�
            var renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.green;
                
            }
        }

        // �I�u�W�F�N�g���N���b�N���ꂽ���ɌĂ΂��
        
        /// <summary>
        /// 2D�r���[�ɐ؂�ւ��鏈��
        /// </summary>
        public void SwitchTo2DView(GameObject playerObject)
        {
            // 1. �J������2D�iOrthographic�j�ɐ؂�ւ���
            if (mainCamera != null)
            {
                mainCamera.orthographic = true;
            }

            // 2. �ݒ肳�ꂽInputField���A�N�e�B�u�i�\���j�ɂ���
            if (UipanelObject != null)
            {
                UipanelObject.SetActive(true);
            }
            UpftPlayerController playerController = playerObject.GetComponent<UpftPlayerController>();
            if (playerController != null)
            {
                //  GameObject�S�̂ł͂Ȃ��A�X�N���v�g�����𖳌��ɂ���
                playerController.enabled = false;
            }

            //  UI����̂��߂Ƀ}�E�X�J�[�\���̃��b�N����������
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}

