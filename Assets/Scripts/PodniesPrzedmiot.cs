using UnityEngine;

public class PodniesPrzedmiot : MonoBehaviour
{
    [Header("Ustawienia przedmiotu")]
    [Tooltip("Zaznacz dla Łomu. Odznacz dla Klucza.")]
    public bool czyToLom = true; 
    public string nazwaPrzedmiotu = "Łom";

    private bool isPlayerNear = false;
    private HintManager hintManager; // System podpowiedzi

    void Start()
    {
        // Szukamy HintManagera na scenie automatycznie
        hintManager = Object.FindFirstObjectByType<HintManager>();
    }

    void Update()
    {
        if (!isPlayerNear) return;

        // Wyświetlanie hinta co klatkę z wymuszonym czarnym kolorem (zniknie automatycznie po odejściu)
        if (hintManager != null)
        {
            hintManager.ShowHint($"<color=black>Naciśnij [F], aby podnieść {nazwaPrzedmiotu}</color>", 0.5f);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            // Dodaje przedmiot do pamięci (włącza obrazek w rogu)
            if (czyToLom) Ekwipunek.maLom = true;
            else Ekwipunek.maKlucz = true;
            
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
            
            // Gasimy natychmiast po wyjściu z triggera przedmiotu
            if (hintManager != null && hintManager.hintPanel != null)
                hintManager.hintPanel.SetActive(false);
        }
    }
}