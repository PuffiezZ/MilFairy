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
            // �֧���ͨҡ��Ңͧ photonView ������ʴ�
            nameTextTMP.text = photonView.Owner.NickName;
        }
        else
        {
            nameTextTMP.gameObject.SetActive(false);
        }

        StartCoroutine(WaitAndSetPayload());
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

    private IEnumerator WaitAndSetPayload()
    {
        // รอจนกว่า RoomManager และ Payload จะถูกตั้งค่า (ป้องกันปัญหา Network Sync ล่าช้า)
        while (RoomManager.Instance == null || RoomManager.Instance.CurrentPlayingPayload == null)
        {
            yield return null;
        }

        playerMovement.plScript = RoomManager.Instance.CurrentPlayingPayload;
    }
    

    private void HideCursorOnSpawn()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
