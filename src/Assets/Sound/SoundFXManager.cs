using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SoundFXManager : MonoBehaviourPun
{
    public static SoundFXManager instance;

    [Header("Settings")]
    [SerializeField] private SoundFXObject soundFXPrefab; // ลาก Prefab ที่สร้างไว้มาใส่
    [SerializeField] private List<SoundData> soundLibrary;

    private void Awake() 
    {
        if(instance == null) 
            instance = this;
    }

    // --- LOCAL PLAY ---
    public void PlayLocalSound(string clipName, Vector3 position, float volume = 1f)
    {
        SpawnSoundObject(clipName, position, volume);
    }

    // --- GLOBAL PLAY (Photon) ---
    public void PlayGlobalSound(string clipName, Vector3 position, float volume = 1f)
    {
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC(nameof(RPC_PlayGlobalSound), RpcTarget.All, clipName, position, volume);
        }
        else
        {
            SpawnSoundObject(clipName, position, volume);
        }
    }

    [PunRPC]
    private void RPC_PlayGlobalSound(string clipName, Vector3 position, float volume)
    {
        SpawnSoundObject(clipName, position, volume);
    }

    private void SpawnSoundObject(string clipName, Vector3 position, float volume)
    {
        AudioClip clip = soundLibrary.Find(s => s.soundName == clipName)?.clip;
        if (clip != null && soundFXPrefab != null)
        {
            // สร้าง Prefab ออกมาที่ตำแหน่งนั้น
            SoundFXObject obj = Instantiate(soundFXPrefab, position, Quaternion.identity);
            obj.PlaySound(clip, volume);
        }
    }
}

[System.Serializable]
public class SoundData
{
    public string soundName;
    public AudioClip clip;
}
