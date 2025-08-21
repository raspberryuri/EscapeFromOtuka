using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditManager : MonoBehaviour
{

    [SerializeField] private GameObject creditPanel;

    private bool isCreditOpen = false;

    void Start()
    {

        creditPanel.SetActive(false);
    }



    void Update()
    {
        if (isCreditOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseCredit();
        }
    }
    public void OpenCredit()
    {
        creditPanel.SetActive(true);
        isCreditOpen = true;
    }

    public void CloseCredit()
    {
        creditPanel.SetActive(false);
        isCreditOpen = false;
    }
}
