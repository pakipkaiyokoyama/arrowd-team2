using UnityEngine;

public class BowController : MonoBehaviour
{
    public enum ArrowMode { None, Move, Attack }
    public ArrowMode currentMode = ArrowMode.None;

    [Header("Arrow Prefabs")]
    public GameObject moveArrowPrefab;
    public GameObject attackArrowPrefab;

    [Header("Bow Settings")]
    public Transform shootPoint;
    public float shootForce = 25f;

    public void SetArrowMode(ArrowMode mode)
    {
        currentMode = mode;
        Debug.Log("Arrow mode changed to: " + currentMode);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (currentMode == ArrowMode.None) return;

        GameObject arrowPrefab = currentMode == ArrowMode.Move ? moveArrowPrefab : attackArrowPrefab;
        GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, shootPoint.rotation);
        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        rb.AddForce(shootPoint.forward * shootForce, ForceMode.Impulse);
    }
}
