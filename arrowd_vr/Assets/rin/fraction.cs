using System.Collections;
using UnityEngine;

public class ScoreTarget : MonoBehaviour
{
    public int scoreValue = 10;
    public string arrowTag = "Arrow";

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(arrowTag)) return;
        //GameScore.Add(scoreValue); // 或通知 GameManager
        // 播放一个小爆炸 / 闪光，再隐藏
    }
}
