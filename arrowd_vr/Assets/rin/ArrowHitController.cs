using UnityEngine;

public class ArrowHitController : MonoBehaviour
{
    [Header("ルートノードかどうか（A／B／C など）")]
    public bool isPathNode = false;      // 路径节点：true；普通加分靶：false

    [Header("ルート順序（0＝A、1＝B、2＝C…　ルートでない場合は −1）")]
    public int pathIndex = -1;

    [Header("ルートマネージャー（ルートノードのみ必要）")]
    public PathManager pathManager;

    [Header("どのキーで命中をシミュレートするか")]
    public KeyCode triggerKey = KeyCode.T;

    [Header("射抜かれた球体（自分自身）")]
    public GameObject ball;

    [Header("爆発エフェクトのプレハブ（Prefab）")]
    public ParticleSystem explodeFxPrefab;

    [Header("本ノードの道路長さ（111 の Scale X）。≦0 の場合は変更しない")]
    public float roadScaleX = 0f;


    [Header("矢のタグ名")]
    public string arrowTag = "Arrow";

    // 内部状态
    bool triggered = false;      // 这个球是否已经被触发过
    bool pathActive = true;      // 对于路径节点：当前是否可用（轮到它）

    /// <summary>
    /// 由 PathManager 调用：使这个球在路径中“启用/禁用”
    /// </summary>
    public void SetPathActive(bool on)
    {
        pathActive = on;

        if (ball != null)
            ball.SetActive(on);

        var col = GetComponent<Collider>();
        if (col != null)
            col.enabled = on;

        // 重新启用时，可以再次触发（目前一次性用，不太会用到）
        if (on)
            triggered = false;
    }

    void Update()
    {
        // 路径节点且当前没轮到自己的时候，不响应
        if (isPathNode && !pathActive)
            return;

        // 按键 T 模拟命中
        if (!triggered && Input.GetKeyDown(triggerKey))
        {
            Trigger();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        // 路径节点且当前没轮到 → 忽略
        if (isPathNode && !pathActive)
            return;

        // 只认箭矢
        if (!other.CompareTag(arrowTag))
            return;

        Trigger();
    }

    /// <summary>
    /// 球爆裂 → （如果是路径节点）通知 PathManager
    /// </summary>
    void Trigger()
    {
        if (triggered) return;
        triggered = true;

        Debug.Log($"[ArrowHit] {name} Trigger(), isPathNode={isPathNode}, manager={(pathManager ? pathManager.name : "null")}, step=?");

        // 1. 生成一次性爆炸特效
        if (explodeFxPrefab != null && ball != null)
        {
            Vector3 pos = ball.transform.position;

            ParticleSystem fx = Instantiate(
                explodeFxPrefab,
                pos,
                Quaternion.identity
            );
            fx.Play();

            var main = fx.main;
            float life = main.duration * main.startLifetimeMultiplier;
            Destroy(fx.gameObject, life + 0.2f);
        }

        // 2. 球体消失
        if (ball != null)
            ball.SetActive(false);

        // 3. 如果是路径节点，交给 PathManager 处理道路 + 马车
        if (isPathNode && pathManager != null)
        {
            pathManager.OnPathNodeHit(this);
        }

        // 4. 如果是得分靶（以后可以在这里加 GameScore）
        // else { 加分用逻辑写这里 }
    }
}
