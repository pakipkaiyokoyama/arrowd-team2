using UnityEngine;

public class GrowOnX : MonoBehaviour
{
    [Header("0％ から 100％ まで伸びる所要時間（秒）")]
    public float duration = 1.0f;

    [Header("逆方向に成長するか（−X 方向）")]
    public bool reverse = false;

    Vector3 fullScale;   // 当前应该生长到的目标长度（由外部随时可修改）
    float t = 0f;
    bool playing = false;

    void Awake()
    {
        // 记录初始 scale（例如 26.3），但不锁死
        fullScale = transform.localScale;

        // 进入游戏时先变成 X=0
        transform.localScale = new Vector3(0f, fullScale.y, fullScale.z);
    }

    /// <summary>
    /// 外部（比如 PathManager）可以随时设置新的长度
    /// </summary>
    public void SetFullScaleX(float x)
    {
        fullScale = new Vector3(x, fullScale.y, fullScale.z);
    }

    /// <summary>从 0% 重新开始播放</summary>
    public void Restart()
    {
        t = 0f;
        playing = true;

        transform.localScale = new Vector3(0f, fullScale.y, fullScale.z);
    }

    void Update()
    {
        if (!playing) return;

        t += Time.deltaTime / duration;
        float k = Mathf.Clamp01(t); // 0 → 1

        float x = Mathf.Lerp(0f, fullScale.x, k);
        if (reverse) x = -x;

        transform.localScale = new Vector3(x, fullScale.y, fullScale.z);

        if (k >= 1f)
            playing = false;
    }
}
