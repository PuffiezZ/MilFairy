using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void ShowWorldInterectUI();
    void HideWorldInterectUI();
    void OnBeginIntereact(GameObject player, bool getBoolean = false);
}
