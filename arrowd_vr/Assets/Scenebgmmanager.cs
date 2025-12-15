using UnityEngine;
using System.Collections;

/// <summary>
/// シーンごとのBGMを管理するスクリプト
/// シーンに1つ配置するだけでBGMが自動再生されます
/// </summary>
public class SceneBGMManager : MonoBehaviour
{
    [Header("ーー BGM設定 ーー")]
    [Tooltip("このシーンで再生するBGM")]
    public AudioClip bgmClip;

    [Tooltip("BGMの音量")]
    [Range(0f, 1f)]
    public float bgmVolume = 0.5f;

    [Tooltip("ループ再生する")]
    public bool loop = true;

    [Header("ーー フェード設定 ーー")]
    [Tooltip("フェードイン時間（秒）")]
    public float fadeInTime = 1.0f;

    [Tooltip("フェードアウト時間（秒）")]
    public float fadeOutTime = 1.0f;

    [Tooltip("シーン開始時にフェードインする")]
    public bool fadeInOnStart = true;

    [Header("ーー 詳細設定 ーー")]
    [Tooltip("シーン開始時に自動再生")]
    public bool playOnStart = true;

    [Tooltip("開始遅延時間（秒）")]
    public float startDelay = 0f;

    [Tooltip("他のBGMを自動的に停止する")]
    public bool stopOtherBGM = true;

    // 内部変数
    private AudioSource audioSource;
    private static SceneBGMManager currentBGM;

    void Awake()
    {
        // AudioSourceを取得または追加
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // AudioSourceの設定
        audioSource.clip = bgmClip;
        audioSource.volume = fadeInOnStart ? 0f : bgmVolume;
        audioSource.loop = loop;
        audioSource.playOnAwake = false;

        // 他のBGMを停止
        if (stopOtherBGM && currentBGM != null && currentBGM != this)
        {
            currentBGM.StopBGM(fadeOutTime);
        }

        currentBGM = this;
    }

    void Start()
    {
        if (playOnStart && bgmClip != null)
        {
            if (startDelay > 0)
            {
                StartCoroutine(DelayedPlay());
            }
            else
            {
                PlayBGM();
            }
        }
    }

    IEnumerator DelayedPlay()
    {
        yield return new WaitForSeconds(startDelay);
        PlayBGM();
    }

    /// <summary>
    /// BGMを再生
    /// </summary>
    public void PlayBGM()
    {
        if (bgmClip == null) return;

        audioSource.Play();

        if (fadeInOnStart)
        {
            StartCoroutine(FadeIn());
        }
    }

    /// <summary>
    /// BGMを停止
    /// </summary>
    public void StopBGM(float fadeTime = 0f)
    {
        if (fadeTime > 0)
        {
            StartCoroutine(FadeOutAndStop(fadeTime));
        }
        else
        {
            audioSource.Stop();
        }
    }

    /// <summary>
    /// BGMを一時停止
    /// </summary>
    public void PauseBGM()
    {
        audioSource.Pause();
    }

    /// <summary>
    /// BGMを再開
    /// </summary>
    public void ResumeBGM()
    {
        audioSource.UnPause();
    }

    /// <summary>
    /// BGMの音量を設定
    /// </summary>
    public void SetVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        audioSource.volume = bgmVolume;
    }

    /// <summary>
    /// BGMを変更
    /// </summary>
    public void ChangeBGM(AudioClip newClip, float crossFadeTime = 1.0f)
    {
        if (newClip == null) return;
        StartCoroutine(CrossFade(newClip, crossFadeTime));
    }

    // ========== フェード処理 ==========

    IEnumerator FadeIn()
    {
        float elapsed = 0f;

        while (elapsed < fadeInTime)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, bgmVolume, elapsed / fadeInTime);
            yield return null;
        }

        audioSource.volume = bgmVolume;
    }

    IEnumerator FadeOutAndStop(float fadeTime)
    {
        float startVolume = audioSource.volume;
        float elapsed = 0f;

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeTime);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
    }

    IEnumerator CrossFade(AudioClip newClip, float crossFadeTime)
    {
        // 現在のBGMをフェードアウト
        float startVolume = audioSource.volume;
        float elapsed = 0f;

        while (elapsed < crossFadeTime / 2f)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / (crossFadeTime / 2f));
            yield return null;
        }

        // BGMを切り替え
        audioSource.Stop();
        audioSource.clip = newClip;
        bgmClip = newClip;
        audioSource.Play();

        // 新しいBGMをフェードイン
        elapsed = 0f;
        while (elapsed < crossFadeTime / 2f)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, bgmVolume, elapsed / (crossFadeTime / 2f));
            yield return null;
        }

        audioSource.volume = bgmVolume;
    }

    // ========== 外部から呼び出せる静的メソッド ==========

    /// <summary>
    /// 現在のBGMマネージャーを取得
    /// </summary>
    public static SceneBGMManager GetCurrent()
    {
        return currentBGM;
    }

    /// <summary>
    /// 現在のBGMを停止（静的メソッド）
    /// </summary>
    public static void StopCurrentBGM(float fadeTime = 1.0f)
    {
        if (currentBGM != null)
        {
            currentBGM.StopBGM(fadeTime);
        }
    }

    /// <summary>
    /// 現在のBGMの音量を設定（静的メソッド）
    /// </summary>
    public static void SetCurrentVolume(float volume)
    {
        if (currentBGM != null)
        {
            currentBGM.SetVolume(volume);
        }
    }
}