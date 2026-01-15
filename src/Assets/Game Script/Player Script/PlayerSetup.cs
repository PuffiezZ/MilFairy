using Photon.Pun;
using Photon.Realtime;
using Sausagecat.PlayerControlSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerSetup : MonoBehaviourPun
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerLocomotion locomotion;
    [SerializeField] private TMP_Text nameTextTMP;
    [SerializeField] private GameObject cameraPlayer;

    private void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            // ดึงชื่อจากเจ้าของ photonView นี้มาแสดง
            nameTextTMP.text = photonView.Owner.NickName;
        }
        else
        {
            nameTextTMP.gameObject.SetActive(false);
        }
    }
    public void IsLocalPlayer()
    {
        playerMovement.enabled = true;
        locomotion.enabled = true;
        cameraPlayer.SetActive(true);

        if (photonView.IsMine)
        {
            nameTextTMP.gameObject.SetActive(false);
        }
       HideCursorOnSpawn();
    }

    private void HideCursorOnSpawn()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
