using UnityEngine;

public class SystemZamka : MonoBehaviour
{
    [Header("Co jest potrzebne?")]
    [Tooltip("Zaznacz = Łom. Odznacz = Klucz.")]
    public bool wymagaLomu = true; 
    
    [Header("Teksty przedmiotu")]
    public string tekstGdyNieMasz = "Wisi zamek (Potrzebny Łom)";
    public string tekstGdyMasz = "Złam zamek [F]";

    [Header("Co ma się odblokować?")]
    [Tooltip("Przeciągnij tutaj DoorScript z tych drzwi")]
    public DoorScript skryptDrzwi;
    
    [Tooltip("Przeciągnij tu model kłódki/zamka 3D z drzwi (Zostaw puste dla wielkich drzwi)")]
    public GameObject modelKlodki;

    private bool isPlayerNear = false;
    private HintManager hintManager;

    void Start()
    {
        // Szukamy centralnego managera podpowiedzi
        hintManager = Object.FindFirstObjectByType<HintManager>();

        // Uśpienie zwykłych drzwi (przestają działać na "E")
        if (skryptDrzwi != null) skryptDrzwi.enabled = false;
    }

    void Update()
    {
        if (!isPlayerNear) return;

        // Sprawdza globalną pamięć, czy masz odpowiedni obrazek w ekwipunku
        bool maOdpowiedniPrzedmiot = wymagaLomu ? Ekwipunek.maLom : Ekwipunek.maKlucz;

        // Wyświetlanie hinta co klatkę (kolor czarny)
        if (hintManager != null)
        {
            string wiadomosc = maOdpowiedniPrzedmiot ? tekstGdyMasz : tekstGdyNieMasz;
            hintManager.ShowHint($"<color=black>{wiadomosc}</color>", 0.5f);
        }

        if (Input.GetKeyDown(KeyCode.F) && maOdpowiedniPrzedmiot)
        {
            OdblokujZamki();
        }
    }

    private void OdblokujZamki()
    {
        // Zużywa przedmiot z ekwipunku
        if (wymagaLomu) Ekwipunek.maLom = false;
        else Ekwipunek.maKlucz = false;

        // Budzi zwykłe drzwi (teraz "E" znów działa)
        if (skryptDrzwi != null)
        {
            skryptDrzwi.enabled = true;
            skryptDrzwi.czyMoznaOtworzyc = true; 
        }

        // Fizycznie niszczy model kłódki 3D, żeby odpadł z drzwi
        if (modelKlodki != null) Destroy(modelKlodki);

        // Wyłącz panel hinta natychmiast, żeby nie wisiał po usunięciu skryptu
        if (hintManager != null && hintManager.hintPanel != null)
            hintManager.hintPanel.SetActive(false);

        Destroy(this); 
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
            
            // Gasimy panel po odejściu od drzwi z kłódką
            if (hintManager != null && hintManager.hintPanel != null)
                hintManager.hintPanel.SetActive(false);
        }
    }
}