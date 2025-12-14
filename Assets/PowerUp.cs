using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public float speed = 2.0f;

    void Update()
    {
        // Move down slowly
        transform.Translate(Vector2.down * speed * Time.deltaTime);

        // Destroy if it goes off screen
        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }
}