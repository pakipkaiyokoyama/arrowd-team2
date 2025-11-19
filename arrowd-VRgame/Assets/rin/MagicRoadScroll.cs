using UnityEngine;

public class MagicRoadScroll : MonoBehaviour
{
    public float speed = 0.3f;
    Renderer rend;
    Vector2 offset;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        offset.x += speed * Time.deltaTime;
        rend.material.SetTextureOffset("_MainTex", offset);
    }
}
