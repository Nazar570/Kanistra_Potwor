using UnityEngine;
using TMPro;

public class CarRequirements : MonoBehaviour
{
    [Header("UI - Opcjonalny prosty napis (np. [E] Interakcja)")]
    public TextMeshProUGUI interactionText;

    [Header("Referencje do skryptów")]
    public CarDriving carDriving;
    public CarInteraction carInteraction;

    [Header("NOWOŚĆ: System Zasadzki i Ścieżki")]
    public AmbushTrigger ambushTriggerScript;
    public AmbushPath ambushPathScript;


    [Header("NOWOŚĆ: Obiekt Krzyku")]
[Tooltip("Przeciągnij tutaj obiekt sciezkakrzyk z Hierarchy")]
public GameObject sciezkaKrzyk;

// --- DOPISZ TO POLE ---
[Tooltip("Przeciągnij tutaj bezpośrednio obiekt MUTANTA ze środka ścieżki")]
public GameObject mutantBezposredni;
    private bool isPlayerNear = false;
    private bool akumulatorZalozony = false;
    private bool swiecaZalozona = false;
    private bool kanisterZalany = false;
    private bool wAucie = false;

    public bool blokadaWsiadania = false;
    public float czasBlokady = 0f;

    private HintManager hintManager;
    private MonoBehaviour playerMovementScript;

    void Start()
{
    if (carDriving != null) carDriving.enabled = false;
    if (carInteraction != null) carInteraction.enabled = false;
    hintManager = Object.FindFirstObjectByType<HintManager>();

    if (sciezkaKrzyk != null) sciezkaKrzyk.SetActive(false);
    
    // --- DOPISZ TO ---
    if (mutantBezposredni != null) mutantBezposredni.SetActive(false); 
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

            SprawdzWarunekKrzyku(); // DODANE: Sprawdzamy, czy mamy już komplet (świeca + aku)
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

            SprawdzWarunekKrzyku(); // DODANE: Sprawdzamy, czy mamy już komplet (świeca + aku)
        }

        if (!kanisterZalany && Ekwipunek.maKanister)
        {
            kanisterZalany = true;
            Ekwipunek.maKanister = false;
            Debug.Log("Paliwo wlane!");
            cosZmieniono = true;

            BearAttackEvent bearEvent = Object.FindFirstObjectByType<BearAttackEvent>();
            if (bearEvent != null)
            {
                bearEvent.TriggerBearAttack(playerMovementScript);
            }
        }

        if (!cosZmieniono)
            Debug.Log("Nie masz nic do włożenia do auta.");

        // SPRAWDZENIE: Jeśli wszystkie części są na miejscu, odpalamy wołanie oraz nową ścieżkę!
        if (akumulatorZalozony && swiecaZalozona && kanisterZalany)
        {
            if (carDriving != null) carDriving.enabled = true;
            if (carInteraction != null) carInteraction.enabled = false;

            if (ambushTriggerScript != null)
            {
                ambushTriggerScript.ActivateAmbushSequence();
            }

            // POPRAWKA: Odpalamy funkcję aktywacji ścieżki
            if (ambushPathScript != null)
            {
                ambushPathScript.ActivatePath();
            }
        }
    }

    // DODANE: Nowa, czysta metoda sprawdzająca obecność świecy i akumulatora
    private void SprawdzWarunekKrzyku()
{
    if (akumulatorZalozony && swiecaZalozona)
    {
        if (sciezkaKrzyk != null)
        {
            sciezkaKrzyk.SetActive(true);
        }

        // --- DOPISZ TO: Wymuszamy włączenie mutanta prosto z kodu bazy ---
        if (mutantBezposredni != null)
        {
            mutantBezposredni.SetActive(true);
            Debug.Log("KOD: Wymusiłem włączenie mutanta bezposrednio!");
        }
    }
}

    void UruchomWsiadanie()
    {
        if (carDriving != null)
        {
            wAucie = true;
            carDriving.enabled = true;
            carDriving.WymusWsiadanieBezposrednie();

            if (hintManager != null && hintManager.hintPanel != null)
                hintManager.hintPanel.SetActive(false);
        }
    }

    public void GraczWysiadl()
    {
        wAucie = false;
    }

    void AktualizujHintWUpdate()
    {
        if (hintManager == null) return;

        // ETAP 3: Wszystko jest naprawione -> Aktywujemy wołanie z budynku!
        if (akumulatorZalozony && swiecaZalozona && kanisterZalany)
        {
            hintManager.ShowHint("Co to? Jakiś krzyk!\nMuszę to sprawdzić!", 0.5f);
            return;
        }

        // ETAP 1: Paliwo jeszcze nie jest wlane -> Pokazujemy TYLKO napisy dotyczące benzyny
        if (!kanisterZalany)
        {
            string tekstPaliwa = Ekwipunek.maKanister ? "Naciśnij [F] aby wlać paliwo\n" : "Pusty bak\n Może gdzieś znajdę trochę benzyny...";
            hintManager.ShowHint(tekstPaliwa.TrimEnd(), 0.5f);
            return;
        }

        // ETAP 2: Paliwo wlane (zwierzak już zaatakował) -> Pokazujemy napisy o akumulatorze i świecy
        string tekstNaprawy = "";

        if (!akumulatorZalozony)
            tekstNaprawy += Ekwipunek.maAkumulator ? "Naciśnij [F] aby założyć akumulator\n" : "Potrzebny akumulator!\n";

        if (!swiecaZalozona)
            tekstNaprawy += Ekwipunek.maSwiecaZaplonowa ? "Naciśnij [F] aby założyć świecę zapłonową\n" : "Potrzebna świeca zapłonowa!\n";

        hintManager.ShowHint(tekstNaprawy.TrimEnd(), 0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            playerMovementScript = other.GetComponent<MonoBehaviour>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (wAucie) return;
            isPlayerNear = false;
            if (hintManager != null && hintManager.hintPanel != null)
                hintManager.hintPanel.SetActive(false);
        }
    }
}