using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Pickupable : MonoBehaviour
{
    [Header("Throw Settings")]
    [SerializeField] private float throwForce = 5f;   // How strong you want to throw
    [SerializeField] private float minCollisionVelocity = 2.0f; // Minimum velocity required to play impact sound
    [SerializeField] private float collisionSoundCooldown = 0.5f; // Debounce interval (in seconds)

    [Header("Blood Effect")]
    [SerializeField] private GameObject bloodPrefab;  // Reference to your blood effect prefab

    public GameObject pickUpableObj;

    private Rigidbody rb;
    private Transform holdParent; // We'll store the holdParent for use on drop

    private bool isPickedUp = false;
    private float nextCollisionSoundTime = 0f; // Tracks when we can next play a collision sound

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Called when the player picks this object up.
    /// </summary>
    public void OnPickUp(Transform holdParent)
    {
        isPickedUp = true;
        this.holdParent = holdParent;

        rb.isKinematic = true;
        rb.useGravity = false;
        pickUpableObj.GetComponent<Collider>().enabled = false;

        var goHolder = holdParent.transform.Find("GameObjectHolder");
        transform.SetParent(goHolder);

        // Play PickUp sound
        AudioManager.Instance.Play("PickUp");

        // Reset local position/rotation so it lines up nicely
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    /// <summary>
    /// Called when the player drops this object.
    /// </summary>
    public void OnDrop()
    {
        isPickedUp = false;

        transform.SetParent(null);

        rb.isKinematic = false;
        rb.useGravity = true;
        pickUpableObj.GetComponent<Collider>().enabled = true;

        if (holdParent != null)
        {
            // Apply an impulse force in front of the player
            rb.AddForce(holdParent.forward * throwForce, ForceMode.Impulse);
        }
    }

    /// <summary>
    /// Detect collisions and play a sound if appropriate.
    /// </summary>
    /// <param name="collision">Collision data.</param>
    private void OnCollisionEnter(Collision collision)
    {
        // Only play collision sound if:
        // 1) We are NOT currently picked up,
        // 2) The collision velocity is above a minimum threshold,
        // 3) We are past the debounce (cooldown) time.
        if (!isPickedUp
            && collision.relativeVelocity.magnitude >= minCollisionVelocity
            && Time.time >= nextCollisionSoundTime)
        {
            // Play impact sound
            AudioManager.Instance.PlayRandomizedPitch("BabyThump");

            // Spawn blood effect at point of collision
            if (bloodPrefab != null && collision.contacts.Length > 0)
            {
                // Use the first contact point to position the blood effect
                ContactPoint contact = collision.contacts[0];
                Instantiate(bloodPrefab, contact.point, Quaternion.identity);
            }

            // Reset next time sound is allowed
            nextCollisionSoundTime = Time.time + collisionSoundCooldown;
        }
    }
}
