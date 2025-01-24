using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Pickupable : MonoBehaviour
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Called when the player picks this object up.
    /// </summary>
    public void OnPickUp(Transform holdParent)
    {
        // Make this object kinematic so it doesn't collide or fall
        rb.isKinematic = true;
        rb.useGravity = false;

        // Parent to the holdPoint
        transform.SetParent(holdParent);

        // Reset local position/rotation so it lines up nicely
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    /// <summary>
    /// Called when the player drops this object.
    /// </summary>
    public void OnDrop()
    {
        // Remove from parent
        transform.SetParent(null);

        // Reactivate physics
        rb.isKinematic = false;
        rb.useGravity = true;
    }
}
