using UnityEngine;

public class Crystal_move : MonoBehaviour
{
    [Header("â~â^ìÆê›íË")]
    public float circleRadius = 2f;
    public float circleSpeed = 1f;

    [Header("é©ì]ê›íË")]
    public float rotateSpeed = 20f;

    private Vector3 startLocalPos;

    void Start()
    {
        startLocalPos = transform.localPosition;
    }

    void Update()
    {
        // XZ ïΩñ ÇÃâ~â^ìÆ
        float angle = Time.time * circleSpeed;
        float offsetX = Mathf.Cos(angle) * circleRadius;
        float offsetZ = Mathf.Sin(angle) * circleRadius;

        transform.localPosition = startLocalPos + new Vector3(offsetX, 0f, offsetZ);

        // é©ì]
        transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime, Space.Self);
    }
}
