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

    void Start()
    {
        scoreManager = FindFirstObjectByType<ScoreManager>();
        puszkiManager = FindFirstObjectByType<PuszkiManager>();

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

        if (dist <= zasiegOdbioru && Input.GetKeyDown(klawiszOdbioru))
            Odbierz();
    }

    void Odbierz()
    {
        odebrano = true;

        if (typNagrody == "bateria")
        {
            if (model3DBaterii != null)
                model3DBaterii.SetActive(true);

            if (scoreManager != null)
                scoreManager.CzyscTekstPoOdebraniuNagrody();
        }
        else if (typNagrody == "swieca")
        {
            if (model3DSwieca != null)
                model3DSwieca.SetActive(true);

            if (puszkiManager != null)
                puszkiManager.CzyscTekst();
        }

        Destroy(nagrodaNaZiemi);
        Debug.Log($"[NAGRODA] Odebrano: {typNagrody}");
    }
}