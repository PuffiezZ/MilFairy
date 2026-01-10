using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageUIManager : MonoBehaviour
{
    [SerializeField] private GameObject[] onStartUI_element;
    [SerializeField] private GameObject[] onLateStartUI_element;

    public void OnResetPage()
    {
        gameObject.SetActive(true);

        if(onStartUI_element.Length > 0)
        {
            for (int i = 0; i < onStartUI_element.Length; i++)
            {
                onStartUI_element[i].SetActive(true);
            }
        }

        if(onLateStartUI_element.Length > 0)
        {
            for (int i = 0; i < onLateStartUI_element.Length; i++)
            {
                onLateStartUI_element[i].SetActive(false);
            }
        }
    }
}
