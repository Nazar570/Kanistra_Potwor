using UnityEngine;
using System.Collections;

public class GasnicaSystem : MonoBehaviour
{
    [Header("Modele i Efekty")]
    public GameObject modelWRekach; // Przeciągnij tu zablokowaną gaśnicę spod kamery
    public ParticleSystem chmuraGazu; // Przeciągnij Particle System

    [Header("Ustawienia Strzału")]
    public float zasiegGazu = 6f; // Jak daleko psika
    public float katGazu = 60f; // Szerokość stożka chmury

    private bool maGasnice = false;
    private bool zostalaUzyta = false;
    private Transform kamera;
    private HintManager hintManager;

    void Start()
    {
        kamera = Camera.main.transform;
        if (modelWRekach != null) modelWRekach.SetActive(false);
        
        // Automatyczne znalezienie globalnego managera podpowiedzi
        hintManager = Object.FindFirstObjectByType<HintManager>();
    }

    public void PodniesGasnice()
    {
        if (maGasnice) return;
        
        maGasnice = true;
        if (modelWRekach != null) modelWRekach.SetActive(true);
        
        Debug.Log("Metoda PodniesGasnice wywołana! Uruchamiam Coroutine...");
        
        StartCoroutine(PokazKomunikatyStartowe());
    }

    IEnumerator PokazKomunikatyStartowe()
    {
        float timer = 0;
        // Wyświetlamy pierwszą instrukcję przez pierwsze 5 sekund (ciemny kolor)
        while(timer < 7f && !zostalaUzyta)
        {
            timer += Time.deltaTime;

            if (hintManager != null && !zostalaUzyta)
            {
                if (timer < 5f)
                {
                    hintManager.ShowHint("<color=#1A1A1A>Żeby użyć naciśnij [G]</color>", 0.5f);
                }
                else
                {
                    hintManager.ShowHint("<color=#333333>chyba do obrony...</color>", 0.5f);
                }
            }

            yield return null;
        }
    }

    void Update()
    {
        if (maGasnice && !zostalaUzyta && Input.GetKeyDown(KeyCode.G))
        {
            StartCoroutine(UzyjGasnicy());
        }
    }

    IEnumerator UzyjGasnicy()
    {
        zostalaUzyta = true;
        
        // Odpal gaz
        chmuraGazu.Play();

        // Czekamy chwilę, symulując "psikanie" przez 3 sekundy
        float czasPsikania = 3f;
        float timer = 0f;

        // W trakcie psikania ciągle sprawdzamy, czy mutant wszedł w chmurę
        while (timer < czasPsikania)
        {
            if (hintManager != null)
            {
                hintManager.ShowHint("<color=#1A1A1A>trzeba szybko schować się!</color>", 0.5f);
            }

            Collider[] trafienia = Physics.OverlapSphere(kamera.position, zasiegGazu);
            foreach (var trafienie in trafienia)
            {
                MutantAI mutant = trafienie.GetComponentInParent<MutantAI>();
                if (mutant != null)
                {
                    // 1. Sprawdzamy czy mutant jest w zasięgu naszej kamery (przed nami)
                    Vector3 kierunekDoMutanta = (mutant.transform.position - kamera.position).normalized;
                    if (Vector3.Angle(kamera.forward, kierunekDoMutanta) < katGazu / 2f)
                    {
                        // 2. Sprawdzamy czy mutant jest obrócony do nas przodem
                        Vector3 mutantDoGracza = (kamera.position - mutant.transform.position).normalized;
                        float katPatrzeniaMutanta = Vector3.Angle(mutant.transform.forward, mutantDoGracza);

                        if (katPatrzeniaMutanta < 85f)
                        {
                            // Trafienie w twarz - ogłusza
                            mutant.OgluszMutanta();
                        }
                        else
                        {
                            // Trafienie w plecy - potwór się wkurza i odwraca
                            mutant.ZauwazTrafienieWPlecy();
                        }
                    }
                }
            }
            timer += Time.deltaTime;
            yield return null;
        }

        chmuraGazu.Stop();
        if (modelWRekach != null) modelWRekach.SetActive(false); // Wywalamy pustą gaśnicę
        maGasnice = false;

        // Podtrzymujemy myśl gracza po strzale jeszcze przez chwilę
        timer = 0f;
        while (timer < 7f)
        {
            timer += Time.deltaTime;
            if (hintManager != null)
            {
                hintManager.ShowHint("<color=#1A1A1A>trzeba szybko schować się!</color>", 0.5f);
            }
            yield return null;
        }
    }
}