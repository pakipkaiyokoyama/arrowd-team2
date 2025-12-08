using UnityEngine;
using System.Collections;

public class DragonSpawner : MonoBehaviour
{
    [Header("スポーン設定")]
    public GameObject dragonPrefab;
    public Transform[] spawnPoints;

    [Header("スポーン間隔")]
    public float minSpawnInterval = 5f;
    public float maxSpawnInterval = 10f;

    [Header("最大同時出現数")]
    public int maxDragons = 3;

    [Header("スポーン開始までの待機時間")]
    public float initialDelay = 3f;

    private int currentDragonCount = 0;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        // 最初の待機
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            // ランダムな間隔で待つ
            float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);

            // 現在のドラゴン数をカウント
            currentDragonCount = FindObjectsOfType<Dragon>().Length;

            // 最大数未満ならスポーン
            if (currentDragonCount < maxDragons)
            {
                SpawnDragon();
            }
        }
    }

    void SpawnDragon()
    {
        if (dragonPrefab == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("DragonSpawner: PrefabまたはSpawnPointが設定されていません");
            return;
        }

        // ランダムなスポーン位置を選ぶ
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        // ドラゴンを生成
        GameObject dragon = Instantiate(dragonPrefab, spawnPoint.position, spawnPoint.rotation);
        Debug.Log($"ドラゴンをスポーン！ 位置: {spawnPoint.name}");
    }
}