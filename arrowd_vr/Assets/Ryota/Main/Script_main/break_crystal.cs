using UnityEngine;

public class CrystalDestroyOnHit : MonoBehaviour
{
    [Header("破壊時に再生するエフェクト")]
    public GameObject breakEffectPrefab;

    [Header("破壊時に再生する効果音")]
    public AudioClip breakSound;

    [Header("HP回復量")]
    public int healAmount = 3;

    [Header("矢のタグ設定")]
    public string arrowTag = "Arrow";

    [Header("Carのタグ設定")]
    public string carTag = "Car";

    [Header("効果音の音量(0.0 ～ 1.0)")]
    [Range(0f, 1f)] public float soundVolume = 1f;

    private bool isDestroyed = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (isDestroyed) return;

        // 矢タグ以外無視
        if (!collision.gameObject.CompareTag(arrowTag))
            return;

        isDestroyed = true;

        // Crystalの位置を保存
        Vector3 pos = transform.position;

        // ★① クリスタルを即削除
        Destroy(gameObject);

        // ★② エフェクト再生（本体削除済でもOK）
        if (breakEffectPrefab != null)
        {
            Instantiate(breakEffectPrefab, pos, Quaternion.identity);
        }

        // ★③ 効果音再生
        if (breakSound != null)
        {
            AudioSource.PlayClipAtPoint(breakSound, pos, soundVolume);
        }

        // ★④ HP回復
        GameObject car = GameObject.FindWithTag(carTag);
        if (car != null)
        {
            var health = car.GetComponent<CarHealth>();
            if (health != null)
            {
                health.Heal(healAmount);
                Debug.Log($"クリスタル取得！HP +{healAmount}");
            }
        }
    }
}
