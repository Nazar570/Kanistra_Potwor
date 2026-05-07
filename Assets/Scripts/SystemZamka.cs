using UnityEngine;
using TMPro;

public class SystemZamka : MonoBehaviour
{
    public TextMeshProUGUI interactionText;
    
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

    void Start()
    {
        // Uśpienie zwykłych drzwi (przestają działać na "E")
        if (skryptDrzwi != null) skryptDrzwi.enabled = false;
    }

    void Update()
    {
        // Sprawdza globalną pamięć, czy masz odpowiedni obrazek w ekwipunku
        bool maOdpowiedniPrzedmiot = wymagaLomu ? Ekwipunek.maLom : Ekwipunek.maKlucz;

        if (isPlayerNear && Input.GetKeyDown(KeyCode.F) && maOdpowiedniPrzedmiot)
        {
            OtworzZamek();
        }
    }

    void OtworzZamek()
    {
        // "Zjada" przedmiot z ekwipunku (obrazek w rogu zniknie sam)
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

        // Wyłącz tekst bezpiecznie i zniszcz ten skrypt
        if (interactionText != null) interactionText.gameObject.SetActive(false);
        Destroy(this); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            AktualizujTekst();
            
            // Włączamy tekst (zabezpieczone)
            if (interactionText != null) interactionText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            
            // Wyłączamy tekst (zabezpieczone)
            if (interactionText != null) interactionText.gameObject.SetActive(false);
        }
    }

    private void AktualizujTekst()
    {
        if (interactionText == null) return; // Jeśli nie ma przypisanego tekstu, nie rób nic
        
        bool maOdpowiedniPrzedmiot = wymagaLomu ? Ekwipunek.maLom : Ekwipunek.maKlucz;
        interactionText.text = maOdpowiedniPrzedmiot ? tekstGdyMasz : tekstGdyNieMasz;
    }
}
