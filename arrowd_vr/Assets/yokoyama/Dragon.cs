using UnityEngine;
using Valve.VR.InteractionSystem;

public class Dragon : MonoBehaviour
{
    [Header("ステータス")]
    public int maxHP = 3;
    public int currentHP;
    public int attackDamage = 1;

    [Header("移動設定")]
    public float moveSpeed = 5f;
    public float attackRange = 3f;
    public float attackInterval = 1f;

    [Header("エフェクト")]
    public ParticleSystem deathEffect;

    [Header("タグ設定")]
    public string carTag = "Car";
    public string arrowTag = "Arrow";

    private Transform target;
    private CarHealth carHealth;
    private float attackTimer;
    private bool isDead = false;

    void Start()
    {
        currentHP = maxHP;

        // Carを探す
        GameObject car = GameObject.FindWithTag(carTag);
        if (car != null)
        {
            target = car.transform;
            carHealth = car.GetComponent<CarHealth>();
        }
    }

    void Update()
    {
        if (isDead || target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > attackRange)
        {
            // Carに向かって移動
            MoveTowardsTarget();
        }
        else
        {
            // 攻撃範囲内なら攻撃
            AttackCar();
        }
    }

    void MoveTowardsTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0; // 水平移動のみ

        transform.position += direction * moveSpeed * Time.deltaTime;

        // Carの方を向く
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void AttackCar()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackInterval)
        {
            attackTimer = 0f;

            if (carHealth != null)
            {
                carHealth.TakeDamage(attackDamage);
                Debug.Log($"ドラゴンが攻撃！ {attackDamage}ダメージ");
            }
        }
    }

    // 矢が当たった時
    void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        bool isArrow = other.CompareTag(arrowTag) ||
                       other.GetComponent<Arrow>() != null ||
                       other.GetComponentInParent<Arrow>() != null;

        if (isArrow)
        {
            TakeDamage(1);
        }
    }

    // 衝突判定（Collider が Is Trigger じゃない場合）
    void OnCollisionEnter(Collision collision)
    {
        if (isDead) return;

        bool isArrow = collision.gameObject.CompareTag(arrowTag) ||
                       collision.gameObject.GetComponent<Arrow>() != null ||
                       collision.gameObject.GetComponentInParent<Arrow>() != null;

        if (isArrow)
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        Debug.Log($"ドラゴンがダメージ！ 残りHP: {currentHP}/{maxHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("ドラゴンを倒した！");

        // 死亡エフェクト
        if (deathEffect != null)
        {
            ParticleSystem fx = Instantiate(deathEffect, transform.position, Quaternion.identity);
            fx.Play();
            Destroy(fx.gameObject, fx.main.duration + 0.5f);
        }

        Destroy(gameObject);
    }
}