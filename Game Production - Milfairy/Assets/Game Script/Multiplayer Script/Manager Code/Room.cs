using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Room : MonoBehaviourPunCallbacks
{
    public TMP_Text NameText_TMP;

    private int currentPlayerAmount;
    private RoomList roomListRef;
    private string roomName;

    public void OnRoomListCalled(string name,RoomList getRoomlist)
    {
        NameText_TMP.text = name;
        roomName = name;
        roomListRef = getRoomlist;
    }
    public void JoinRoom()
    {
        MainMenu.MainMenusingleton.OnClickConnect();
        roomListRef.JoinRoomOnListName(roomName);
    }
}
