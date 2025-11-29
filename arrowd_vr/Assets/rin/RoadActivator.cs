using UnityEngine;

public class RoadLookAtTarget : MonoBehaviour
{
    [Header("起点 ＝ RoadRoot の位置")]
    public Transform startPoint;

    [Header("目標 ＝ 球体")]
    public Transform target;

    [Header("道路モデル（111，RoadRoot 的子）")]
    public Transform roadModel;

    [Header("成長スクリプト（在 111 に付いているもの，任意）")]
    public GrowOnX growScript;

    [Header("シーンに入ったとき自動実行")]
    public bool playOnStart = true;


    /// <summary>
    /// 只对准起点→目标的方向，不再自动调长度
    /// </summary>
    public void AimAndResize()
    {
        if (startPoint == null || target == null)
            return;

        // 1. RoadRoot 放到起点
        transform.position = startPoint.position;

        // 2. 直接让 RoadRoot 的 forward（Z+）指向目标
        Vector3 dir = target.position - startPoint.position;
        if (dir.sqrMagnitude < 0.0001f) return;

        transform.rotation = Quaternion.LookRotation(dir, Vector3.up);

        // ❌ 不再改 roadModel.localScale，长度你自己在 Inspector 里调
    }
}
