using UnityEngine;
using System;
using UnityEngine.SceneManagement;


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
        Instance = this;
        
        FindFirstObjectByType<IntroManager>().IntroFinished += OnIntroFinished;
        FindFirstObjectByType<GameloopManager>().TimeRanOut += OnTimeRanOut;
        FindFirstObjectByType<GameOverManager>().GameOverFinished += OnGameoverFinished;
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
        GameOverManager.Instance.StartGameover();
    }
    
    private void OnGameoverFinished()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
