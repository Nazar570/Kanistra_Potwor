using UnityEngine;

public class NoteScript : MonoBehaviour
{
    [Header("UI Elementy")]
    public GameObject notePanel;            

    private bool isPlayerNear = false;
    private bool isReading = false;
    private HintManager hintManager;

    void Start()
    {
        hintManager = Object.FindFirstObjectByType<HintManager>();
        if (notePanel != null) notePanel.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNear)
        {
            // Wyświetl podpowiedź interakcji na jasnym panelu (tylko jeśli gracz nie czyta aktualnie)
            if (!isReading && hintManager != null)
            {
                hintManager.ShowHint("<color=black>Naciśnij [E] aby przeczytać</color>", 0.5f);
            }

            if (Input.GetKeyDown(KeyCode.E))
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
    }

    void OpenNote()
    {
        isReading = true;
        notePanel.SetActive(true);
        
        // Wyłączamy panel hinta całkowicie na czas czytania samej kartki
        if (hintManager != null && hintManager.hintPanel != null) 
            hintManager.hintPanel.SetActive(false);
    }

    public void CloseNote()
    {
        isReading = false;
        notePanel.SetActive(false);
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
            
            // Gasimy podpowiedź po odejściu od listu/kartki
            if (hintManager != null && hintManager.hintPanel != null)
                hintManager.hintPanel.SetActive(false);
        }
    }
}