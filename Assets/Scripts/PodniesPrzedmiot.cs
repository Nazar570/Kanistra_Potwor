using UnityEngine;
using TMPro;

public class PodniesPrzedmiot : MonoBehaviour
{
    public TextMeshProUGUI interactionText;
    
    [Tooltip("Zaznacz dla Łomu. Odznacz dla Klucza.")]
    public bool czyToLom = true; 
    public string nazwaPrzedmiotu = "Łom";

    private bool isPlayerNear = false;

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.F))
        {
            // Dodaje przedmiot do pamięci (włącza obrazek w rogu)
            if (czyToLom) Ekwipunek.maLom = true;
            else Ekwipunek.maKlucz = true;

            // Sprząta napis i usuwa model 3D ze stołu
            if (interactionText != null) interactionText.gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            interactionText.text = "Podnieś " + nazwaPrzedmiotu + " [F]";
            interactionText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            interactionText.gameObject.SetActive(false);
        }
    }
}
