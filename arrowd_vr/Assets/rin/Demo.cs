using UnityEngine;

public class Demo : MonoBehaviour
{
    [Header("起点 / 终点（世界坐标）")]
    public Transform startPoint;
    public Transform endPoint;

    [Header("移动所需时间（秒）")]
    public float duration = 2f;

    [Header("是否锁定高度（Y）")]
    public bool lockY = true;

    private float t = 0f;

    void OnEnable()
    {
        t = 0f;

        if (startPoint != null)
        {
            // 一开始先放到起点
            transform.position = startPoint.position;
        }
    }

    void Update()
    {
        if (startPoint == null || endPoint == null || duration <= 0f) return;

        t += Time.deltaTime / duration;
        t = Mathf.Clamp01(t);   // 0 → 1

        Vector3 start = startPoint.position;
        Vector3 end   = endPoint.position;

        // 如果只想在地面上画线，就锁死 Y
        if (lockY)
        {
            end.y = start.y;
        }

        // 直线插值
        Vector3 pos = Vector3.Lerp(start, end, t);
        transform.position = pos;

        // 走到终点就停
        if (t >= 1f)
        {
            enabled = false;
        }
    }
}
