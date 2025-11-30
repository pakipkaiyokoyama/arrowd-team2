using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalFlag : MonoBehaviour
{
    void Update()
    {
        // デバッグ用：Mキーを押すとゴール
        if (Input.GetKeyDown(KeyCode.M))
        {
            DebugGoal();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            GoToResult(other.gameObject);
        }
    }

    // デバッグ用
    void DebugGoal()
    {
        GameObject car = GameObject.FindWithTag("Car");
        if (car != null)
        {
            GoToResult(car);
        }
        else
        {
            Debug.LogWarning("Carが見つかりません。");
        }
    }

    void GoToResult(GameObject car)
    {
        // Carの体力を保存
        Player carScript = car.GetComponent<Player>();
        if (carScript != null)
        {
            GameData.CarHP = carScript.HP;
        }
        else
        {
            Debug.LogWarning("Carスクリプトが見つかりません。");
            GameData.CarHP = 0f;
        }

        // タイマー停止
        Timer timer = FindTimer();
        if (timer != null)
        {
            timer.StopTimer();
        }
        else
        {
            Debug.LogWarning("Timerが見つかりません。");
            GameData.ClearTime = 0f;
        }

        // Resultシーンに移動
        SceneManager.LoadScene("Result");
    }

    // FindObjectOfType の古い形式を避ける安全版
    Timer FindTimer()
    {
        Timer[] timers = GameObject.FindObjectsOfType<Timer>();
        if (timers.Length > 0)
        {
            return timers[0];
        }
        return null;
    }
}
