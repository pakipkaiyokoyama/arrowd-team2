using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class CarHealth : MonoBehaviour
{
    [Header("HP設定")]
    public int maxHP = 100;
    public int currentHP;

    [Header("イベント")]
    public UnityEvent onDamaged;
    public UnityEvent onDeath;

    [Header("デバッグ")]
    public bool showDebugLog = true;

    private bool isDead = false;

    // 自動ダメージ
    public float damageInterval = 120f;
    public int autoDamageAmount = 5;
    private bool isAutoDamageActive = true;

    private BGMManager bgm;

    void Start()
    {
        currentHP = maxHP;

        // BGMマネージャー取得
        bgm = FindFirstObjectByType<BGMManager>();

        // 自動ダメージ開始
        StartCoroutine(AutoDamageRoutine());
    }

    private IEnumerator AutoDamageRoutine()
    {
        while (!isDead && isAutoDamageActive)
        {
            yield return new WaitForSeconds(damageInterval);

            TakeDamage(autoDamageAmount);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHP -= amount;

        if (currentHP <= 0)
        {
            currentHP = 0;
            isDead = true;

            onDeath?.Invoke();

            if (showDebugLog)
                Debug.Log("馬車が破壊された！");

            return;
        }

        onDamaged?.Invoke();

        if (showDebugLog)
            Debug.Log($"ダメージ！HP {currentHP}/{maxHP}");

        // -------------------------
        // ★ 低HP BGM 切替
        // -------------------------
        if (bgm != null)
            bgm.UpdateHP(currentHP);
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHP += amount;
        if (currentHP > maxHP) currentHP = maxHP;

        if (showDebugLog)
            Debug.Log($"回復 HP: {currentHP}/{maxHP}");

        if (bgm != null)
            bgm.UpdateHP(currentHP);
    }

    public void StopAutoDamage()
    {
        isAutoDamageActive = false;
    }
}
