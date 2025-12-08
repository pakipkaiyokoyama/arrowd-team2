using UnityEngine;
using System.Collections;
using Valve.VR.InteractionSystem;
using Valve.VR;
using UnityEngine.SceneManagement;

public class ArrowHitController : MonoBehaviour
{
    [Header("ーー リンク設定 ーー")]
    [Tooltip("次に有効化する風船（リレー用）")]
    public GameObject nextBalloon;

    [Tooltip("【NEW】ここを割ったら移動するシーン名（ゴール用）")]
    public string nextSceneName;

    [Header("自分自身（消す用）")]
    public GameObject ball;

    [Header("ワープ先の地点（StandPoint）")]
    public Transform warpPoint;

    [Header("プレイヤーをどれくらい前に立たせるか")]
    public float playerOffset = 2.0f;

    [Header("馬車の設定")]
    public Transform carriage;
    [Tooltip("馬車がついてくる速度")]
    public float carSpeed = 5.0f;
    [Tooltip("馬車をどれくらい後ろに配置するか")]
    public float carriageOffset = 5.0f;

    [Header("馬車の向き補正")]
    public float modelCorrectionY = 0f;

    [Header("ーー チーム開発用設定 ーー")]
    public bool isPathNode = false;
    public int pathIndex = -1;
    public float roadScaleX = 0f;

    [Header("ーー その他設定 ーー")]
    public KeyCode triggerKey = KeyCode.T;
    public ParticleSystem explodeFxPrefab;
    public string arrowTag = "Arrow";
    public string carTag = "Car";

    // 内部フラグ
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
            StartCoroutine(SafeSceneLoad(nextSceneName));
        }
    }

    IEnumerator SafeSceneLoad(string sceneName)
    {
        Debug.Log("シーン移動開始（非同期モード）...");

        SteamVR_Fade.Start(Color.black, 0.3f);
        yield return new WaitForSeconds(0.3f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);
        SteamVR_Fade.Start(Color.clear, 0.5f);
    }

    void WarpPlayerAndMoveCar()
    {
        Vector3 basePos = (warpPoint != null) ? warpPoint.position : transform.position;

        Vector3 moveDir = Vector3.forward;
        if (carriage != null)
        {
            moveDir = (basePos - carriage.position).normalized;
            moveDir.y = 0;
            if (moveDir == Vector3.zero) moveDir = Vector3.forward;
        }

        if (Valve.VR.InteractionSystem.Player.instance != null)
        {
            Vector3 playerPos = basePos + (moveDir * playerOffset);
            Valve.VR.InteractionSystem.Player.instance.transform.position = playerPos;

            float playerY = Quaternion.LookRotation(moveDir).eulerAngles.y;
            Valve.VR.InteractionSystem.Player.instance.transform.rotation = Quaternion.Euler(0, playerY, 0);
        }

        if (carriage != null)
        {
            Vector3 carTargetPos = basePos - (moveDir * carriageOffset);
            StartCoroutine(MoveCarriageSmoothly(carTargetPos, moveDir));
        }
    }

    IEnumerator MoveCarriageSmoothly(Vector3 targetPos, Vector3 finalForward)
    {
        while (Vector3.Distance(carriage.position, targetPos) > 0.1f)
        {
            Vector3 nextPos = Vector3.MoveTowards(carriage.position, targetPos, carSpeed * Time.deltaTime);

            Vector3 directionToTarget = (targetPos - carriage.position).normalized;
            directionToTarget.y = 0;

            if (directionToTarget != Vector3.zero)
            {
                Quaternion lookRot = Quaternion.LookRotation(directionToTarget);
                lookRot *= Quaternion.Euler(0, modelCorrectionY, 0);
                carriage.rotation = Quaternion.RotateTowards(carriage.rotation, lookRot, carSpeed * 20 * Time.deltaTime);
            }

            carriage.position = nextPos;
            yield return null;
        }
        carriage.position = targetPos;

        if (finalForward != Vector3.zero)
        {
            Quaternion finalRot = Quaternion.LookRotation(finalForward) * Quaternion.Euler(0, modelCorrectionY, 0);
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