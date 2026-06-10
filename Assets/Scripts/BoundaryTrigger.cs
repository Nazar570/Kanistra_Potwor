using UnityEngine;

public class BoundaryTrigger : MonoBehaviour
{
    public string messageToShow = "Chyba lepiej, żebym nie szedł pieszo";
    public float displayTime = 5f;
    
    private HintManager hintManager;

    void Start()
    {
        hintManager = FindFirstObjectByType<HintManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Czegoś dotknąłem: " + other.name); 
        
        if (other.CompareTag("Player") && hintManager != null)
        {
            Debug.Log("To gracz! Wysyłam wiadomość...");
            // Wymuszenie czarnego koloru tekstu dla niewidzialnych ścian mapy
            hintManager.ShowHint($"<color=black>{messageToShow}</color>", displayTime);
        }
    }
}