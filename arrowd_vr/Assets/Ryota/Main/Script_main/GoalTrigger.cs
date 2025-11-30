using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

public class GoalFlag : MonoBehaviour
{
    [Header("【設定】移動先のリザルトシーン名")]
    public string resultSceneName = "Result"; // 画像では "map__" になっていましたが、ゴールシーン名を入れてください

    [Header("【設定】ゴールした時の効果音")]
    public AudioClip goalSound;

    [Header("【設定】当たると反応するタグ")]
    public string arrowTag = "Arrow";
    public string carTag = "Car";

    private bool isTriggered = false;

    // ★変更点：OnCollisionEnter（物理的な衝突）を使います
    void OnCollisionEnter(Collision collision)
    {
        if (isTriggered) return;

        // 矢が当たった場合（タグ判定 または Arrowコンポーネント判定）
        if (collision.gameObject.CompareTag(arrowTag) ||
            collision.gameObject.GetComponentInParent<Valve.VR.InteractionSystem.Arrow>() != null)
        {
            Debug.Log("矢が命中（衝突）！ゴール！");
            GoToResult();
        }

        // 馬車（Car）が当たった場合
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
        SaveGameData(); // 前回のデータ保存処理（省略していなければここに）

        if (goalSound != null) AudioSource.PlayClipAtPoint(goalSound, transform.position);

        Debug.Log($"シーン移動開始: {resultSceneName}");
        SteamVR_LoadLevel.Begin(resultSceneName);
    }

    void SaveGameData()
    {
        // （ここは前回のままでOKです。データ保存処理があれば残してください）
        GameObject car = GameObject.FindWithTag(carTag);
        if (car != null) { /* HP保存処理など */ }

        var timer = FindFirstObjectByType<Timer>();
        if (timer != null) timer.StopTimer();
    }
}