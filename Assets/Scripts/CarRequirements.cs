using UnityEngine;
using TMPro;

public class CarRequirements : MonoBehaviour
{
    [Header("UI - Opcjonalny prosty napis (np. [E] Interakcja)")]
    public TextMeshProUGUI interactionText;

    [Header("Referencje do skryptów")]
    public CarDriving carDriving;
    public CarInteraction carInteraction;

    private bool isPlayerNear = false;
    private bool akumulatorZalozony = false;
    private bool swiecaZalozona = false;
    private bool kanisterZalany = false;
    private bool wAucie = false;

    public bool blokadaWsiadania = false;
    public float czasBlokady = 0f;

    private HintManager hintManager; // System podpowiedzi
    private MonoBehaviour playerMovementScript; // Zapamiętany skrypt ruchu gracza

    void Start()
    {
        if (carDriving != null) carDriving.enabled = false;
        if (carInteraction != null) carInteraction.enabled = false;

        // Automatycznie szukamy HintManagera na tej scenie
        hintManager = Object.FindFirstObjectByType<HintManager>();
    }

    void Update()
    {
        if (!isPlayerNear) return;

        if (blokadaWsiadania)
        {
            czasBlokady -= Time.deltaTime;
            if (czasBlokady <= 0f)
                blokadaWsiadania = false;
            return;
        }

        // NOWOŚĆ/POWRÓT: Wysyłamy tekst do HintManagera na bieżąco, gdy gracz stoi obok auta
        AktualizujHintWUpdate();

        if (!akumulatorZalozony || !swiecaZalozona || !kanisterZalany)
        {
            if (Input.GetKeyDown(KeyCode.F))
                WlozCzescDoAuta();
        }

        if (akumulatorZalozony && swiecaZalozona && kanisterZalany)
        {
            if (Input.GetKeyDown(KeyCode.E))
                UruchomWsiadanie();
        }
    }

    void WlozCzescDoAuta()
    {
        bool cosZmieniono = false;

        if (!akumulatorZalozony && Ekwipunek.maAkumulator)
        {
            akumulatorZalozony = true;
            Ekwipunek.maAkumulator = false;
            OdbierzNagrode odbiornik = FindFirstObjectByType<OdbierzNagrode>();
            if (odbiornik != null && odbiornik.model3DBaterii != null)
                odbiornik.model3DBaterii.SetActive(false);
            Debug.Log("Akumulator zamontowany!");
            cosZmieniono = true;
        }

        if (!swiecaZalozona && Ekwipunek.maSwiecaZaplonowa)
        {
            swiecaZalozona = true;
            Ekwipunek.maSwiecaZaplonowa = false;
            OdbierzNagrode odbiornik = FindFirstObjectByType<OdbierzNagrode>();
            if (odbiornik != null && odbiornik.model3DSwieca != null)
                odbiornik.model3DSwieca.SetActive(false);
            Debug.Log("Świeca zapłonowa zamontowana!");
            cosZmieniono = true;
        }

        if (!kanisterZalany && Ekwipunek.maKanister)
        {
            kanisterZalany = true;
            Ekwipunek.maKanister = false;
            Debug.Log("Paliwo wlane!");
            cosZmieniono = true;

            // WYWOŁANIE EVENTU: Szukamy skryptu obsługującego atak niedźwiedzia i go odpalamy
            BearAttackEvent bearEvent = Object.FindFirstObjectByType<BearAttackEvent>();
            if (bearEvent != null)
            {
                bearEvent.TriggerBearAttack(playerMovementScript);
            }
        }

        if (!cosZmieniono)
            Debug.Log("Nie masz nic do włożenia do auta.");

        if (akumulatorZalozony && swiecaZalozona && kanisterZalany)
        {
            if (carDriving != null) carDriving.enabled = true;
            if (carInteraction != null) carInteraction.enabled = false;
        }
    }

    void UruchomWsiadanie()
    {
        if (carDriving != null)
        {
            wAucie = true;
            carDriving.enabled = true;
            carDriving.WymusWsiadanieBezposrednie();
            
            // Ukrywamy panel hinta po wsiadaniu, żeby nie wisiał na ekranie
            if (hintManager != null && hintManager.hintPanel != null)
                hintManager.hintPanel.SetActive(false);
        }
    }

    public void GraczWysiadl()
    {
        wAucie = false;
    }

    // Nowa wersja starej funkcji - teraz karmi danymi HintManagera
    void AktualizujHintWUpdate()
    {
        if (hintManager == null) return;

        if (akumulatorZalozony && swiecaZalozona && kanisterZalany)
        {
            hintManager.ShowHint("Auto gotowe! Naciśnij [E], aby wsiąść", 0.5f);
            return;
        }

        string tekst = "";

        if (!akumulatorZalozony)
            tekst += Ekwipunek.maAkumulator
                ? "Naciśnij [F] aby założyć akumulator\n"
                : "Potrzebny akumulator!\n";

        if (!swiecaZalozona)
            tekst += Ekwipunek.maSwiecaZaplonowa
                ? "Naciśnij [F] aby założyć świecę zapłonową\n"
                : "Potrzebna świeca zapłonowa!\n";

        if (!kanisterZalany)
            tekst = Ekwipunek.maKanister
                ? "Naciśnij [F] aby wlać paliwo\n"
                : "Pusty bak\n Może gdzieś znajdę trochę benzyny...";

        // Wysyłamy tekst co klatkę z bardzo krótkim czasem wyświetlania (0.5 sekundy),
        // dzięki czemu podpowiedź zniknie od razu po odejściu od auta
        hintManager.ShowHint(tekst.TrimEnd(), 0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;

            // Zapisujemy skrypt ruchu gracza, który wszedł w trigger
            playerMovementScript = other.GetComponent<MonoBehaviour>();

            // UWAGA: Jeśli Twój skrypt ruchu to np. FirstPersonController i ukrywa się głębiej, 
            // w razie problemów podmień linijkę wyżej na:
            // playerMovementScript = other.GetComponent("FirstPersonController") as MonoBehaviour;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (wAucie) return;

            isPlayerNear = false;
            
            // Po odejściu od auta natychmiast gasimy panel HintManagera
            if (hintManager != null && hintManager.hintPanel != null)
                hintManager.hintPanel.SetActive(false);
        }
    }
}