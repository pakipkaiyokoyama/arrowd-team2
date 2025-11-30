using UnityEngine;
using System.Collections;
using Valve.VR.InteractionSystem;

public class SetPlayerPosition : MonoBehaviour
{
    IEnumerator Start()
    {
        // 1. ロード処理が落ち着くまで一瞬待つ
        yield return null;

        // 2. SteamVRのプレイヤーをフルネームで指定して探す
        var vrPlayer = Valve.VR.InteractionSystem.Player.instance;

        if (vrPlayer != null)
        {
            // 位置合わせ
            vrPlayer.transform.position = transform.position;

            // 向き合わせ（Y軸の回転だけ適用）
            Vector3 rot = transform.eulerAngles;
            vrPlayer.transform.rotation = Quaternion.Euler(0, rot.y, 0);

            Debug.Log("プレイヤーをスタート位置に移動しました");
        }
    }
}