using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void ShowWorldInterectUI();
    void HideWorldInterectUI();
    void Interact(GameObject player);
}
