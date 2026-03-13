using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MinimapMilfairy : MonoBehaviour
{
    [Header("References")]

    [Header("Icon Settings")]
    [SerializeField] private Sprite selfIcon;
    [SerializeField] private Sprite allyIcon;

    [Header("Environment Objects")]
    [SerializeField] private List<EnvironmentMarker> staticMarkers = new List<EnvironmentMarker>();

    [System.Serializable]
    public class EnvironmentMarker
    {
        
        public Transform targetTransform;
        public Sprite icon;
    }

    private void Start()
    {
        
       
    }
}