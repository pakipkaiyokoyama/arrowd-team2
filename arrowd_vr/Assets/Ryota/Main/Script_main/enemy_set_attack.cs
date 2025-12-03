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
    public LayerMask groundLayer;   // 地形レイヤー

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
            Destroy(other.gameObject);
            DamageCar(2);
            KillEnemy();
            return;
        }

        // --- Carに衝突した場合 ---
        if (other.transform == car)
        {
            DamageCar(2);
            KillEnemy();
        }
    }

    // Carへダメージ
    private void DamageCar(int amount)
    {
        CarHealth hp = car.GetComponent<CarHealth>();
        if (hp != null) hp.TakeDamage(amount);
    }

    // 敵を倒して復活処理へ
    private void KillEnemy()
    {
        Vector3 spawnPos = GetValidSpawnPoint();

        StartCoroutine(RespawnEnemy(spawnPos));

        Destroy(gameObject);
    }

    // 地形に干渉しないように復活位置を取得する
    private Vector3 GetValidSpawnPoint()
    {
        Vector3 randomPos = car.position + Random.insideUnitSphere * respawnRadius;
        randomPos.y = 500f; // 空中から Raycast するための高さ

        RaycastHit hit;
        if (Physics.Raycast(randomPos, Vector3.down, out hit, 1000f, groundLayer))
        {
            return hit.point + Vect
