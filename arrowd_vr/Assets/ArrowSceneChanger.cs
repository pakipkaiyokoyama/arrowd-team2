using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
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
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        Debug.Log("ロード開始...");

        // Playerを保護
        PrepareForSceneChange();

        // 1. フェードアウト
        SteamVR_Fade.Start(Color.black, 0.3f);
        yield return new WaitForSeconds(0.5f);

        // 2. 非同期ロード
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextSceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Debug.Log("ロード完了！");

        // 3. 少し待つ（シーンの初期化を待つ）
        yield return new WaitForSeconds(0.3f);

        // 4. 強制フェードイン（複数回実行して確実に）
        SteamVR_Fade.Start(Color.clear, 0f);
        yield return null;
        SteamVR_Fade.Start(Color.clear, 0.5f);

        Debug.Log("フェードイン実行！");
    }

    void PrepareForSceneChange()
    {
        Player player = Player.instance;
        if (player == null) return;

        DontDestroyOnLoad(player.gameObject);

        foreach (Hand hand in player.hands)
        {
            if (hand == null) continue;

            GameObject held = hand.currentAttachedObject;
            if (held != null)
            {
                if (!held.transform.IsChildOf(player.transform))
                {
                    var interactable = held.GetComponent<Interactable>();
                    if (interactable != null)
                    {
                        interactable.enabled = false;
                    }
                    held.transform.SetParent(player.transform, true);
                }
            }
        }
    }
}