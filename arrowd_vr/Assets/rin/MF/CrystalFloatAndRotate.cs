using UnityEngine;

public class CrystalFloatAndRotate : MonoBehaviour
{
    [Header("上下浮动")]
    public float floatAmplitude = 0.2f;
    public float floatFrequency = 1f;

    [Header("旋转")]
    public float rotateSpeed = 20f;

    private Vector3 startLocalPos;

    void Start()
    {
        startLocalPos = transform.localPosition;
    }

    void Update()
    {
        float offsetY = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.localPosition = startLocalPos + new Vector3(0f, offsetY, 0f);

        // 🔥 沿着自己的 Z 轴旋转（自转）
        transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime, Space.Self);
    }
}
