using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class IntroManager : MonoBehaviour
{
    public event Action IntroFinished;

    [SerializeField] private Transform introCameraWrapper;
    
    [SerializeField] private Transform introCamPos;
    
    [SerializeField] private Animator playerAnimator;

    [SerializeField] private Transform playerCamera;

    [SerializeField] private GameObject player;
    
    [SerializeField] private GameObject startGameUI;
    
    [SerializeField] private GameObject screenSpaceUI;
    
    [SerializeField] private GameObject playerNpcModel;
    
    [SerializeField] private CanvasGroup darkOverlay;
    
    [SerializeField] private Transform wakeupCamPos;
    
    [SerializeField] private CanvasGroup gameTitleCanvas;
    [SerializeField] private CanvasGroup gameTitle;
    [SerializeField] private CanvasGroup gameDisclaimer;
    
    public GameObject middleDot;
    
    public CanvasGroup creditsCanvas;
    private bool _showingCredits;
    
    private bool _hasThrownBaby;
    
    private static bool _hasAlreadyShownTitle; 
    
    private void Awake() {
        startGameUI.SetActive(false);
        introCameraWrapper.gameObject.SetActive(false);
        
        Cursor.visible = false;
    }
    
    private void Update() {
        if(Input.GetKeyDown(KeyCode.C) && !_showingCredits)
        {
            _showingCredits = true;
            var sequence = DOTween.Sequence();
            sequence.Insert(0, creditsCanvas.DOFade(1f, 0.5f));
            sequence.Insert(5f, creditsCanvas.DOFade(0f, 0.5f));
            sequence.onComplete += () => {_showingCredits = false;};
        }
    }

    public void StartIntro()
    {
        darkOverlay.alpha = 1;
        StartCoroutine(StartIntroCoroutine());
    }

    private IEnumerator StartIntroCoroutine()
    {
        _hasThrownBaby = false;
        player.SetActive(false);
        introCameraWrapper.gameObject.SetActive(true);
        introCameraWrapper.position = wakeupCamPos.position;
        introCameraWrapper.rotation = wakeupCamPos.rotation;
        screenSpaceUI.SetActive(false);
        
        if(!_hasAlreadyShownTitle)
        {
            gameTitleCanvas.alpha = 1;
            gameTitle.alpha = 0;
            gameDisclaimer.alpha = 0;
            
            yield return new WaitForSeconds(1f);
            gameTitle.DOFade(1, 1f);
            
            yield return new WaitForSeconds(2f);
            
            gameTitle.DOFade(0, 1f);
            
            yield return new WaitForSeconds(1f);
            
            gameDisclaimer.DOFade(1, 1f);
            
            yield return new WaitForSeconds(2f);
            
            gameDisclaimer.DOFade(0, 1f);
            
            yield return new WaitForSeconds(1f);
            
            gameTitleCanvas.DOFade(0, 1f);
            _hasAlreadyShownTitle = true;
        }


        // START INTRO CINEMATIC
        yield return new WaitForSeconds(1f);
        playerAnimator.Play("Wakeup");
        darkOverlay.DOFade(0, 0.2f);
        
        yield return new WaitForSeconds(1f);
        
        var sequenceCam1 = DOTween.Sequence();
        sequenceCam1.Insert(0, introCameraWrapper.transform.DOMove(introCamPos.position, 1.5f).SetEase(Ease.InOutSine));
        sequenceCam1.Insert(0, introCameraWrapper.transform.DORotate(introCamPos.rotation.eulerAngles, 1.5f).SetEase(Ease.InOutSine));

        yield return new WaitForSeconds(2f);

        screenSpaceUI.SetActive(true);
        screenSpaceUI.transform.localScale = Vector3.zero;
        screenSpaceUI.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

        yield return new WaitForSeconds(2f);

        startGameUI.SetActive(true);
        
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E) && !_showingCredits);

        startGameUI.SetActive(false);
        
        screenSpaceUI.transform.DOScale(Vector3.zero, 0.5f)
            .SetEase(Ease.InBack)
            .OnComplete(() => screenSpaceUI.SetActive(false));

        var sequence = DOTween.Sequence(); 
        sequence.Insert(0, introCameraWrapper.transform.DOMove(playerCamera.position, 1f).SetEase(Ease.InOutSine));
        sequence.Insert(0, introCameraWrapper.transform.DORotate(playerCamera.rotation.eulerAngles, 1f).SetEase(Ease.InOutSine));
        sequence.Play(); 

        yield return new WaitForSeconds(1.5f);
        
        var blinkSequence = DOTween.Sequence();
        blinkSequence.Append(darkOverlay.DOFade(1, 0.4f).SetEase(Ease.InOutSine));
        blinkSequence.Append(darkOverlay.DOFade(0, 0.4f).SetEase(Ease.InOutSine));
        blinkSequence.Append(darkOverlay.DOFade(1, 0.4f).SetEase(Ease.InOutSine));
        blinkSequence.Append(darkOverlay.DOFade(0, 0.4f).SetEase(Ease.InOutSine));
        blinkSequence.Append(darkOverlay.DOFade(1, 0.6f).SetEase(Ease.InOutSine));
        blinkSequence.AppendInterval(1f);
        
        yield return new WaitUntil(() => !blinkSequence.active || blinkSequence.IsComplete());

        AudioManager.Instance.Play("WakeUp");

        darkOverlay.DOFade(0, 0.2f).SetEase(Ease.InOutSine);
        
        playerNpcModel.SetActive(false);
        introCameraWrapper.gameObject.SetActive(false);

        startGameUI.SetActive(false);
        player.SetActive(true);
        middleDot.SetActive(true);
        
        yield return new WaitUntil(() => _hasThrownBaby);
        
        IntroFinished?.Invoke();
    }
    
    public void ThrownBaby()
    {
        _hasThrownBaby = true;
    }
}
