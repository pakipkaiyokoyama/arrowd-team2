using UnityEngine;

public class GrowOnX : MonoBehaviour
{
    public float duration = 2f;   // 完整生长需要的时间（秒）

    private Vector3 targetScale;
    private float t = 0f;

    void Start()
    {
        // 记录模型原本的缩放比例
        targetScale = transform.localScale;

        // 一开始 X 轴长度为0（从起点开始长）
        transform.localScale = new Vector3(0f, targetScale.y, targetScale.z);
    }

    void Update()
    {
        if (t < 1f)
        {
            t += Time.deltaTime / duration;
            float k = Mathf.Clamp01(t);

            // X 轴从 0 插值到原始 scale.x
            float x = Mathf.Lerp(0f, targetScale.x, k);
            transform.localScale = new Vector3(x, targetScale.y, targetScale.z);
        }
    }
}
