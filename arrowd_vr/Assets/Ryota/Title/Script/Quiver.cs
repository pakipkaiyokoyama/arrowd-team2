using UnityEngine;
using Valve.VR;                // SteamVR Input 用
using Valve.VR.InteractionSystem;

public class Quiver : MonoBehaviour
{
    public GameObject arrowPrefab;       // SteamVR の Arrow.prefab
    public Transform spawnPoint;         // Arrow が出る位置
    public Hand playerHand;              // 矢をつかむ手（RightHand など）

    public SteamVR_Action_Boolean grabAction;  // Squeeze / Trigger どちらでも可

    private GameObject currentArrow;

    void Update()
    {
        if (grabAction.GetStateDown(playerHand.handType))
        {
            TryGrabArrow();
        }
    }

    private void TryGrabArrow()
    {
        if (currentArrow != null) return; // すでに矢を持っている

        // 矢を生成
        currentArrow = Instantiate(arrowPrefab, spawnPoint.position, spawnPoint.rotation);

        // 手にアタッチ（SteamVR InteractionSystem）
        playerHand.AttachObject(currentArrow, GrabTypes.Grip);

        // isKinematic をオンにして弓につがえるまで物理を止める
        var rb = currentArrow.GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    public void ReleaseArrow()
    {
        currentArrow = null;
    }
}
