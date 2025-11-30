using UnityEngine;
using TMPro;

public class ResultManager : MonoBehaviour
{
    public TMP_Text timeText;
    public TMP_Text hpText;
    public TMP_Text scoreText;
    public TMP_Text rankText;

    void Start()
    {
        float time = GameData.ClearTime;
        int m = Mathf.FloorToInt(time / 60f);
        int s = Mathf.FloorToInt(time % 60f);
        float hp = GameData.CarHP;

        // スコア計算（例）
        float score = hp + Mathf.Max(0, 100 - time);

        // ランク判定
        string rank;
        if (score >= 150) rank = "S";
        else if (score >= 120) rank = "A";
        else if (score >= 80) rank = "B";
        else rank = "C";

        // TMP_Textに右揃えで表示
        timeText.alignment = TextAlignmentOptions.Right;
        hpText.alignment = TextAlignmentOptions.Right;
        scoreText.alignment = TextAlignmentOptions.Right;
        rankText.alignment = TextAlignmentOptions.Right;

        timeText.text = $"{m}:{s}";
        hpText.text = $"{hp:F0}";
        scoreText.text = $"{score:F0}";
        rankText.text = $"{rank}";
    }
}
