using UnityEngine;

public class SingletonPlayer : MonoBehaviour
{
    private static SingletonPlayer instance;

    void Awake()
    {
        // ‚·‚Å‚É‘¶İ‚µ‚Ä‚¢‚½‚ç©•ª‚ğÁ‚·
        if (instance != null && instance != this)
        {
            Debug.Log("d•¡Player‚ğíœ");
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}