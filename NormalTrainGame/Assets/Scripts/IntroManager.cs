using System;
using System.Collections;
using UnityEngine;

public class IntroManager : MonoBehaviour
{
    public event Action IntroFinished;

    [SerializeField] private FirstPersonController player;
    [SerializeField] private GameObject startText;
    
    private void Awake() {
        startText.SetActive(false);
    }

    public void StartIntro()
    {
        StartCoroutine(StartIntroCoroutine());
    }

    private IEnumerator StartIntroCoroutine()
    {
        startText.SetActive(true);
        player.enabled = false;
        
        yield return new WaitForSeconds(3f);
        startText.SetActive(false);
        player.enabled = true;
        
        IntroFinished?.Invoke();
    }
}
