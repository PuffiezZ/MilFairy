using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviourPunCallbacks
{
    [Header("Require Pages")]
    [SerializeField] private RectTransform rt_MenuPage;
    [SerializeField] private RectTransform rt_HostPage;
    [SerializeField] private RectTransform rt_LobbyPage;
    [SerializeField] private RectTransform rt_ErrorPage;
    [SerializeField] private RectTransform rt_RoomAvaliablePage;
    [Header("Player Input Name TMP")]
    [SerializeField] private TMP_InputField nameInput_TMP;

    public static MainMenu MainMenusingleton { get; private set; }
    private void Awake()
    {
        MainMenusingleton = this;
    }
    void Start()
    {
        MainMenuOnStart();
    }

    private void MainMenuOnStart()
    {
        rt_ErrorPage.gameObject.SetActive(false);
        rt_HostPage.gameObject.SetActive(false);
        rt_LobbyPage.gameObject.SetActive(false);
        rt_RoomAvaliablePage.gameObject.SetActive(false);

        rt_MenuPage.gameObject.SetActive(true);
    }

    public void OnClickConnect()
    {
        RegisterName();
    }   

    private void RegisterName()
    {
        string name = nameInput_TMP.text;
        // 1. ตรวจสอบว่าช่องใส่ชื่อไม่ว่าง
        if (string.IsNullOrEmpty(name))
        {
            PhotonNetwork.NickName = "SausageCat";
            return;
        }
        else
        {
            PhotonNetwork.NickName = name;
        }

    }
}
