using UnityEngine;

public class AutoDestroyEffect : MonoBehaviour
{
    void Start()
    {
        // Destroy the effect object after 2 seconds
        Destroy(gameObject, 2f);
    }
}