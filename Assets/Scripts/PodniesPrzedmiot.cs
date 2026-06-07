using UnityEngine;
using TMPro;

public class PodniesPrzedmiot : MonoBehaviour
{
    [Header("UI - Stary tekst (można zostawić pusty)")]
    public TextMeshProUGUI interactionText;
    
    [Tooltip("Zaznacz dla Łomu. Odznacz dla Klucza.")]
    public bool czyToLom = true; 
    public string nazwaPrzedmiotu = "Łom";

    private bool isPlayerNear = false;
    private HintManager hintManager; // System podpowiedzi

    void Start()
    {
        // Szukamy HintManagera na scenie automatycznie
        hintManager = Object.FindFirstObjectByType<HintManager>();

        if (interactionText != null) 
            interactionText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isPlayerNear) return;

        // Wyświetlanie hinta co klatkę (zniknie automatycznie po odejściu)
        if (hintManager != null)
        {
            hintManager.ShowHint($"Naciśnij [F], aby podnieść {nazwaPrzedmiotu}", 0.5f);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            // Dodaje przedmiot do pamięci (włącza obrazek w rogu)
            if (czyToLom) Ekwipunek.maLom = true;
            else Ekwipunek.maKlucz = true;

            // Sprząta stary tekst UI, jeśli istniał
            if (interactionText != null) 
                interactionText.gameObject.SetActive(false);
            
            // Gasimy natychmiast panel HintManagera, żeby napis nie wisiał po zniszczeniu obiektu
            if (hintManager != null && hintManager.hintPanel != null)
                hintManager.hintPanel.SetActive(false);

            Debug.Log($"{nazwaPrzedmiotu} podniesiony!");
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            
            // Czyszczenie hinta od razu przy wyjściu z triggera
            if (hintManager != null && hintManager.hintPanel != null)
                hintManager.hintPanel.SetActive(false);
        }
    }
}