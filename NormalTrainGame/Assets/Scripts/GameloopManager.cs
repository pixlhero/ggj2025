using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameloopManager : MonoBehaviour
{
    [SerializeField] private Color redAmbientColor;
    private Color standardAmbientColor;

    public DoorInteractible[] doors;

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
    
    private int[] maxBabies = new int[]{6, 11, 20, 31};
    private float [] timePerBaby = new float[]{10, 7, 4, 2};
    
    public int CurrentLevel = 0;

    public float secondsLeft;

    private Sequence _uiAnimationSequence;

    private bool _isInCooldownMode = false;
    
    private float maxTime => timePerBaby[CurrentLevel];

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

        foreach (var door in doors)
        {
            door.Opened += () => OnOpenedDoor(door);
        }
        
        standardAmbientColor = RenderSettings.ambientLight;
    }

    private void Update()
    {
        if (GameManager.Instance.gameState != GameManager.GameState.GAMEPLAY) return;

        if (_isInCooldownMode)
            return;

        timeBarFilled.fillAmount = secondsLeft / maxTime;

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
        secondsLeft = maxTime;

        timeUI.SetActive(true);

        middleDot.SetActive(true);

        AudioManager.Instance.StopLoopingFadeOut("Elevator", 1);
        AudioManager.Instance.PlayLoopingSound("Soundtrack");
        
        // Change Ambient Color
        RenderSettings.ambientLight = redAmbientColor;
    }

    public void ResetScore()
    {
        score = 0;
    }

    public void AddScore(int points)
    {
        score += points;
        AudioManager.Instance.PlayCalculatedAnnouncerSound(score);

        secondsLeft = maxTime;

        scoreText.text = "Babies Left: " + (maxBabies[CurrentLevel] - score);

        _uiAnimationSequence?.Kill();
        _uiAnimationSequence = DOTween.Sequence();

        scoreText.transform.localScale = Vector3.one;
        _uiAnimationSequence.Append(scoreText.transform.DOScale(Vector3.one * 1.5f, 0.1f).SetEase(Ease.Linear));
        _uiAnimationSequence.Append(scoreText.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.Linear));
        _uiAnimationSequence.Insert(0, timeUI.transform.DOScale(Vector3.one * 1.5f, 0.1f).SetEase(Ease.Linear));
        _uiAnimationSequence.Insert(0.1f, timeUI.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.Linear));

        if (score >= maxBabies[CurrentLevel])
        {
            if (CurrentLevel >= 3)
            {
                GotAllBabies?.Invoke();
            }
            else
            {
                CurrentLevel++;
                _isInCooldownMode = true;

                AudioManager.Instance.StopLoopingFadeOut("Soundtrack", 1);
                AudioManager.Instance.StartLoopingFadeIn("Elevator", .2f, .5f);
                RenderSettings.ambientLight = standardAmbientColor;

                scoreUI.SetActive(false);
                timeUI.SetActive(false);
            }
        }
    }

    private void OnOpenedDoor(DoorInteractible door)
    {
        if (!_isInCooldownMode)
            return;

        if (door == doors[CurrentLevel - 1])
        {
            AudioManager.Instance.StopLoopingFadeOut("Elevator", 1);
            AudioManager.Instance.StartLoopingFadeIn("Soundtrack", .2f, .5f);
            RenderSettings.ambientLight = redAmbientColor;

            _isInCooldownMode = false;
            secondsLeft = maxTime;
            score = 0;
            scoreUI.SetActive(true);
            timeUI.SetActive(true);
            scoreText.text = "Babies Left: " + (maxBabies[CurrentLevel] - score);
        }
    }
}
