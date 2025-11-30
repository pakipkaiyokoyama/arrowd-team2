using UnityEngine;
using Valve.VR.InteractionSystem;
using UnityEngine.SceneManagement;

public class ArrowSceneChanger : MonoBehaviour
{
    [Header("移動先のシーン名")]
    public string nextSceneName = "GameScene";

    [Header("当たった時の音")]
    public AudioClip hitSound;

    private bool isTriggered = false;

    // ★変更点：OnTriggerEnter に変更しました
    void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;

        // デバッグ用：何かが当たったらコンソールに名前を出す
        Debug.Log("何かが当たりました: " + other.name);

        // 矢かどうかチェック
        if (other.GetComponentInParent<Arrow>() != null)
        {
            Debug.Log("矢だと認識しました！シーン移動を開始します");
            isTriggered = true;

            if (hitSound != null) AudioSource.PlayClipAtPoint(hitSound, transform.position);

            SceneManager.LoadScene(nextSceneName);
        }
    }
}