using UnityEngine;
using Valve.VR.InteractionSystem;

public class ScrollDisplayController : MonoBehaviour
{
    [Header("弓とタイトルUIの切り替え対象")]
    public GameObject bowRoot;    // シーンに置いてある弓オブジェクトのルート（最初は非アクティブ）
    public GameObject titleUI;    // タイトル画面用の Canvas / UI（最初は非アクティブ）

    [Header("オプション設定")]
    public bool setBowActiveOnAttach = true;
    public bool setUIActiveOnAttach = true;

    // Interactable イベント：手にくっついたとき
    private void OnAttachedToHand(Hand hand)
    {
        if (bowRoot != null && setBowActiveOnAttach)
        {
            bowRoot.SetActive(true);
            // 弓をカメラ前に表示し、少しだけ位置を調整する
            PositionBowInFrontOfCamera(bowRoot);
        }

        if (titleUI != null && setUIActiveOnAttach)
        {
            titleUI.SetActive(true);
        }
    }

    // Interactable イベント：手から離れたとき
    private void OnDetachedFromHand(Hand hand)
    {
        if (bowRoot != null && setBowActiveOnAttach)
        {
            bowRoot.SetActive(false);
        }

        if (titleUI != null && setUIActiveOnAttach)
        {
            titleUI.SetActive(false);
        }
    }

    private void PositionBowInFrontOfCamera(GameObject bow)
    {
        // メインカメラの前方に弓を配置する
        var cam = Camera.main;
        if (cam == null) return;

        bow.transform.position =
            cam.transform.position + cam.transform.forward * 1.2f + cam.transform.up * -0.2f;
        bow.transform.rotation = Quaternion.LookRotation(cam.transform.forward, cam.transform.up);
    }
}
