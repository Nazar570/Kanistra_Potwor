using UnityEngine;

public class NoFuelCar : MonoBehaviour
{
    public HintManager hintManager; // Przeciągnij tu obiekt, na którym jest skrypt HintManager
    public string carMessage = "No nie, pusty bak!\nPotrzebuje paliwa";
    public float displayDuration = 3f;

    void Start()
    {
        // Jeśli nie przypisano w Inspektorze, szukamy automatycznie
        if (hintManager == null) hintManager = Object.FindFirstObjectByType<HintManager>();

        // Wyświetla komunikat zaraz po załadowaniu sceny (kolor czarny)
        if (hintManager != null)
        {
            hintManager.ShowHint($"<color=black>{carMessage}</color>", displayDuration);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && hintManager != null) // Sprawdza, czy to gracz podszedł
        {
            hintManager.ShowHint($"<color=black>{carMessage}</color>", displayDuration);
        }
    }
}