using UnityEngine;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance;

    public GameObject pickupHint;

    private void Awake()
    {
        Instance = this;

        // Hide the pickup hint by default
        pickupHint.SetActive(false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
