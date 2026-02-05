using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StateUIOverhead : MonoBehaviour
{
    public TMP_Text stateText_TMP;

    public void UpdateStateText(string newState)
    {
        if (stateText_TMP == null) return;

        stateText_TMP.text = newState;
    }
}
