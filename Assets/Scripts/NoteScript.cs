using UnityEngine;
using TMPro;

public class NoteScript : MonoBehaviour
{
    [Header("UI Elementy")]
    public TextMeshProUGUI interactionText; 
    public GameObject notePanel;            

    private bool isPlayerNear = false;
    private bool isReading = false;

    void Start()
    {
        if (notePanel != null) notePanel.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            if (!isReading)
            {
                OpenNote();
            }
            else
            {
                CloseNote();
            }
        }
    }

    void OpenNote()
    {
        isReading = true;
        notePanel.SetActive(true);
        
        // WYŁĄCZAMY żółty napis całkowicie na czas czytania
        if (interactionText != null) 
            interactionText.gameObject.SetActive(false);
            
        // Jeśli chcesz, żeby gracz nie mógł chodzić podczas czytania, odkomentuj to:
        // Time.timeScale = 0f; 
    }

    public void CloseNote()
    {
        isReading = false;
        notePanel.SetActive(false);
        
        // WŁĄCZAMY napis z powrotem, bo gracz nadal jest blisko kartki
        if (interactionText != null)
        {
            interactionText.text = "Naciśnij [E] aby przeczytać";
            interactionText.gameObject.SetActive(true);
        }

        // Time.timeScale = 1f; // Przywrócenie czasu
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            if (interactionText != null)
            {
                interactionText.text = "Naciśnij [E] aby przeczytać";
                interactionText.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            isReading = false;
            if (notePanel != null) notePanel.SetActive(false);
            if (interactionText != null) interactionText.gameObject.SetActive(false);
        }
    }
}
