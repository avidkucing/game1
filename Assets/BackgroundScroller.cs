using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Speed of the scrolling. Positive moves texture down (simulating forward movement).")]
    public float scrollSpeed = 0.5f;

    private Renderer bgRenderer;

    void Start()
    {
        bgRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        // Calculate the new Y offset
        // Mathf.Repeat ensures the value stays between 0 and 1, preventing massive numbers over time
        float yOffset = Mathf.Repeat(Time.time * scrollSpeed, 1);

        // Apply the offset to the texture
        bgRenderer.material.mainTextureOffset = new Vector2(0, yOffset);
    }
}