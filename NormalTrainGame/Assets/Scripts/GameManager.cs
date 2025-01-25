using UnityEngine;
using System;


public class GameManager : MonoBehaviour
{
    // A singleton instance to allow easy access from any script
    public static GameManager Instance;

    public int score;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetScore() {
        score = 0;
    }

    public void AddScore(int points) {
        score += points;
    }
}
