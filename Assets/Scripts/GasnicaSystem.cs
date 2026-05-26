using UnityEngine;
using TMPro;
using System.Collections;

public class GasnicaSystem : MonoBehaviour
{
    [Header("Modele i Efekty")]
    public GameObject modelWRekach; // Przeciągnij tu zablokowaną gaśnicę spod kamery
    public ParticleSystem chmuraGazu; // Przeciągnij Particle System

    [Header("UI")]
    public TextMeshProUGUI tekstZoltyPodpowiedz;
    public TextMeshProUGUI tekstBialyMysli;

    [Header("Ustawienia Strzału")]
    public float zasiegGazu = 6f; // Jak daleko psika
    public float katGazu = 60f; // Szerokość stożka chmury

    private bool maGasnice = false;
    private bool zostalaUzyta = false;
    private Transform kamera;

    void Start()
    {
        kamera = Camera.main.transform;
        if (modelWRekach != null) modelWRekach.SetActive(false);
        if (tekstZoltyPodpowiedz != null) tekstZoltyPodpowiedz.text = "";
        if (tekstBialyMysli != null) tekstBialyMysli.text = "";
    }

    public void PodniesGasnice()
    {
        if (maGasnice) return;
        
        maGasnice = true;
        if (modelWRekach != null) modelWRekach.SetActive(true);
        
        Debug.Log("Metoda PodniesGasnice wywołana! Uruchamiam Coroutine...");
        
        if (tekstZoltyPodpowiedz == null || tekstBialyMysli == null)
        {
            Debug.LogError("Nazar, nie przypiąłeś tekstów w Inspektorze do skryptu GasnicaSystem!");
            return;
        }

        StartCoroutine(PokazKomunikatyStartowe());
    }

    IEnumerator PokazKomunikatyStartowe()
    {
        Debug.Log("Ustawiam teksty na ekranie...");
        tekstZoltyPodpowiedz.text = "Żeby użyć naciśnij [G]";
        tekstBialyMysli.text = "chyba do obrony...";
        
        float timer = 0;
        // Pętla trwa 7 sekund (tylko myśli gracza tyle wiszą)
        while(timer < 7f && !zostalaUzyta)
        {
            timer += Time.deltaTime;

            // Wyłącz żółty tekst dokładnie po 5 sekundach
            if (timer >= 5f && tekstZoltyPodpowiedz.text != "")
            {
                tekstZoltyPodpowiedz.text = "";
            }

            yield return null;
        }

        if (!zostalaUzyta)
        {
            tekstZoltyPodpowiedz.text = "";
            tekstBialyMysli.text = "";
            Debug.Log("Czas minął, czyszczę teksty.");
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
        
        // Aktualizacja tekstów
        tekstZoltyPodpowiedz.text = "";
        tekstBialyMysli.text = "trzeba szybko schować się!";
        
        // Odpal gaz
        chmuraGazu.Play();

        // Czekamy chwilę, symulując "psikanie" przez 3 sekundy
        float czasPsikania = 3f;
        float timer = 0f;

        // W trakcie psikania ciągle sprawdzamy, czy mutant wszedł w chmurę
        while (timer < czasPsikania)
        {
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
        modelWRekach.SetActive(false); // Wywalamy pustą gaśnicę
        maGasnice = false;

        // Trzymaj komunikat jeszcze przez 7 sekund (razem 10s od strzału)
        yield return new WaitForSeconds(7f);
        tekstBialyMysli.text = "";
    }
}
