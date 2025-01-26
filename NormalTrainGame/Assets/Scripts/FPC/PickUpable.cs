using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Pickupable : MonoBehaviour
{
    [Header("Throw Settings")]
    [SerializeField] private float throwForce = 5f;   // How strong you want to throw
    [SerializeField] private float minCollisionVelocity = 2.0f; // Minimum velocity required to play impact sound
    [SerializeField] private float collisionSoundCooldown = 0.5f; // Debounce interval (in seconds)

    [Header("Blood Effect")]
    [SerializeField] private GameObject collisionParticlePrefab;

    [Tooltip("How long to keep the collision prefab in the scene before destroying it.")]
    [SerializeField] private float collisionPrefabLifetime = 5f;

    public GameObject pickUpableObj;

    private Rigidbody rb;
    private Transform holdParent; // We'll store the holdParent for use on drop

    private bool isPickedUp = false;
    private float nextCollisionSoundTime = 0f; // Tracks when we can next play a collision sound

    // Store the rocking Tween so we can kill it later
    private Tween rockingTween;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        // Start the rocking motion
        // Start a gentle rocking motion with DoTween
        // Adjust the angle, duration, and easing as desired
        rockingTween = pickUpableObj.transform.DOLocalRotate(
            new Vector3(10f, 0f, 0f), // the rotation we add
            0.5f,                     // duration of one half of the rock
            RotateMode.LocalAxisAdd   // rotate relative to current rotation
        )
        .SetLoops(-1, LoopType.Yoyo) // repeat forever, back and forth
        .SetEase(Ease.InOutSine);    // smooth back-and-forth
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
        gameObject.GetComponentInChildren<Collider>().enabled = false;

        var goHolder = holdParent.transform.Find("GameObjectHolder");
        transform.SetParent(goHolder);

        // Play PickUp sound
        AudioManager.Instance.Play("PickUp");

        // Play baby crying
        AudioManager.Instance.Play("BabyCry");

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
        gameObject.GetComponentInChildren<Collider>().enabled = true;

        // Stop baby crying
        AudioManager.Instance.StopLoopingSound("BabyCry");

        // Play baby sendoff
        AudioManager.Instance.Play("BabySendoff");

        // Kill the rocking motion when dropped
        if (rockingTween != null && rockingTween.IsActive())
        {
            rockingTween.Kill();
        }

        if (holdParent != null)
        {
            // Apply an impulse force in front of the player
            var throwDirection = holdParent.forward + holdParent.up * 0.4f;
            throwDirection.Normalize();
            rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
        }
    }

    /// <summary>
    /// Detect collisions and play a sound if appropriate.
    /// </summary>
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
            // Determine which sound to play based on the tag of the collided object
            string collisionTag = collision.gameObject.tag;
            switch (collisionTag)
            {
                case "Metal":
                    AudioManager.Instance.Play("MetalThump");
                    break;
                case "BigMetal":
                    AudioManager.Instance.Play("BigMetalThump");
                    break;
                case "DampThud":
                    AudioManager.Instance.Play("DampThud");
                    break;
                case "Thump":
                    AudioManager.Instance.Play("Thump");
                    break;
                case "Oof":
                    AudioManager.Instance.PlayRandomizedPitch("Oof");
                    break;
                default:
                    // Fallback sound if the tag doesn't match known ones
                    AudioManager.Instance.Play("BabyThump");
                    break;
            }

            // Spawn collision effect at the point of collision
            if (collisionParticlePrefab != null && collision.contacts.Length > 0)
            {
                ContactPoint contact = collision.contacts[0];
                GameObject spawnedEffect = Instantiate(collisionParticlePrefab, contact.point, Quaternion.identity);

                // Destroy the spawned effect after the specified lifetime
                Destroy(spawnedEffect, collisionPrefabLifetime);
            }

            // Reset next time sound is allowed
            nextCollisionSoundTime = Time.time + collisionSoundCooldown;
        }
    }
}
