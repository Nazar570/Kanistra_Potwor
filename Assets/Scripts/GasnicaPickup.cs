using UnityEngine;

public class GasnicaPickup : MonoBehaviour
{
    private bool wZasiegu = false;
    private GasnicaSystem systemGracza;
    private HintManager hintManager;

    [Header("UI Interakcji")]
    public string tekstPodniesienia = "Naciśnij [E] aby wziąć gaśnicę";

    void Start()
    {
        hintManager = Object.FindFirstObjectByType<HintManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            wZasiegu = true;
            systemGracza = other.GetComponent<GasnicaSystem>();
            Debug.Log("Gracz wszedł w strefę gaśnicy!");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            wZasiegu = false;
            systemGracza = null;

            // Po wyjściu ze strefy natychmiast gasimy panel, jeśli to my go kontrolowaliśmy
            if (hintManager != null && hintManager.hintPanel != null)
                hintManager.hintPanel.SetActive(false);
                
            Debug.Log("Gracz wyszedł ze strefy gaśnicy.");
        }
    }

    void Update()
    {
        if (!wZasiegu) return;

        // Ciągłe wysyłanie tekstu interakcji (kolor czarny)
        if (hintManager != null)
        {
            hintManager.ShowHint($"<color=black>{tekstPodniesienia}</color>", 0.5f);
        }

        if (Input.GetKeyDown(KeyCode.E) && systemGracza != null)
        {
            // Gasimy panel przed zniszczeniem obiektu, by tekst nie wisiał w nieskończoność
            if (hintManager != null && hintManager.hintPanel != null)
                hintManager.hintPanel.SetActive(false);
            
            systemGracza.PodniesGasnice();
            Destroy(gameObject); 
        }
    }
}