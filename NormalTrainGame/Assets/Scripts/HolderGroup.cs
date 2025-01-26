using UnityEngine;

public class HolderGroup : MonoBehaviour
{
    
    private const float noiseSpeed = 1f;
    private const float angle = 3f;

    // Update is called once per frame
    void Update()
    {
        var noisyAngle = Mathf.PerlinNoise(Time.time *noiseSpeed, 0) * 2f - 1f;
        transform.localRotation = Quaternion.Euler(noisyAngle * angle, 0, 0);
    }
}
