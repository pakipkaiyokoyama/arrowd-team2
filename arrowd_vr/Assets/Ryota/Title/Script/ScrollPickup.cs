using UnityEngine;
using Valve.VR.InteractionSystem;

public class ScrollDisplayController : MonoBehaviour
{
    [Header("表示切替するオブジェクト")]
    public GameObject bowRoot;    // シーンに置いた弓モデルのルート（最初は非アクティブ）
    public GameObject titleUI;    // Canvas or UI の親（最初は非アクティブ）

    [Header("オプション設定")]
    public bool setBowActiveOnAttach = true;
    public bool setUIActiveOnAttach = true;

    // Interactable のイベントとして呼ばれる
    private void OnAttachedToHand(Hand hand)
    {
        if (bowRoot != null && setBowActiveOnAttach)
        {
            bowRoot.SetActive(true);
            // もし弓を画面中央やカメラ前に表示したければ位置調整をここで行う
            PositionBowInFrontOfCamera(bowRoot);
        }

        if (titleUI != null && setUIActiveOnAttach)
        {
            titleUI.SetActive(true);
        }
    }

    // Interactable のイベントとして呼ばれる
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
        // 例：メインカメラの正面に表示（タイトル画面での見栄え調整）
        var cam = Camera.main;
        if (cam == null) return;

        // bow をワールド上の位置で置きたいときだけ有効化
        bow.transform.position = cam.transform.position + cam.transform.forward * 1.2f + cam.transform.up * -0.2f;
        bow.transform.rotation = Quaternion.LookRotation(cam.transform.forward, cam.transform.up);
    }
}
