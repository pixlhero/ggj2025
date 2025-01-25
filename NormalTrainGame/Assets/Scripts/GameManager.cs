using UnityEngine;
using System;


public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        INTRO,
        GAMEPLAY,
        GAMEOVER,
    }
    
    
    public GameState gameState = GameState.INTRO;
    
    // A singleton instance to allow easy access from any script
    public static GameManager Instance;

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
        
        FindFirstObjectByType<IntroManager>().IntroFinished += OnIntroFinished;
        FindFirstObjectByType<GameloopManager>().TimeRanOut += OnTimeRanOut;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // add ambience
        AudioManager.Instance.PlayLoopingSound("AmbianceTrain");
        AudioManager.Instance.StartRandomInterval("Yamanote");
        
        gameState = GameState.INTRO;
        FindFirstObjectByType<IntroManager>().StartIntro();
    }
    
    private void OnIntroFinished()
    {
        gameState = GameState.GAMEPLAY;
        FindFirstObjectByType<GameloopManager>().StartGameLoop();
    }
    
    private void OnTimeRanOut()
    {
        gameState = GameState.GAMEOVER;
    }
}
