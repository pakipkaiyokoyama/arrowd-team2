using UnityEngine;

public class CubeMoveTest : MonoBehaviour
{
    public float speed = 3f;

    void Update()
    {
        // 沿 Z 轴移动，生成轨迹
        transform.position += Vector3.forward * speed * Time.deltaTime;
    }
}
