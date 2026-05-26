using UnityEngine;
using TMPro;

public class GasnicaPickup : MonoBehaviour
{
    private bool wZasiegu = false;
    private GasnicaSystem systemGracza;

    [Header("UI Interakcji")]
    public TextMeshProUGUI interactionText; 
    public string tekstPodniesienia = "Naciśnij [E] aby wziąć gaśnicę";

    void Start()
    {
        // Upewniamy się, że tekst jest wyłączony na starcie
        if (interactionText != null) interactionText.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            wZasiegu = true;
            systemGracza = other.GetComponent<GasnicaSystem>();

            if (interactionText != null)
            {
                interactionText.text = tekstPodniesienia;
                interactionText.gameObject.SetActive(true);
            }
            Debug.Log("Gracz wszedł w strefę gaśnicy!");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            wZasiegu = false;
            systemGracza = null;

            if (interactionText != null)
            {
                interactionText.gameObject.SetActive(false);
            }
            Debug.Log("Gracz wyszedł ze strefy gaśnicy.");
        }
    }

    void Update()
    {
        if (wZasiegu && Input.GetKeyDown(KeyCode.E) && systemGracza != null)
        {
            // Wyłączamy tekst przed zniszczeniem obiektu
            if (interactionText != null) interactionText.gameObject.SetActive(false);
            
            systemGracza.PodniesGasnice();
            Destroy(gameObject); 
        }
    }
}
