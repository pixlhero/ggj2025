using TMPro;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    public  TMP_Text timerCounterTextMesh;


    // Update is called once per frame
    void Update()
    {
        var secondsLeft = GameloopManager.Instance.secondsLeft;
        var time = (Mathf.Round(secondsLeft * 100) / 100).ToString();
        timerCounterTextMesh.text = time;
    }
}
