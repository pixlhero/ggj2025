using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    private Vector3 camDefaultLocalPos;
    
    [SerializeField] private float shakeSpeed;
    [SerializeField] private float shakeIntensity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camDefaultLocalPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        SubwayCameraShake();
    }

    // Subway Camera Shake Method ---
    private void SubwayCameraShake()
    {
        // Generate Perlin Noise offsets (smooth, natural rumble)
        float xOffset = (Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) - 0.5f) * shakeIntensity;
        float yOffset = (Mathf.PerlinNoise(0f, Time.time * shakeSpeed) - 0.5f) * shakeIntensity;

        // Apply them to the camera's default position
        Vector3 newLocalPos = camDefaultLocalPos + new Vector3(xOffset, yOffset, 0f);
        transform.localPosition = newLocalPos;
    }
}
