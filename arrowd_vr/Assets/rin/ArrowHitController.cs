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

    [Header("★重要：馬車の向き補正")]
    [Tooltip("もし馬車が後ろを向くなら「180」にしてください。横なら「90」か「-90」")]
    public float modelCorrectionY = 0f; // ← ここで向きを強制修正！


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

        // A. プレイヤーワープ（指定位置へ）
        if (Valve.VR.InteractionSystem.Player.instance != null)
        {
            // StandPointの位置そのものに立つ
            Valve.VR.InteractionSystem.Player.instance.transform.position = basePos;

            float playerY = baseRot.eulerAngles.y;
            Valve.VR.InteractionSystem.Player.instance.transform.rotation = Quaternion.Euler(0, playerY, 0);
        }

        // B. 馬車移動（プレイヤーの後ろへ）
        if (carriage != null)
        {
            // プレイヤーの位置から後ろにずらした場所を目的地にする
            Vector3 carTargetPos = basePos - (forwardDir * playerOffset);

            StartCoroutine(MoveCarriageSmoothly(carTargetPos));
        }
    }

    // ★変更：常に目的地（プレイヤーの方）を向くように修正
    IEnumerator MoveCarriageSmoothly(Vector3 targetPos)
    {
        while (Vector3.Distance(carriage.position, targetPos) > 0.1f)
        {
            // 1. 移動
            Vector3 nextPos = Vector3.MoveTowards(carriage.position, targetPos, carSpeed * Time.deltaTime);

            // 2. 向きの計算：「目的地」の方を向く
            Vector3 directionToTarget = (targetPos - carriage.position).normalized;
            directionToTarget.y = 0; // 上下には傾かない

            if (directionToTarget != Vector3.zero)
            {
                // その方向を向く回転を作る
                Quaternion lookRot = Quaternion.LookRotation(directionToTarget);

                // ★ここで補正値を足す！（180度回すなど）
                lookRot *= Quaternion.Euler(0, modelCorrectionY, 0);

                // ゆっくり回す
                carriage.rotation = Quaternion.RotateTowards(carriage.rotation, lookRot, carSpeed * 20 * Time.deltaTime);
            }

            carriage.position = nextPos;
            yield return null;
        }

        carriage.position = targetPos;

        // 到着後も、次の進行方向（プレイヤーの向き）に合わせておく
        if (warpPoint != null)
        {
            Quaternion finalRot = Quaternion.Euler(0, warpPoint.eulerAngles.y + modelCorrectionY, 0);
            carriage.rotation = finalRot;
        }
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