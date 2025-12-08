using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class ArrowSceneChanger : MonoBehaviour
{
    public string nextSceneName = "GameScene";
    public string arrowTag = "Arrow";

    private bool isLoading = false;

    void OnTriggerEnter(Collider other)
    {
        if (isLoading) return;

        bool isArrow = other.CompareTag(arrowTag) ||
                       other.GetComponent<Arrow>() != null ||
                       other.GetComponentInParent<Arrow>() != null;

        if (!isArrow) return;

        Debug.Log("矢が当たった！シーン移動開始");
        isLoading = true;

        // GoalFlagと同じ方法でシーン移動
        SteamVR_LoadLevel.Begin(nextSceneName);
    }
}