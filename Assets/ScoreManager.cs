using UnityEngine;
using TMPro; // Wymaga TextMeshPro

public class ScoreManager : MonoBehaviour
{
    public int points = 0;
    public int pointsToWin = 2;
    public TextMeshProUGUI scoreText; 
    
    
    public bool isInThrowZone = false;

    void Update()
    {
        
        if (scoreText != null)
        {
            scoreText.text = "Punkty: " + points + " / " + pointsToWin;

            if (points >= pointsToWin)
            {
                scoreText.text = "ZADANIE WYKONANE!";
                scoreText.color = Color.green;
            }
        }
    }

    public void AddPoint()
    {
        
        if (isInThrowZone)
        {
            points++;
            Debug.Log("Trafienie! Masz punktów: " + points);
        }
        else
        {
            Debug.Log("Trafienie, ale stoisz poza strefą! Brak punktu.");
        }
    }

    private void OnTriggerStay(Collider other)
    {
  
        if (other.CompareTag("ThrowZone"))
        {
            isInThrowZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ThrowZone"))
        {
            isInThrowZone = false;
        }
    }
}