using UnityEngine;
using System.Collections;
using Valve.VR.InteractionSystem;

public class SetPlayerPosition : MonoBehaviour
{
    IEnumerator Start()
    {
        // 1. ロード処理が落ち着くまで少し待つ（画面が暗い間に位置合わせするため）
        yield return new WaitForSeconds(0.1f);

        // 2. SteamVRのプレイヤーを探す
        var vrPlayer = Valve.VR.InteractionSystem.Player.instance;

        if (vrPlayer != null)
        {
            // 位置をこのオブジェクト（StartPoint）と同じ場所にする
            vrPlayer.transform.position = transform.position;

            // 向きをこのオブジェクトと同じ向きにする（Y軸＝横回転だけ合わせる）
            Vector3 targetRotation = transform.eulerAngles;
            vrPlayer.transform.rotation = Quaternion.Euler(0, targetRotation.y, 0);

            Debug.Log("プレイヤーの位置と向きを修正しました");
        }
    }
}