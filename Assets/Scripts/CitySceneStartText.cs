using UnityEngine;

public class CitySceneStartText : MonoBehaviour
{
    [Header("Ustawienia Tekstu na Start Sceny")]
    [TextArea(2, 5)] // Tworzy ładne, większe pole tekstowe w Inspektorze
    public string welcomeMessage = "Gdzie ja do cholery jestem? Muszę się stąd wydostać...";
    public float displayTime = 5f;

    void Start()
    {
        // Szukamy HintManagera obecnego na tej konkretnej scenie
        HintManager hintManager = Object.FindFirstObjectByType<HintManager>();

        if (hintManager != null)
        {
            // Odpalamy podpowiedź zaraz po uruchomieniu sceny
            hintManager.ShowHint(welcomeMessage, displayTime);
        }
        else
        {
            Debug.LogWarning("SceneStartHint: Nie znaleziono HintManagera na tej scenie!");
        }
    }
}