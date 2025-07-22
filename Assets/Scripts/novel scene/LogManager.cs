using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LogManager : MonoBehaviour
{
    [Header("UI �Q��")]
    [SerializeField] private GameObject logPanel;       // ���O�S�̂��ރp�l��
    [SerializeField] private GameObject logItemPrefab;  // �P�s���̃��O�\���p�v���n�u (Text or TMP)
    [SerializeField] private Transform content;         // Scroll View > Viewport > Content
    [SerializeField] private ScrollRect scrollRect;     // ScrollRect �R���|�[�l���g
    [SerializeField] private GameObject OpenbackLog;
    [SerializeField] private GameObject ClosebackLog;

    // ��b���������߂郊�X�g
    private List<string> logHistory = new List<string>();

    /// <summary>
    /// ��b�������������ɌĂяo���B
    /// ��ʏ�̃��O�ɂ������ǉ����A�����ɂ��c���B
    /// </summary>
    public void AddLog(string message)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            logHistory.Add(message);
        }

    }
    /// <summary>
    /// ���O�p�l�����J���{�^���Ɋ��蓖�Ă�B
    /// ��������������N���A���Ă���A�S���ĕ`�悷��B
    /// </summary>
    public void OpenLogWindow()
    {
        // 1) �p�l���\��
        logPanel.SetActive(true);



        // �\���O�ɓ��e�N���A
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // ���O��1����Text�Ƃ��Đ���
        foreach (string msg in logHistory)
        {
            GameObject item = Instantiate(logItemPrefab, content);
            item.GetComponent<Text>().text = msg;
        }

        // �X�N���[���ŉ�����
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    /// <summary>
    /// ���O�p�l�������{�^���Ɋ��蓖�Ă�B
    /// </summary>
    public void CloseLogWindow()
    {
        logPanel.SetActive(false);
    }
}

