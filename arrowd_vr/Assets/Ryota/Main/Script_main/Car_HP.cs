using UnityEngine;
using UnityEngine.Events;

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

    void Start()
    {
        currentHP = maxHP;
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
    }

    // HPを回復する（アイテムなどで使う場合）
    public void Heal(int amount)
    {
        if (isDead) return;

        currentHP += amount;
        if (currentHP > maxHP) currentHP = maxHP;

        if (showDebugLog)
            Debug.Log($"馬車が回復！ HP: {currentHP}/{maxHP}");
    }

    // HP割合を取得（UIで使う場合）
    public float GetHPRatio()
    {
        return (float)currentHP / maxHP;
    }

    // 死亡判定
    public bool IsDead()
    {
        return isDead;
    }
}