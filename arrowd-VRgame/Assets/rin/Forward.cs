using UnityEngine;

public class Forward : MonoBehaviour
{
    public float speed = 200f;

    void Update()
    {
        // 绕 X 轴转
        transform.Rotate(0f, speed * Time.deltaTime, 0f);
    }
}
