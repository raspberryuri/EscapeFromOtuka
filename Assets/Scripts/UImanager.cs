using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro���g�p���邽�߂ɕK�v

public class UIManager : MonoBehaviour
{
    // Inspector����UI�e�L�X�g���A�^�b�`���邽�߂̕ϐ�
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI TimerText;
    [SerializeField]
    private Image TimerImage; // �^�C�}�[�p��UI�C���[�W

    [SerializeField]
    private float maxTimeSec = 60; // �ő厞�ԁi�b�j
    private float timer; // �^�C�}�[�̕ϐ�s

    // ���݂̃X�R�A��ێ�����ϐ�
    private int score = 0;
    

    void Start()
    {
        // �Q�[���J�n���ɃX�R�A�\����������
        UpdateScoreText();
    }

    void Update()
    {
        // �^�C�}�[�̍X�V�i�K�v�ɉ����Ď����j
        // �����ł͒P���Ɏ��Ԃ��J�E���g�_�E������������
        if(timer < maxTimeSec)
        {
            timer += Time.deltaTime;
            TimerImage.fillAmount = 1 - (timer / maxTimeSec); // �^�C�}�[��UI�C���[�W���X�V
            TimerText.enabled = false; // �^�C�}�[�̃e�L�X�g���\���ɂ���
        }
        else
        {
            // �^�C���A�b�v���̏����������ɒǉ�
            TimerImage.fillAmount = 0; // �^�C�}�[��UI�C���[�W��0�ɐݒ�
            Debug.Log("Time's up!");
            TimerText.enabled = true; // �^�C�}�[�̃e�L�X�g���\���ɂ���

        }
    }

    // �X�R�A�����Z���郁�\�b�h (���̃X�N���v�g����Ăяo����悤��public�ɂ���)
    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    // UI�e�L�X�g�̕\�����X�V���郁�\�b�h
    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }
}