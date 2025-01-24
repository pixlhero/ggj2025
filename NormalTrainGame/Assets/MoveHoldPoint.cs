using UnityEngine;

public class MoveHoldpoint : MonoBehaviour
{
    public Transform cameraPosition;

    // Update is called once per frame
    void Update()
    {
        transform.rotation = cameraPosition.rotation;
    }
}
