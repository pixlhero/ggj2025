using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class IntroManager : MonoBehaviour
{
    public event Action IntroFinished;

    [SerializeField] private Transform introCameraWrapper;

    [SerializeField] private Transform playerCamera;

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject startText;
    
    [SerializeField] private GameObject startGameUI;
    
    [SerializeField] private GameObject screenSpaceUI;
    
    [SerializeField] private GameObject playerNpcModel;
    
    [SerializeField] private CanvasGroup darkOverlay;

    public GameObject middleDot;
    
    private bool _hasThrownBaby;
    
    private void Awake() {
        startText.SetActive(false);
        introCameraWrapper.gameObject.SetActive(false);
    }

    public void StartIntro()
    {
        darkOverlay.alpha = 1;
        StartCoroutine(StartIntroCoroutine());
    }

    private IEnumerator StartIntroCoroutine()
    {
        darkOverlay.DOFade(0, 1f);

        _hasThrownBaby = false;
        player.SetActive(false);
        introCameraWrapper.gameObject.SetActive(true);

        yield return new WaitForSeconds(4f);

        startText.SetActive(true);
        
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));

        startGameUI.SetActive(false);
        screenSpaceUI.SetActive(false);

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

        darkOverlay.DOFade(0, 0.2f).SetEase(Ease.InOutSine);
        
        playerNpcModel.SetActive(false);
        introCameraWrapper.gameObject.SetActive(false);

        startText.SetActive(false);
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
