using UnityEngine;

public class CarriageOnRoad : MonoBehaviour
{
    [Header("道路の両端")]
    public Transform startPoint;
    public Transform target;

    [Header("道路の向き（任意）")]
    public Transform roadRoot;

    [Header("輪")]
    public Transform frontWheel;
    public Transform backWheel;
    public float wheelRadius = 0.4f;

    [Header("馬車が路面から離れる高さ")]
    public float heightOffset = 0.2f;

    [Header("起点から終点まで移動する所要時間（秒）")]
    public float moveDuration = 5f;

    [Header("到達後に傾斜角度をリセットする速度")]
    public float settleSpeed = 2f;

    float t = 0f;
    float totalLength;
    bool arrived = false;

    bool isMoving = false;

    public bool IsArrived => arrived;

    void Start()
    {
        if (startPoint == null || target == null)
        {
            Debug.LogError("CarriageOnRoad: startPoint 或 target 没填");
            enabled = false;
            return;
        }

        totalLength = Vector3.Distance(startPoint.position, target.position);
        t = 0f;
    }

    /// <summary>外部触发：开始从起点向终点移动</summary>
    public void BeginMove()
    {
        if (startPoint == null || target == null) return;

        totalLength = Vector3.Distance(startPoint.position, target.position);
        t = 0f;
        arrived = false;
        isMoving = true;
    }

    void Update()
    {
        if (!isMoving && !arrived) return;

        if (arrived)
        {
            float y = transform.eulerAngles.y;
            Quaternion flatRot = Quaternion.Euler(0f, y, 0f);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                flatRot,
                Time.deltaTime * settleSpeed
            );
            return;
        }

        float prevDist = t * totalLength;

        t += Time.deltaTime / moveDuration;
        if (t >= 1f)
        {
            t = 1f;
            arrived = true;
        }

        float curDist = t * totalLength;
        float deltaDist = curDist - prevDist;

        Vector3 posOnLine = Vector3.Lerp(startPoint.position, target.position, t);
        Vector3 up = roadRoot ? roadRoot.up : Vector3.up;
        transform.position = posOnLine + up * heightOffset;

        Quaternion rot = roadRoot
            ? roadRoot.rotation
            : Quaternion.LookRotation(target.position - startPoint.position, Vector3.up);
        transform.rotation = rot;

        if (wheelRadius > 0.0001f)
        {
            float deltaAngle = (deltaDist / (2f * Mathf.PI * wheelRadius)) * 360f;
            if (frontWheel)
                frontWheel.Rotate(Vector3.right, deltaAngle, Space.Self);
            if (backWheel)
                backWheel.Rotate(Vector3.right, deltaAngle, Space.Self);
        }
    }
}
