using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Pickupable : MonoBehaviour
{
    [Header("Throw Settings")]
    [SerializeField] private float throwForce = 5f;   // How strong you want to throw

    public GameObject pickUpableObj;

    private Rigidbody rb;
    private Transform holdParent; // We'll store the holdParent for use on drop

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// Called when the player picks this object up.
    public void OnPickUp(Transform holdParent)
    {
        // Remember holdParent so we know the direction to throw later
        this.holdParent = holdParent;

        // Make this object kinematic so it doesn't collide or fall
        rb.isKinematic = true;
        rb.useGravity = false;
        pickUpableObj.GetComponent<Collider>().enabled = false;

        // Parent to the holdParent
        var goHolder = holdParent.transform.Find("GameObjectHolder");
        transform.SetParent(goHolder);

        // Reset local position/rotation so it lines up nicely
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    /// Called when the player drops this object.
    public void OnDrop()
    {
        // Remove from parent
        transform.SetParent(null);

        // Reactivate physics
        rb.isKinematic = false;
        rb.useGravity = true;
        pickUpableObj.GetComponent<Collider>().enabled = true;

        // Throw the object away from the player in the direction the holdParent is facing
        if (holdParent != null)
        {
            // Apply an impulse force in front of the player
            rb.AddForce(holdParent.forward * throwForce, ForceMode.Impulse);
        }
    }
}
