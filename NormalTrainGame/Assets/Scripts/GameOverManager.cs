using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public event Action GameOverFinished;

    public CanvasGroup blackOverlay;

    // A singleton instance to allow easy access from any script
    public static GameOverManager Instance;

    private void Awake()
    {
        // Enforce singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Make sure the AudioManager persists across scene loads
        DontDestroyOnLoad(gameObject);
    }

    public void StartGameover(){
        blackOverlay.DOFade(0f, 1f);
        
        StartCoroutine(StartGameoverCoroutine());
    }
    
    private IEnumerator StartGameoverCoroutine(){
        blackOverlay.DOFade(1f, 1f);
        
        yield return new WaitForSeconds(1f);
        
        GameOverFinished?.Invoke();
    }
}
