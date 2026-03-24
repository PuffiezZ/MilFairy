using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundFXObject : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip, float volume)
    {
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();

        // ทำลาย Object นี้ทิ้งหลังจากเล่นจบ (ความยาวคลิป)
        Destroy(gameObject, clip.length);
    }
}