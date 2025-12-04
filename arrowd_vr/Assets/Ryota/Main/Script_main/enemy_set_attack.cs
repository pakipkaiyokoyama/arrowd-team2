using System.Collections;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class EnemyAI : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform car;
    public float detectionRange = 50f;
    public float fieldOfView = 90f;
    public float chargeSpeed = 20f;

    [Header("Respawn Settings")]
    public GameObject enemyPrefab;
    public float respawnRadius = 150f;
    public float respawnTime = 3f;
    public LayerMask groundLayer;   // 地形レイヤー（InspectorでGroundを設定すること）

    [Header("Arrow Settings")]
    public string arrowTag = "Arrow";

    private bool isCharging = false;

    void Update()
    {
        if (car == null) return;

        float distance = Vector3.Distance(transform.position, car.position);
        if (distance > detectionRange) return;

        Vector3 dirToCar = (car.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dirToCar);

        if (angle < fieldOfView / 2f)
            isCharging = true;

        if (isCharging)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                car.position,
                chargeSpeed * Time.deltaTime
            );
            transform.LookAt(car.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // --- 矢に当たった場合 ---
        if (other.CompareTag(arrowTag) || other.GetComponentInParent<Arrow>() != null)
        {
            // 矢を消す
            Destroy(other.gameObject);

            // ダメージ処理（必要なら）
            // DamageCar(2); 

            // 敵死亡処理
            KillEnemy();
            return;
        }

        // --- Carに衝突した場合 ---
        // タグ判定も追加しておくと安全です
        if (other.transform == car || other.CompareTag("Car"))
        {
            DamageCar(2);
            KillEnemy();
        }
    }

    // Carへダメージ
    private void DamageCar(int amount)
    {
        // CarHealthスクリプトがついていると仮定
        // 見つからなければエラーにならないようnullチェックを入れています
        var hp = car.GetComponent("CarHealth") as MonoBehaviour;

        if (hp != null)
        {
            // hp.TakeDamage(amount); // ←CarHealthにこのメソッドがある場合
            hp.SendMessage("TakeDamage", amount, SendMessageOptions.DontRequireReceiver);
        }
    }

    // 敵を倒して復活処理へ
    private void KillEnemy()
    {
        Vector3 spawnPos = GetValidSpawnPoint();

        // コルーチンを開始（復活待ち時間があるため）
        StartCoroutine(RespawnEnemy(spawnPos));
    }

    // ★追加：復活コルーチン
    private IEnumerator RespawnEnemy(Vector3 pos)
    {
        // 1. まず自分を見えなくする（当たり判定も消す）
        // Destroyしてしまうとコルーチンも止まるため、見た目だけ消して待機します
        GetComponent<Collider>().enabled = false;
        foreach (var r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = false;
        }

        // 2. 指定時間待つ
        yield return new WaitForSeconds(respawnTime);

        // 3. 新しい敵を生成
        if (enemyPrefab != null)
        {
            Instantiate(enemyPrefab, pos, Quaternion.identity);
        }

        // 4. 古い自分を完全に削除
        Destroy(gameObject);
    }

    // 地形に干渉しないように復活位置を取得する
    private Vector3 GetValidSpawnPoint()
    {
        Vector3 randomPos = car.position + Random.insideUnitSphere * respawnRadius;
        randomPos.y = 500f; // 空中から Raycast するための高さ

        RaycastHit hit;
        // 地面に向かってレイを飛ばす
        if (Physics.Raycast(randomPos, Vector3.down, out hit, 1000f, groundLayer))
        {
            // ★修正箇所：地面の少し上にスポーンさせる
            return hit.point + Vector3.up;
        }

        // Raycastが当たらなかった場合の保険（とりあえずCarの近く）
        return car.position + Vector3.up * 5f;
    }
}