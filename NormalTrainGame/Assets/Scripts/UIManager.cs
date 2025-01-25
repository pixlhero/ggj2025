using UnityEngine;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance;

    public GameObject pickupHint;
    
    public GameObject alwaysLockedHint;
    
    public GameObject notEnoughScoreHint;

    private void Awake()
    {
        Instance = this;

        // Hide the pickup hint by default
        pickupHint.SetActive(false);
        alwaysLockedHint.SetActive(false);
        notEnoughScoreHint.SetActive(false);
    }
    
    public void SetPickupHintActive(GameObject pickupObject) 
    {
        if(pickupObject.TryGetComponent(out DoorInteractible door))
        {
            var cannotOpenBecauseOfScore = GameloopManager.Instance.score < door.miniumScore;
            var alwaysLocked = door.alwaysLocked;
            
            if(cannotOpenBecauseOfScore)
            {
                notEnoughScoreHint.SetActive(true);
                alwaysLockedHint.SetActive(false);
                pickupHint.SetActive(false);
            }
            else if(alwaysLocked)
            {
                alwaysLockedHint.SetActive(true);
                notEnoughScoreHint.SetActive(false);
                pickupHint.SetActive(false);
            }
            else{
                pickupHint.SetActive(true);
                notEnoughScoreHint.SetActive(false);
                alwaysLockedHint.SetActive(false);
            }
        }
        else{
            pickupHint.SetActive(true);
            notEnoughScoreHint.SetActive(false);
            alwaysLockedHint.SetActive(false);
        }
    }
    
    public void SetPickupHintInactive() 
    {
        pickupHint.SetActive(false);
        notEnoughScoreHint.SetActive(false);
        alwaysLockedHint.SetActive(false);
    }
}
