using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleButton : MonoBehaviour
{
    [SerializeField] GameObject configPanel;

    public void StartBtn()
    {
        SceneManager.LoadScene("Main");
    }

    public void ShowConfigPanel()
    {
        configPanel.SetActive(true);
    }

    public void HideConfigPanel()
    {
        configPanel.SetActive(false);
    }
}
