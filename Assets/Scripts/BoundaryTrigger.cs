using UnityEngine;

public class BoundaryTrigger : MonoBehaviour
{
    public string messageToShow = "Chyba lepiej, żebym nie szedł pieszo";
    public float displayTime = 5f;
    
    private HintManager hintManager;

    void Start()
    {
        // Szukamy managera na scenie
        hintManager = FindFirstObjectByType<HintManager>();
    }

    private void OnTriggerEnter(Collider other)
{
    Debug.Log("Czegoś dotknąłem: " + other.name); // To wypisze w konsoli nazwę wszystkiego, co wejdzie w ścianę
    
    if (other.CompareTag("Player"))
    {
        Debug.Log("To gracz! Wysyłam wiadomość...");
        hintManager.ShowHint(messageToShow, displayTime);
    }
}
}