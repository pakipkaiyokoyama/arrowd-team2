using UnityEngine;

public class WheelRotate : MonoBehaviour
{
    public float speed = 180f;

    void Update()
    {
        transform.Rotate(Vector3.forward * speed * Time.deltaTime);
    }
}
