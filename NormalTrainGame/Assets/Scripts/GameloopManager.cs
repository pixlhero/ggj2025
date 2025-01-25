using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameloopManager : MonoBehaviour
{
    public event Action TimeRanOut;
    
    public event Action GotAllBabies;

    public static GameloopManager Instance;
    
    public GameObject scoreUI;
    public TMP_Text scoreText;
    
    public TMP_Text timeLeft;
    
    public GameObject timeUI;
    
    public Image timeBarFilled;
    
    public GameObject middleDot;

    public int score;
    
    private int maxBabies = 5;
    
    public float secondsLeft;
    
    private Sequence _uiAnimationSequence;

    private void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        timeUI.SetActive(false);
        scoreUI.SetActive(false);
        middleDot.SetActive(false);
        scoreText.text = "Babies Left: " + maxBabies;
    }
    
    private void Update() {
        if(GameManager.Instance.gameState != GameManager.GameState.GAMEPLAY) return;
        
        timeBarFilled.fillAmount = secondsLeft / 10f;

        if (secondsLeft > 0)
        {
            secondsLeft -= Time.deltaTime;
            timeLeft.text = (Mathf.Round(secondsLeft * 100) / 100).ToString();
        }
        else
        {
            scoreUI.SetActive(false);
            secondsLeft = 0;
            timeUI.SetActive(false);
            middleDot.SetActive(false);

            TimeRanOut?.Invoke();
        }
    }
    
    public void StartGameLoop()
    {
        scoreUI.SetActive(true);
        secondsLeft = 10;
        
        timeUI.SetActive(true);
        
        middleDot.SetActive(true);
    }

    public void ResetScore()
    {
        score = 0;
    }

    public void AddScore(int points)
    {
        score += points;
        AudioManager.Instance.PlayCalculatedAnnouncerSound(score);
        
        secondsLeft = 10;
        
        scoreText.text = "Babies Left: " + (maxBabies - score);
            
        _uiAnimationSequence?.Kill();
        _uiAnimationSequence = DOTween.Sequence();

        scoreText.transform.localScale = Vector3.one;
        _uiAnimationSequence.Append(scoreText.transform.DOScale(Vector3.one * 1.5f, 0.1f).SetEase(Ease.Linear));
        _uiAnimationSequence.Append(scoreText.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.Linear));
        _uiAnimationSequence.Insert(0, timeUI.transform.DOScale(Vector3.one * 1.5f, 0.1f).SetEase(Ease.Linear));
        _uiAnimationSequence.Insert(0.1f, timeUI.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.Linear));
        
        if(score >= maxBabies)
        {
            GotAllBabies?.Invoke();
        }
    }
}
