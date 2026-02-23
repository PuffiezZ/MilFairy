using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void ShowWorldInterectUI();
    void HideWorldInterectUI();
    void OnBeginIntereact(GameObject player, bool getBoolean = false);
    void OnHoldInteract(GameObject player, float progress); // progress 0 to 1
    void OnCancelInteract();
}
