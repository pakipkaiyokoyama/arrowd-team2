using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Valve.VR; // SteamVRの機能を使うために追加

public class ArrowSceneChanger : MonoBehaviour
{
    public string nextSceneName = "GameScene";

    void OnTriggerEnter(Collider other)
    {
        // 矢などが当たったらコルーチンを開始
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        Debug.Log("ロード開始...");

        // 1. 画面を黒くフェードアウトさせる（VR酔い防止の最重要ポイント！）
        SteamVR_Fade.Start(Color.black, 0.5f);

        // フェードが終わるまで少し待つ
        yield return new WaitForSeconds(0.5f);

        // 2. 裏側で非同期にシーンを読み込む
        // LoadSceneAsyncを使うと、読み込み中も処理が止まりにくいです
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextSceneName);

        // 読み込みが終わるまでここで待機する
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // 3. 次のシーンが始まったら、画面をフェードイン（黒から透明へ）戻す
        // ※これは次のシーンにあるスクリプト（SetPlayerPositionなど）でやるか、
        //   SteamVRが自動で戻してくれることが多いですが、念のため
        SteamVR_Fade.Start(Color.clear, 1.0f);
    }
}