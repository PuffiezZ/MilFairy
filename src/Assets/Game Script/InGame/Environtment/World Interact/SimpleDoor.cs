using UnityEngine;

public class SimpleDoor : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private string openBoolName = "isOpen";

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openClip;
    [SerializeField] private AudioClip closeClip;

    // Open the door
    public void OpenDoor()
    {
        if (doorAnimator != null)
            doorAnimator.SetBool(openBoolName, true);

        PlaySound(openClip);
        Debug.Log("Door Opening...");
    }

    // Close the door
    public void CloseDoor()
    {
        if (doorAnimator != null)
            doorAnimator.SetBool(openBoolName, false);

        PlaySound(closeClip);
        Debug.Log("Door Closing...");
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
}