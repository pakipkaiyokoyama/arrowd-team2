using UnityEngine;

public class SceneBGMManager : MonoBehaviour
{
    [Header("çƒê∂Ç∑ÇÈAudioSource")]
    protected AudioSource audioSource;

    [Header("ç≈èâÇ…é©ìÆçƒê∂Ç∑ÇÈÇ©")]
    public bool playOnStart = true;

    protected virtual void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (playOnStart && audioSource != null)
        {
            audioSource.Play();
        }
    }

    public void ChangeBGM(AudioClip clip, float volume = 1f)
    {
        if (audioSource == null) return;

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
    }

    public void StopBGM()
    {
        if (audioSource != null)
            audioSource.Stop();
    }
}
