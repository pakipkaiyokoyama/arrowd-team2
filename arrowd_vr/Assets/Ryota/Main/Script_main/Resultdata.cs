using UnityEngine;

public class Timer : MonoBehaviour
{
    private float timeElapsed = 0f;
    private bool isRunning = true;

    void Update()
    {
        if (isRunning)
        {
            timeElapsed += Time.deltaTime;
        }
    }

    public void StopTimer()
    {
        isRunning = false;
        GameData.ClearTime = timeElapsed;
    }
}
