using UnityEngine;
using Valve.VR.InteractionSystem;
using Valve.VR; // ★★★ これが足りていませんでした！追加してください ★★★

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


    [Header("ーー チーム開発用設定 ーー")]
    public bool isPathNode = false;
    public int pathIndex = -1;
    public PathManager pathManager;
    public float roadScaleX = 0f;

    [Header("ーー その他設定 ーー")]
    public KeyCode triggerKey = KeyCode.T;
    public ParticleSystem explodeFxPrefab;
    public string arrowTag = "Arrow";

    // 内部フラグ
    bool triggered = false;
    public bool pathActive = true;

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

        // 1. プレイヤーワープ
        if (Valve.VR.InteractionSystem.Player.instance != null)
        {
            Vector3 targetPos = (warpPoint != null) ? warpPoint.position : transform.position;
            Quaternion targetRot = (warpPoint != null) ? warpPoint.rotation : transform.rotation;
            Vector3 forwardDir = (warpPoint != null) ? warpPoint.forward : transform.forward;
            Vector3 finalPlayerPos = targetPos + (forwardDir * playerOffset);

            Valve.VR.InteractionSystem.Player.instance.transform.position = finalPlayerPos;
            Valve.VR.InteractionSystem.Player.instance.transform.rotation = targetRot;
        }

        // 2. 次の風船を起こす
        if (nextBalloon != null)
        {
            var nextScript = nextBalloon.GetComponent<ArrowHitController>();
            if (nextScript != null) nextScript.SetPathActive(true);
        }

        // 3. 爆発エフェクト
        if (explodeFxPrefab != null && ball != null)
        {
            ParticleSystem fx = Instantiate(explodeFxPrefab, ball.transform.position, Quaternion.identity);
            fx.Play();
            var main = fx.main;
            Destroy(fx.gameObject, main.duration + 0.2f);
        }

        // 4. 自分を消す
        if (ball != null) ball.SetActive(false);

        // 5. シーン移動処理
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            Debug.Log("ゴール！シーン移動します: " + nextSceneName);

            // ★もしここでエラーが出るなら、下の「SceneManager」の方を使ってください
            SteamVR_LoadLevel.Begin(nextSceneName);
        }

        // 6. マネージャーに通知
        if (isPathNode && pathManager != null)
        {
            pathManager.OnPathNodeHit(this);
        }
    }
}