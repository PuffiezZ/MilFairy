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
    [SerializeField] private GameObject camera;

    private void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            // ดึงชื่อจากเจ้าของ photonView นี้มาแสดง
            nameTextTMP.text = photonView.Owner.NickName;
        }
        else
        {
            nameTextTMP.text = "Player (Offline)";
        }
    }
    public void IsLocalPlayer()
    {
        playerMovement.enabled = true;
        locomotion.enabled = true;
        camera.SetActive(true);

        if (photonView.IsMine)
        {
            nameTextTMP.gameObject.SetActive(false);
        }
    }
}
