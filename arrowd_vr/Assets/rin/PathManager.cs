using System.Collections;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    [Header("道路 / 馬車コントロール")]
    public RoadLookAtTarget road;
    public CarriageOnRoad carriage;

    [Header("ルートノード位置")]
    public Transform startPoint;
    public Transform pointA;
    public Transform pointB;
    public Transform pointC;

    [Header("各ノードに対応する ArrowHitController")]
    public ArrowHitController nodeA;   // SphereA 上的 ArrowHitController
    public ArrowHitController nodeB;   // SphereB 上的 ArrowHitController
    public ArrowHitController nodeC;   // SphereC 上的 ArrowHitController

    int currentStep = 0;   // 0 = Start→A, 1 = A→B, 2 = B→C, 3 = 结束

    void Start()
    {
        // 防呆检查
        if (road == null || carriage == null ||
            startPoint == null || pointA == null || pointB == null || pointC == null)
        {
            Debug.LogError("PathManager: 道路／馬車／ノード位置に未設定の参照があります！");
            enabled = false;
            return;
        }

        if (nodeA == null || nodeB == null || nodeC == null)
        {
            Debug.LogError("PathManager: nodeA / nodeB / nodeC Inspector に設定されていません！");
            enabled = false;
            return;
        }

        // 初始化 A/B/C 三个节点
        nodeA.isPathNode = true; nodeA.pathIndex = 0; nodeA.pathManager = this;
        nodeB.isPathNode = true; nodeB.pathIndex = 1; nodeB.pathManager = this;
        nodeC.isPathNode = true; nodeC.pathIndex = 2; nodeC.pathManager = this;

        // 一开始只激活 A
        nodeA.SetPathActive(true);
        nodeB.SetPathActive(false);
        nodeC.SetPathActive(false);
    }

    /// <summary>
    /// 被某个路径节点命中时，从该节点的脚本回调到这里
    /// </summary>
    public void OnPathNodeHit(ArrowHitController node)
    {
        Debug.Log($"[PathManager] OnPathNodeHit step={currentStep}, node={node.name}");

        if (currentStep >= 3) return;   // 已经结束了

        // 1. 确认是当前应该命中的节点
        if (currentStep == 0 && node != nodeA) return;
        if (currentStep == 1 && node != nodeB) return;
        if (currentStep == 2 && node != nodeC) return;

        // 2. 根据步骤确定起点和终点
        Transform from = null, to = null;
        if (currentStep == 0)
        {
            from = startPoint;
            to   = pointA;
        }
        else if (currentStep == 1)
        {
            from = pointA;
            to   = pointB;
        }
        else if (currentStep == 2)
        {
            from = pointB;
            to   = pointC;
        }

         // 3. 设置道路和马车的起点终点
        road.startPoint = from;
        road.target     = to;
        carriage.startPoint = from;
        carriage.target     = to;

        // 3.1 根据当前节点，设置道路长度（111 的 scale.x）
        if (road.roadModel != null && node.roadScaleX > 0f)
        {
            // 告诉 GrowOnX 这一段的目标长度
            if (road.growScript != null)
                road.growScript.SetFullScaleX(node.roadScaleX);

            // 把模型当前的 scale 也改一下
            Vector3 s = road.roadModel.localScale;
            s.x = node.roadScaleX;
            road.roadModel.localScale = s;

            Debug.Log($"[PathManager] 設定 111 scale.x = {s.x} （ノード {node.name}）");
        }

        // 4. 播放道路生长 + 马车移动
        road.AimAndResize();                       // 这里只改方向
        if (road.growScript != null)
            road.growScript.Restart();             // 从 0 → node.roadScaleX
        carriage.BeginMove();

            // 5. 等马车到达后，解锁下一个节点
            StartCoroutine(WaitAndUnlockNext());
        }

    IEnumerator WaitAndUnlockNext()
    {
        // CarriageOnRoad 里要有：public bool IsArrived => arrived;
        while (!carriage.IsArrived)
            yield return null;

        if (currentStep == 0)
        {
            nodeA.SetPathActive(false);
            nodeB.SetPathActive(true);
        }
        else if (currentStep == 1)
        {
            nodeB.SetPathActive(false);
            nodeC.SetPathActive(true);
        }
        else if (currentStep == 2)
        {
            nodeC.SetPathActive(false);
            // 到 C 结束，可以在这里写通关逻辑
        }

        currentStep++;
    }
}
