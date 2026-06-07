using UnityEngine;

public class FuelPath : MonoBehaviour
{
    [Header("Ustawienia Podpowiedzi")]
    [TextArea(3, 6)]
    public string tekstPodpowiedzi; // Tutaj wpisujesz unikalny tekst dla danej sfery

    [Tooltip("Przez ile sekund tekst ma się wyświetlać na ekranie")]
    public float czasWyswietlania = 4f;

    private void OnTriggerEnter(Collider other)
    {
        // Sprawdzamy, czy w sferę wszedł gracz (upewnij się, że gracz ma tag "Player")
        if (other.CompareTag("Player"))
        {
            // Szukamy Twojego skryptu HintManager na scenie
            HintManager hintManager = FindFirstObjectByType<HintManager>();

            if (hintManager != null)
            {
                // Wywołujemy Twoją metodę i przekazujemy tekst oraz czas z tej konkretnej sfery
                hintManager.ShowHint(tekstPodpowiedzi, czasWyswietlania);
            }
            else
            {
                Debug.LogError("[StrefaPodpowiedzi] Nie znaleziono HintManagera na scenie!");
            }
        }
    }
}