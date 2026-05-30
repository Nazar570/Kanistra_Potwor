using UnityEngine;
using TMPro;

public class CarRequirements : MonoBehaviour
{
    [Header("UI - Tekst przy aucie")]
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

    void Start()
    {
        if (carDriving != null) carDriving.enabled = false;
        if (carInteraction != null) carInteraction.enabled = false;
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

        AktualizujTekst();

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
            Debug.Log("Œwieca zap³onowa zamontowana!");
            cosZmieniono = true;
        }

        if (!kanisterZalany && Ekwipunek.maKanister)
        {
            kanisterZalany = true;
            Ekwipunek.maKanister = false;
            Debug.Log("Paliwo wlane!");
            cosZmieniono = true;
        }

        if (!cosZmieniono)
            Debug.Log("Nie masz nic do w³o¿enia do auta.");

        if (akumulatorZalozony && swiecaZalozona && kanisterZalany)
        {
            if (interactionText != null)
                interactionText.text = "Auto gotowe! Naciœnij [E], aby wsi¹œæ";
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
        }
    }

    public void GraczWysiadl()
    {
        wAucie = false;
    }

    void AktualizujTekst()
    {
        if (interactionText == null) return;

        if (akumulatorZalozony && swiecaZalozona && kanisterZalany)
        {
            interactionText.text = "Auto gotowe! Naciœnij [E], aby wsi¹œæ";
            return;
        }

        string tekst = "";

        if (!akumulatorZalozony)
            tekst += Ekwipunek.maAkumulator
                ? "Naciœnij [F] aby za³o¿yæ akumulator\n"
                : "Potrzebny akumulator!\n";

        if (!swiecaZalozona)
            tekst += Ekwipunek.maSwiecaZaplonowa
                ? "Naciœnij [F] aby za³o¿yæ œwiecê zap³onow¹\n"
                : "Potrzebna œwieca zap³onowa!\n";

        if (!kanisterZalany)
            tekst += Ekwipunek.maKanister
                ? "Naciœnij [F] aby wlaæ paliwo\n"
                : "Potrzebne paliwo!\n";

        interactionText.text = tekst.TrimEnd();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            if (interactionText != null)
                interactionText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Nie resetuj jeœli gracz w³aœnie wsiad³ do auta
            if (wAucie) return;

            isPlayerNear = false;
            if (interactionText != null)
                interactionText.gameObject.SetActive(false);
        }
    }
}