using UnityEngine;
using UnityEngine.UI; // Legacy UI elements (Button, InputField)
using TMPro; // TextMeshPro���g������

public class WordJudge : MonoBehaviour
{
    [Header("���肷��UI�v�f")]
    [SerializeField]
    private TMP_InputField _inputField; // ��������͂���InputField

    [SerializeField]
    private Button _submitButton; // ��������s����{�^��

    [SerializeField]
    private TextMeshProUGUI _resultText; // �u�����I�v�u���s�I�v��\������e�L�X�g

    [Header("����ݒ�")]
    [SerializeField]
    private string _correctAnswer = "password"; // �����̕�����iInspector����ύX�\�j

    /// <summary>
    /// ����������
    /// </summary>
    private void Start()
    {
        // �{�^�����N���b�N���ꂽ��AValidateInput���\�b�h���Ăяo���悤�ɐݒ�
        _submitButton.onClick.AddListener(ValidateInput);

        // InputField��Enter�������ꂽ���̏�����o�^
        _inputField.onSubmit.AddListener((text) => ValidateInput());

        // �ŏ��͌��ʃe�L�X�g���\���ɂ��Ă���
        _resultText.gameObject.SetActive(false);
    }

    /// <summary>
    /// ���͂��ꂽ������𔻒肷�郁�\�b�h
    /// </summary>
    public void ValidateInput()
    {
        // ���ʃe�L�X�g��\����Ԃɂ���
        _resultText.gameObject.SetActive(true);

        // InputField�̃e�L�X�g�Ɛ����̕�������r
        if (_inputField.text == _correctAnswer)
        {
            // ���������ꍇ
            _resultText.text = "Success!!";
            _resultText.color = Color.green; // �����F��΂�
            Debug.Log("�����I");
        }
        else
        {
            // ���s�����ꍇ
            _resultText.text = "Faled...";
            _resultText.color = Color.red; // �����F��Ԃ�
            Debug.Log("���s�I");
        }
        _inputField.gameObject.SetActive(false);
        _submitButton.gameObject.SetActive(false);
    }
}