using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonHitToStart : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // –î‚É‚¾‚¯”½‰ž‚³‚¹‚é
        if (other.CompareTag("Arrow"))
        {
            SceneManager.LoadScene("Main");
        }
    }
}
