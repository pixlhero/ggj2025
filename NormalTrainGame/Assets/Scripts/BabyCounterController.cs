using TMPro;
using UnityEngine;

public class BabyCounterController : MonoBehaviour
{
    public  TMP_Text babyCounterTextMesh;


    // Update is called once per frame
    void Update()
    {
        babyCounterTextMesh.text = GameloopManager.Instance.score.ToString();
    }
}
