using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool gameStarted = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void StartGame()
    {
        if (gameStarted) return;
        gameStarted = true;

        //FindAnyObjectByType<EnemySpawner>()?.BeginSpawn();
        //FindAnyObjectByType<CartController>()?.StartMoving();
    }
}
