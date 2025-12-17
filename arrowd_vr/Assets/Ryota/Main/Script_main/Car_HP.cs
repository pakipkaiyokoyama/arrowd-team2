using UnityEngine;
using UnityEngine.Events;
using System.Collections;


public class CarHealth : MonoBehaviour
{
    [Header("HP設定")]
    public int maxHP = 100;
    public int currentHP;

    [Header("イベント（Inspectorで設定可能）")]
    public UnityEvent onDamaged;
    public UnityEvent onDeath;

    [Header("デバッグ")]
    public bool showDebugLog = true;

    private bool isDead = false;

    // ★追加：一定時間でHPを減らす設定
    public float damageInterval = 120f; // 2分（120秒）
    public int autoDamageAmount = 5;    // 5ダメージ
    private bool isAutoDamageActive = true;

    void Start()
    {
        currentHP = maxHP;

        // ★追加：自動ダメージ開始
        StartCoroutine(AutoDamageRoutine());
    }

    // ★追加：2分ごとにHPを減らす処理
    private IEnumerator AutoDamageRoutine()
    {
        while (!isDead && isAutoDamageActive)
        {
            yield return new WaitForSeconds(damageInterval);

            if (!isDead)
            {
                TakeDamage(autoDamageAmount);
                if (showDebugLog)
                    Debug.Log($"時間経過でダメージ: {autoDamageAmount} (現在HP: {currentHP})");
            }
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHP -= amount;

        if (showDebugLog)
            Debug.Log($"馬車がダメージ！ 残りHP: {currentHP}/{maxHP}");

        onDamaged?.Invoke();

        if (currentHP <= 0)
        {
            currentHP = 0;
            isDead = true;

            if (showDebugLog)
                Debug.Log("馬車が破壊された！");

            onDeath?.Invoke();
        }
        if (currentHP <= 30)
        {
            var bgm = FindFirstObjectByType<BGMManager>();
            if (bgm != null)
            {
                bgm.PlayLowHPBGM();
            }
        }

    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHP += amount;
        if (currentHP > maxHP) currentHP = maxHP;

        if (showDebugLog)
            Debug.Log($"馬車が回復！ HP: {currentHP}/{maxHP}");
    }

    public float GetHPRatio()
    {
        return (float)currentHP / maxHP;
    }

    public bool IsDead()
    {
        return isDead;
    }

    // ★必要なら：ゴール到達で自動ダメージを止める関数
    public void StopAutoDamage()
    {
        isAutoDamageActive = false;
    }
}
