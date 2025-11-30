using UnityEngine;
using System.Collections;
using Valve.VR; // これがないとSteamVRの機能が使えない

public class ArrowSceneChanger : MonoBehaviour
{
    [Header("移動先のシーン名")]
    public string nextSceneName = "GameScene";

    [Header("当たった時の音")]
    public AudioClip hitSound;

    private bool isTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;

        // 矢が当たったかチェック
        // (Arrowコンポーネントを持っているか、親が持っているか)
        if (other.GetComponentInParent<Valve.VR.InteractionSystem.Arrow>() != null)
        {
            isTriggered = true;
            Debug.Log("矢が当たりました！暗転ロードを開始します");

            if (hitSound != null)
            {
                AudioSource.PlayClipAtPoint(hitSound, transform.position);
            }

            // ★ここが変更点！
            // Unity標準の LoadScene ではなく、SteamVR専用のロードを使います
            // これだけで「フェードアウト → ロード → フェードイン」を全部やってくれます
            SteamVR_LoadLevel.Begin(nextSceneName);
        }
    }
}