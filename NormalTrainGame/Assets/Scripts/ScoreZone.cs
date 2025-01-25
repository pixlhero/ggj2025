using UnityEngine;

public class ScoreZone : MonoBehaviour
{
    [Tooltip("Assign the tag used by your Pickupable object. Alternatively, check for the Pickupable component in OnTriggerEnter.")]
    [SerializeField] private string pickupableTag = "Pickupable";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(pickupableTag))
        {
            GameloopManager.Instance.AddScore(1);
        }
    }
}
