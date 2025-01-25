using UnityEngine;

public class ScoreZone : MonoBehaviour
{
    [Tooltip("Assign the tag used by your Pickupable object. Alternatively, check for the Pickupable component in OnTriggerEnter.")]
    [SerializeField] private string pickupableTag = "Pickupable";

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered by: " + other.name);
        Debug.Log("Tag: " + other.tag);

        // If you prefer using a tag check:
        if (other.CompareTag(pickupableTag))
        {
            GameManager.Instance.AddScore(1);
        }
    }
}
