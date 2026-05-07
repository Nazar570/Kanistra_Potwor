using UnityEngine;

public class NoFuelCar : MonoBehaviour
{
    public HintManager hintManager; // Przeciągnij tu obiekt, na którym jest skrypt HintManager
    public string carMessage = "No nie, pusty bak!\nPotrzebuje paliwa";
    public float displayDuration = 3f;

    void Start()
    {
        // Wyświetla komunikat zaraz po załadowaniu sceny
        if (hintManager != null)
        {
            hintManager.ShowHint(carMessage, displayDuration);
        }
    }

    // Wywołuje się, gdy klikniesz na samochód (wymaga Collidera na aucie!)
    void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player")) // Sprawdza, czy to gracz podszedł
    {
        hintManager.ShowHint(carMessage, displayDuration);
    }
}
}