using UnityEngine;

public class TestGrowAuto : MonoBehaviour
{
    public float targetLengthX = 30f;
    public float duration = 1.5f;

    void Start()
    {
        var grow = GetComponent<GrowOnX>();
        if (grow == null)
        {
            Debug.LogError("GrowOnX 未挂在该物体上");
            return;
        }

        grow.duration = duration;
        grow.SetFullScaleX(targetLengthX);

        // 自动开始生长（进场景直接跑）
        grow.Restart();
    }
}
