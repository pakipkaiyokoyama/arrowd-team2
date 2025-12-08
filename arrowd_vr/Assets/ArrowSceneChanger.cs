using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class ArrowSceneChanger : MonoBehaviour
{
    public string nextSceneName = "GameScene";
    public string arrowTag = "Arrow";

    private bool isLoading = false; // 二重実行防止

    void OnTriggerEnter(Collider other)
    {
        // すでにロード中なら無視
        if (isLoading) return;

        // 矢かどうかチェック（タグまたはArrowコンポーネント）
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

        // 1. フェードアウト
        SteamVR_Fade.Start(Color.black, 0.5f);
        yield return new WaitForSeconds(0.5f);

        // 2. 非同期ロード
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextSceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // 3. フェードイン
        SteamVR_Fade.Start(Color.clear, 1.0f);
    }
}