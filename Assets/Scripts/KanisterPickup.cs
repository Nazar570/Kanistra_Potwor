using UnityEngine;
using TMPro;

public class KanisterPickup : MonoBehaviour
{
    [Header("UI - Stary tekst (można zostawić pusty)")]
    public TextMeshProUGUI interactionText;
    
    [Header("Ustawienia przedmiotu")]
    public string nazwaPrzedmiotu = "Kanister";

    private bool isPlayerNear = false;
    private HintManager hintManager; // System podpowiedzi
    [Header("Porządki po ataku (Nowe opcje)")]
    public GameObject sladIBenzynaGroup; // DODANE: Wyłączy całą grupę obiektów za jednym zamachem

    void Start()
    {
        // Automatycznie szukamy HintManagera na scenie, tak jak w aucie
        hintManager = Object.FindFirstObjectByType<HintManager>();
        
        if (interactionText != null)
            interactionText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isPlayerNear) return;

        // Karmienie HintManagera tekstem podnoszenia co klatkę (dzięki czemu zniknie po odejściu)
        if (hintManager != null)
        {
            hintManager.ShowHint($"Naciśnij [F], aby podnieść {nazwaPrzedmiotu}", 0.5f);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Ekwipunek.maKanister = true;
            
            // Gasimy stary tekst UI jeśli istniał
            if (interactionText != null)
                interactionText.gameObject.SetActive(false);
                
            // Gasimy panel HintManagera natychmiast po podniesieniu przedmiotu
            if (hintManager != null && hintManager.hintPanel != null)
                hintManager.hintPanel.SetActive(false);

            Debug.Log($"{nazwaPrzedmiotu} podniesiony!");
             // --- DODANE: CZYSZCZENIE ŚLADÓW I AKTUALIZACJA TEKSTU ---
            if (sladIBenzynaGroup != null)
            {
                sladIBenzynaGroup.SetActive(false); // Wyłącza ślad benzyny oraz wszystkie sfery pod nim
            }
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
            
            // Po odejściu od przedmiotu gasimy panel hinta
            if (hintManager != null && hintManager.hintPanel != null)
                hintManager.hintPanel.SetActive(false);
        }
    }
}