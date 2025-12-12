using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

public class GoalFlag : MonoBehaviour
{
    [Header("【設定】移動先のリザルトシーン名")]
    public string resultSceneName = "Result";

    [Header("【設定】ゴールした時の効果音")]
    public AudioClip goalSound;

    [Header("【設定】当たると反応するタグ")]
    public string arrowTag = "Arrow";
    public string carTag = "Car";

    private bool isTriggered = false;

    void OnCollisionEnter(Collision collision)
    {
        if (isTriggered) return;

        if (collision.gameObject.CompareTag(arrowTag) ||
            collision.gameObject.GetComponentInParent<Valve.VR.InteractionSystem.Arrow>() != null)
        {
            Debug.Log("矢が命中（衝突）！ゴール！");
            GoToResult();
        }
        else if (collision.gameObject.CompareTag(carTag))
        {
            Debug.Log("馬車が到着（衝突）！ゴール！");
            GoToResult();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) DebugGoal();
    }

    void DebugGoal()
    {
        Debug.Log("デバッグキーでゴール");
        GoToResult();
    }

    void GoToResult()
    {
        isTriggered = true;

        // ★★ 追加：自動HP減少（AutoDamage）を停止 ★★
        GameObject car = GameObject.FindWithTag(carTag);
        if (car != null)
        {
            var health = car.GetComponent<CarHealth>(); // ← CarHealth スクリプト名がこれで合っていればOK
            if (health != null)
            {
                health.StopAutoDamage();
            }
        }

        SaveGameData();

        if (goalSound != null)
            AudioSource.PlayClipAtPoint(goalSound, transform.position);

        Debug.Log($"シーン移動開始: {resultSceneName}");
        SteamVR_LoadLevel.Begin(resultSceneName);
    }

    void SaveGameData()
    {
        GameObject car = GameObject.FindWithTag(carTag);
        if (car != null) { /* HP保存処理など */ }

        var timer = FindFirstObjectByType<Timer>();
        if (timer != null) timer.StopTimer();
    }
}
