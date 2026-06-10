using UnityEngine;

public class OdbierzNagrode : MonoBehaviour
{
    public float zasiegOdbioru = 3f;
    public KeyCode klawiszOdbioru = KeyCode.E;

    [Header("Modele nagród w rogu ekranu")]
    public GameObject model3DBaterii;
    public GameObject model3DSwieca;

    private GameObject nagrodaNaZiemi;
    private bool odebrano = false;
    private string typNagrody = "";

    private ScoreManager scoreManager;
    private PuszkiManager puszkiManager;
    private HintManager hintManager;

    void Start()
    {
        scoreManager = Object.FindFirstObjectByType<ScoreManager>();
        puszkiManager = Object.FindFirstObjectByType<PuszkiManager>();
        hintManager = Object.FindFirstObjectByType<HintManager>();

        if (model3DBaterii != null) model3DBaterii.SetActive(false);
        if (model3DSwieca != null) model3DSwieca.SetActive(false);
    }

    public void UstawNagrodeNaZiemi(GameObject obj)
    {
        nagrodaNaZiemi = obj;
        typNagrody = "bateria";
        odebrano = false;
    }
  
    public void UstawSwiece(GameObject obj)
    {
        nagrodaNaZiemi = obj;
        typNagrody = "swieca";
        odebrano = false;
    }

    void Update()
    {
        if (odebrano || nagrodaNaZiemi == null) return;

        float dist = Vector3.Distance(transform.position, nagrodaNaZiemi.transform.position);

        if (dist <= zasiegOdbioru)
        {
            // Wyświetl podpowiedź pobrania, dopóki gracz stoi blisko przedmiotu
            if (hintManager != null)
            {
                string nazwaPrzedmiotu = typNagrody == "bateria" ? "Akumulator" : "Świecę zapłonową";
                hintManager.ShowHint($"Naciśnij <color=white>[ {klawiszOdbioru} ]</color> aby podnieść {nazwaPrzedmiotu}.", 0.2f);
            }

            if (Input.GetKeyDown(klawiszOdbioru))
                Odbierz();
        }
    }

    void Odbierz()
    {
        odebrano = true;
        
        if (hintManager != null && hintManager.hintPanel != null)
            hintManager.hintPanel.SetActive(false);

        if (typNagrody == "bateria")
        {
            if (model3DBaterii != null) model3DBaterii.SetActive(true);
            Ekwipunek.maAkumulator = true;
            Debug.Log("[NAGRODA] Akumulator dodany do ekwipunku!");

            if (scoreManager != null) scoreManager.CzyscTekstPoOdebraniuNagrody();
        }
        else if (typNagrody == "swieca")
        {
            if (model3DSwieca != null) model3DSwieca.SetActive(true);
            Ekwipunek.maSwiecaZaplonowa = true;
            Debug.Log("[NAGRODA] Świeca zapłonowa dodana do ekwipunku!");

            if (puszkiManager != null) puszkiManager.CzyscTekst();
        }
        Destroy(nagrodaNaZiemi);
    }
}