using UnityEngine;

public class PosterMovement : MonoBehaviour
{
    
    private const float noiseSpeed = 1f;
    private const float angle = 7f;

    // Update is called once per frame
    void Update()
    {
        var noisyAngle = Mathf.PerlinNoise(Time.time * noiseSpeed, 3) * 2f - 1f;
        transform.localRotation = Quaternion.Euler(0, 0, noisyAngle * angle);
    }
}
