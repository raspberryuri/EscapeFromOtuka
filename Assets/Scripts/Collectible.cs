using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    // GameManager���L���b�V�����邽�߂̕ϐ�
    private UIManager uiManager;

    void Start()
    {
        // �V�[��������UIManager�R���|�[�l���g�����I�u�W�F�N�g��T���Ď擾
        // �p�t�H�[�}���X�̂��߁A����̏Փˎ��ł͂Ȃ��ŏ��Ɉ�x�����擾���܂�
        uiManager = FindObjectOfType<UIManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // �ڐG�����I�u�W�F�N�g�̃^�O�� "Player" ���ǂ������m�F
        if (other.CompareTag("Player"))
        {
            // UIManager���������Ă���΁A�J�E���^�[�𑝂₷���\�b�h���Ăяo��
            if (uiManager != null)
            {
                uiManager.AddScore(1);
            }

            // ���̃I�u�W�F�N�g���\���i��A�N�e�B�u�j�ɂ���
            gameObject.SetActive(false);
        }
    }
}