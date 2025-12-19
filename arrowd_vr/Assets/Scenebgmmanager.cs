using System.Collections;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    [Header("í èÌBGMê›íË")]
    public AudioClip normalBGM;
    public float normalLoopStart = 0f;
    public float normalLoopEnd = 10f;

    [Header("HPí·â∫BGMê›íË")]
    public AudioClip lowHPBGM;
    public float lowLoopStart = 0f;
    public float lowLoopEnd = 10f;

    [Header("HPËáíl")]
    public int lowHPThreshold = 30;

    private bool isLowHP = false;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.loop = false;

        // í èÌBGMäJén
        SwitchToNormalBGM();
    }

    void Update()
    {
        if (!audioSource.isPlaying) return;

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

    // HPéÛÇØéÊÇË
    public void UpdateHP(int currentHP)
    {
        if (!isLowHP && currentHP <= lowHPThreshold)
        {
            SwitchToLowHPBGM();
        }
        else if (isLowHP && currentHP > lowHPThreshold)
        {
            SwitchToNormalBGM();
        }
    }

    public void SwitchToNormalBGM()
    {
        if (normalBGM == null) return;
        isLowHP = false;
        audioSource.clip = normalBGM;
        audioSource.time = normalLoopStart;
        audioSource.Play();
    }

    public void SwitchToLowHPBGM()
    {
        if (lowHPBGM == null) return;
        if (isLowHP) return;
        isLowHP = true;
        audioSource.clip = lowHPBGM;
        audioSource.time = lowLoopStart;
        audioSource.Play();
    }
}
