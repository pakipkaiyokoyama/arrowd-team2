using UnityEngine;

public class BGMManager : MonoBehaviour
{
    [Header("通常BGM")]
    public AudioClip normalBGM;
    public float normalLoopStart = 0f;
    public float normalLoopEnd = 10f;

    [Header("HP30以下BGM")]
    public AudioClip lowHPBGM;
    public float lowLoopStart = 0f;
    public float lowLoopEnd = 10f;

    private AudioSource audioSource;
    private bool isLowHP = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayNormalBGM();
    }

    void Update()
    {
        if (!audioSource.isPlaying) return;

        // 現在の状態によってループ処理を切替
        if (!isLowHP)
        {
            if (audioSource.time >= normalLoopEnd)
                audioSource.time = normalLoopStart;
        }
        else
        {
            if (audioSource.time >= lowLoopEnd)
                audioSource.time = lowLoopStart;
        }
    }

    public void PlayNormalBGM()
    {
        if (normalBGM == null) return;

        isLowHP = false;
        audioSource.clip = normalBGM;
        audioSource.time = normalLoopStart;
        audioSource.Play();
    }

    public void PlayLowHPBGM()
    {
        if (lowHPBGM == null) return;

        if (isLowHP) return; // 連続呼び出し防止

        isLowHP = true;
        audioSource.clip = lowHPBGM;
        audioSource.time = lowLoopStart;
        audioSource.Play();
    }
}
