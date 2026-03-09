using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[CreateAssetMenu(fileName = "New Combo", menuName = "Sausage Cat/Object Sender")]
public class ObjectSender : ScriptableObject
{
    // ต้องทำการ Initialize List เสมอเพื่อป้องกัน NullReferenceException
    [SerializeField] private List<Object> objects = new List<Object>();
    
    public object GetObjectsList()
    {
        if(objects != null && objects.Count != 0)
        {
            Debug.Log($"{this.name} send out object list!");
            return objects;
        }   
        else
        {
            return null;
        }
    }
    
    public void SetObjectList(List<Object> newObjects,PhotonView photonView)
    {
        if (photonView == null) return;

        if(PhotonNetwork.InRoom)
        {
            if(!photonView.IsMine)
               return;
        }
        objects = newObjects;
    }
    
    public void AddObject(Object newObject,PhotonView photonView)
    {
        if (photonView == null) return;

        if(PhotonNetwork.InRoom)
        {
            if(!photonView.IsMine)
               return;
        }
        
        if (objects == null) 
            objects = new List<Object>();
            
        objects.Add(newObject);

    }
    public void RemoveObject(Object newObject,PhotonView photonView)
    {
        if (photonView == null) return;

        if(PhotonNetwork.InRoom)
        {
            if(!photonView.IsMine)
               return;
        }
        if (objects != null)
            objects.Remove(newObject);
    }
    
    public void ClearList(PhotonView photonView)
    {
        if (photonView == null) return;

        if(PhotonNetwork.InRoom)
        {
            if(!photonView.IsMine)
               return;
        }
        if (objects != null)
            objects.Clear();
    }
}
