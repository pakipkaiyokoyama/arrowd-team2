using UnityEngine;
using System.Collections;
using Valve.VR.InteractionSystem;
using Valve.VR;

public class ArrowHitController : MonoBehaviour
{
    [Header("ーー リンク設定 ーー")]
    public GameObject nextBalloon;
    public string nextSceneName;

    [Header("自分自身（消す用）")]
    public GameObject ball;

    [Header("ワープ先の地点（StandPoint）")]
    public Transform warpPoint;

    [Header("プレイヤーをどれくらい前に立たせるか")]
    public float playerOffset = 2.0f;

    [Header("馬車の設定")]
    public Transform carriage;
    public float carSpeed = 5.0f;

    // ★向き補正の設定は削除しました（自動で向くため）

    [Header("ーー チーム開発用設定 ーー")]
    public bool isPathNode = false;
    public int pathIndex = -1;
    public PathManager pathManager;
    public float roadScaleX = 0f;

    [Header("ーー その他設定 ーー")]
    public KeyCode triggerKey = KeyCode.T;
    public ParticleSystem explodeFxPrefab;
    public string arrowTag = "Arrow";
    public string carTag = "Car";

    bool triggered = false;
    public bool pathActive = true;

    void Start()
    {
        if (pathActive) SetPathActive(true);

        if (carriage == null)
        {
            GameObject carObj = GameObject.FindWithTag(carTag);
            if (carObj != null) carriage = carObj.transform;
        }
    }

    public void SetPathActive(bool on)
    {
        pathActive = on;
        if (ball != null) ball.SetActive(on);
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = on;
        if (on) triggered = false;
    }

    void Update()
    {
        if (isPathNode && !pathActive) return;
        if (!triggered && Input.GetKeyDown(triggerKey)) Trigger();
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (isPathNode && !pathActive) return;

        if (other.GetComponentInParent<Arrow>() == null && !other.CompareTag(arrowTag))
            return;

        Trigger();
    }

    void Trigger()
    {
        if (triggered) return;
        triggered = true;

        Debug.Log($"[ArrowHit] {name} 命中！");

        WarpPlayerAndMoveCar();

        if (nextBalloon != null)
        {
            var nextScript = nextBalloon.GetComponent<ArrowHitController>();
            if (nextScript != null) nextScript.SetPathActive(true);
        }

        if (explodeFxPrefab != null && ball != null)
        {
            ParticleSystem fx = Instantiate(explodeFxPrefab, ball.transform.position, Quaternion.identity);
            fx.Play();
            Destroy(fx.gameObject, fx.main.duration + 0.2f);
        }

        StartCoroutine(HideAndDelayDestroy());

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SteamVR_LoadLevel.Begin(nextSceneName);
        }
    }

    void WarpPlayerAndMoveCar()
    {
        Vector3 basePos = (warpPoint != null) ? warpPoint.position : transform.position;
        Quaternion baseRot = (warpPoint != null) ? warpPoint.rotation : transform.rotation;
        Vector3 forwardDir = (warpPoint != null) ? warpPoint.forward : transform.forward;

        if (Valve.VR.InteractionSystem.Player.instance != null)
        {
            Vector3 playerPos = basePos + (forwardDir * playerOffset);
            Valve.VR.InteractionSystem.Player.instance.transform.position = playerPos;

            float playerY = baseRot.eulerAngles.y;
            Valve.VR.InteractionSystem.Player.instance.transform.rotation = Quaternion.Euler(0, playerY, 0);
        }

        if (carriage != null)
        {
            // 回転情報は渡さず、場所だけ渡す
            StartCoroutine(MoveCarriageSmoothly(basePos));
        }
    }

    // ★変更：進行方向を向いて進むロジック
    IEnumerator MoveCarriageSmoothly(Vector3 targetPos)
    {
        while (Vector3.Distance(carriage.position, targetPos) > 0.1f)
        {
            // 1. 移動する
            Vector3 nextPos = Vector3.MoveTowards(carriage.position, targetPos, carSpeed * Time.deltaTime);

            // 2. 進む方向を計算する（目的地 - 現在地）
            Vector3 direction = (targetPos - carriage.position).normalized;
            direction.y = 0; // 上下には傾かないようにする

            // 3. その方向を向く
            if (direction != Vector3.zero)
            {
                Quaternion lookRot = Quaternion.LookRotation(direction);
                carriage.rotation = Quaternion.RotateTowards(carriage.rotation, lookRot, carSpeed * 20 * Time.deltaTime);
            }

            carriage.position = nextPos;
            yield return null;
        }

        // 到着
        carriage.position = targetPos;

        // 最後に、次の目的地（StandPointの向き）に合わせて整列させるならこれを使う
        // carriage.rotation = Quaternion.Euler(0, (warpPoint != null ? warpPoint.eulerAngles.y : transform.eulerAngles.y), 0);
    }

    IEnumerator HideAndDelayDestroy()
    {
        if (ball != null)
        {
            foreach (var r in ball.GetComponentsInChildren<Renderer>()) r.enabled = false;
        }
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        yield return new WaitForSeconds(10.0f);

        if (ball != null) ball.SetActive(false);
    }
}